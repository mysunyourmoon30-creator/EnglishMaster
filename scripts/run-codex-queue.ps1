param(
    [int]$StartFrom = 58,
    [int]$EndAt = 161,
    [switch]$DryRun,
    [switch]$RetryOnLimit,
    [switch]$IncludeFailed,
    [int]$LimitCheckMinutes = 60,
    [int]$MaxRetries = 999,
    [switch]$SkipBuild,
    [switch]$SkipTest
)

$QueueFile = ".\prompt-queue.json"
$LogDir = ".\logs\codex-queue"

New-Item -ItemType Directory -Force $LogDir | Out-Null

function Save-Queue($queue) {
    $queue | ConvertTo-Json -Depth 20 | Set-Content $QueueFile -Encoding UTF8
}

function Is-LimitError($text) {
    if ([string]::IsNullOrWhiteSpace($text)) {
        return $false
    }

    $patterns = @(
        "usage limit",
        "rate limit",
        "daily limit",
        "You've hit your usage limit",
        "depleted",
        "temporarily unavailable",
        "try again later"
    )

    foreach ($pattern in $patterns) {
        if ($text.ToLower().Contains($pattern.ToLower())) {
            return $true
        }
    }

    return $false
}

function Set-ItemStatus($queue, $item, $status, $notes = "") {
    $item.status = $status

    if ($item.PSObject.Properties.Name -notcontains "updated_at") {
        $item | Add-Member -NotePropertyName "updated_at" -NotePropertyValue ""
    }

    if ($item.PSObject.Properties.Name -notcontains "notes") {
        $item | Add-Member -NotePropertyName "notes" -NotePropertyValue ""
    }

    $item.updated_at = (Get-Date).ToString("s")

    if ($notes -ne "") {
        $item.notes = $notes
    }

    Save-Queue $queue
}

function Run-DotnetChecks {
    if (-not $SkipBuild) {
        dotnet restore
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet restore failed"
        }

        dotnet build
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet build failed"
        }
    }

    if (-not $SkipTest) {
        dotnet test
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet test failed"
        }
    }
}

function Run-CodexPrompt($promptFile, $logFile) {
    Write-Host "Running Codex prompt: $promptFile"

    $command = ".\.tools\codex\codex.exe"

    $logFolder = Split-Path $logFile -Parent
    New-Item -ItemType Directory -Force $logFolder | Out-Null

    if (-not (Test-Path $command)) {
        $message = "Project-local Codex CLI not found: $command"
        $message | Tee-Object -FilePath $logFile
        return @{
            ExitCode = 127
            Output = $message
        }
    }

    $versionOutput = & $command --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        $message = "Codex CLI exists but cannot run: $versionOutput"
        $message | Tee-Object -FilePath $logFile
        return @{
            ExitCode = 126
            Output = $message
        }
    }

    $promptContent = Get-Content $promptFile -Raw

    # Correct Codex CLI usage:
    # Pipe prompt content into: codex exec -
    $output = $promptContent | & $command exec - 2>&1
    $exitCode = $LASTEXITCODE

    $output | Tee-Object -FilePath $logFile

    return @{
        ExitCode = $exitCode
        Output = ($output -join "`n")
    }
}

while ($true) {
    if (-not (Test-Path $QueueFile)) {
        throw "prompt-queue.json not found"
    }

    $queue = Get-Content $QueueFile -Raw | ConvertFrom-Json

    $allowedStatuses = @("pending", "waiting_limit")

    if ($IncludeFailed) {
        $allowedStatuses += "failed"
    }

    $rangeItems = $queue.items |
        Where-Object {
            $_.id -ge $StartFrom `
            -and $_.id -le $EndAt
        } |
        Sort-Object id

    $items = $rangeItems |
        Where-Object {
            $allowedStatuses -contains $_.status
        } |
        Sort-Object id

    if ($items.Count -eq 0) {
        Write-Host "No pending items from Prompt $StartFrom to $EndAt."
        break
    }

    $nextItem = $items[0]
    $priorBlocker = $rangeItems |
        Where-Object {
            $_.id -lt $nextItem.id `
            -and ($_.status -eq "running" -or $_.status -eq "failed")
        } |
        Select-Object -First 1

    if ($priorBlocker) {
        $blockerId = "{0:D3}" -f [int]$priorBlocker.id
        throw "Prompt $blockerId is $($priorBlocker.status). Resolve it before running Prompt $($nextItem.id)."
    }

    $item = $nextItem
    $promptId = "{0:D3}" -f [int]$item.id
    $promptFile = $item.file
    $logFile = $item.log_file

    Write-Host "Next prompt: $promptId - $($item.title)"
    Write-Host "Status: $($item.status)"

    if (-not (Test-Path $promptFile)) {
        Set-ItemStatus $queue $item "failed" "Prompt file not found: $promptFile"
        throw "Prompt file not found: $promptFile"
    }

    $promptContent = Get-Content $promptFile -Raw

    if ([string]::IsNullOrWhiteSpace($promptContent)) {
        Set-ItemStatus $queue $item "failed" "Prompt file is empty"
        throw "Prompt file is empty: $promptFile"
    }

    if ($promptContent.Contains("PASTE FULL PROMPT CONTENT HERE")) {
        Set-ItemStatus $queue $item "failed" "Prompt content placeholder still exists"
        throw "Prompt file still has placeholder: $promptFile"
    }

    if ($DryRun) {
        Write-Host "[DryRun] Would run Prompt ${promptId}: $promptFile"
        break
    }

    $retryCount = 0

    while ($true) {
        $queue = Get-Content $QueueFile -Raw | ConvertFrom-Json
        $item = $queue.items | Where-Object { $_.id -eq [int]$promptId }

        Set-ItemStatus $queue $item "running" "Running prompt $promptId"

        $result = Run-CodexPrompt $promptFile $logFile

        if (Is-LimitError $result.Output) {
            $retryCount++

            Set-ItemStatus $queue $item "waiting_limit" "Codex limit detected. Retry $retryCount. Waiting $LimitCheckMinutes minutes."

            Write-Host "Codex limit detected on Prompt $promptId."
            Write-Host "Waiting $LimitCheckMinutes minutes before retry..."

            if (-not $RetryOnLimit) {
                Write-Host "RetryOnLimit is not enabled. Stop here."
                exit 1
            }

            if ($retryCount -ge $MaxRetries) {
                Set-ItemStatus $queue $item "failed" "Max retries reached after limit waiting."
                throw "Max retries reached for Prompt $promptId"
            }

            Start-Sleep -Seconds ($LimitCheckMinutes * 60)

            Write-Host "Retrying Prompt $promptId..."
            continue
        }

        if ($result.ExitCode -ne 0) {
            Set-ItemStatus $queue $item "failed" "Codex command failed. See log: $logFile"
            throw "Codex failed on Prompt $promptId"
        }

        try {
            Run-DotnetChecks

            $queue = Get-Content $QueueFile -Raw | ConvertFrom-Json
            $item = $queue.items | Where-Object { $_.id -eq [int]$promptId }

            Set-ItemStatus $queue $item "completed" "Prompt completed successfully."

            Write-Host "Prompt $promptId completed."
            break
        }
        catch {
            $queue = Get-Content $QueueFile -Raw | ConvertFrom-Json
            $item = $queue.items | Where-Object { $_.id -eq [int]$promptId }

            Set-ItemStatus $queue $item "failed" $_.Exception.Message
            throw
        }
    }
}
