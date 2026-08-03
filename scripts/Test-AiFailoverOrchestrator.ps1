[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$GitExcludesFile = ""

$repositoryRoot = (& git -c "core.excludesFile=$GitExcludesFile" rev-parse --show-toplevel 2>&1 | Out-String).Trim()
if ($LASTEXITCODE -ne 0) {
    throw "Run this test from the EnglishMaster repository."
}

$orchestrator = Join-Path $repositoryRoot "scripts/Invoke-AiFailoverOrchestrator.ps1"
$providerFile = Join-Path $repositoryRoot "automation/ai/providers.json"
$taskFile = Join-Path $repositoryRoot "automation/ai/tasks.json"
$testRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("englishmaster-ai-orchestrator-test-{0}" -f [guid]::NewGuid().ToString("N"))

function Assert-Throws {
    param(
        [Parameter(Mandatory)][scriptblock]$Action,
        [Parameter(Mandatory)][string]$ExpectedText
    )

    try {
        & $Action
    }
    catch {
        if ($_.Exception.Message -notlike "*$ExpectedText*") {
            throw "Expected error containing '$ExpectedText', received '$($_.Exception.Message)'."
        }
        return
    }
    throw "Expected an error containing '$ExpectedText', but the action succeeded."
}

function Write-TestJson {
    param(
        [Parameter(Mandatory)]$Value,
        [Parameter(Mandatory)][string]$Path
    )

    $json = $Value | ConvertTo-Json -Depth 30
    [System.IO.File]::WriteAllText($Path, $json, [System.Text.UTF8Encoding]::new($false))
}

function Copy-TaskContract {
    param([Parameter(Mandatory)]$Task)

    $copy = [ordered]@{}
    foreach ($key in $Task.Keys) {
        $copy[$key] = $Task[$key]
    }
    return $copy
}

function New-TestProviderConfiguration {
    param(
        [switch]$IncludeLimitProvider,
        [switch]$LimitOnly
    )

    $providers = @()
    if ($IncludeLimitProvider -or $LimitOnly) {
        $providers += [ordered]@{
            name = "fake-limit"
            priority = 1
            enabled = $true
            paid = $false
            command = "cmd.exe"
            arguments = @("/d", "/c", "echo usage limit 1>&2 & exit /b 1")
            versionArguments = @("/d", "/c", "exit /b 0")
            retryAfterMinutes = 60
        }
    }
    if (-not $LimitOnly) {
        $providers += [ordered]@{
            name = "fake-success"
            priority = 2
            enabled = $true
            paid = $false
            command = "cmd.exe"
            arguments = @("/d", "/c", "exit /b 0")
            versionArguments = @("/d", "/c", "exit /b 0")
            retryAfterMinutes = 60
        }
    }

    return [ordered]@{
        version = "1.0.0"
        providers = $providers
        rateLimitPatterns = @("usage limit")
    }
}

function New-RunnableTask {
    param(
        [Parameter(Mandatory)][string]$Id,
        [Parameter(Mandatory)][string]$Branch
    )

    return [ordered]@{
        id = $Id
        feature = "End-to-end fake provider"
        status = "queued"
        provider = $null
        previousProvider = $null
        branch = $Branch
        worktree = "."
        dependencies = @()
        allowedPaths = @("allowed/**")
        forbiddenPaths = @("forbidden/**")
        promptFile = "prompt.md"
        checks = @([ordered]@{
            name = "fake-check"
            command = "cmd.exe"
            arguments = @("/d", "/c", "exit /b 0")
        })
    }
}

try {
    [System.IO.Directory]::CreateDirectory($testRoot) | Out-Null

    $parseErrors = $null
    [void][System.Management.Automation.Language.Parser]::ParseFile(
        $orchestrator,
        [ref]$null,
        [ref]$parseErrors)
    if ($parseErrors.Count -gt 0) {
        throw "PowerShell parser errors: $($parseErrors -join '; ')"
    }

    & $orchestrator -TaskFile $taskFile -ProviderFile $providerFile -ValidateOnly

    $unknownProviderQueuePath = Join-Path $testRoot "unknown-provider.json"
    $unknownProviderTask = New-RunnableTask "UNKNOWN_PROVIDER" "test/unknown-provider"
    $unknownProviderTask.provider = "missing-provider"
    Write-TestJson ([ordered]@{
        version = "1.0.0"
        project = "EnglishMaster"
        tasks = @($unknownProviderTask)
    }) $unknownProviderQueuePath
    Assert-Throws {
        & $orchestrator -TaskFile $unknownProviderQueuePath -ProviderFile $providerFile -ValidateOnly
    } "unknown preferred provider"

    $invalidJsonPath = Join-Path $testRoot "invalid.json"
    [System.IO.File]::WriteAllText($invalidJsonPath, "{invalid", [System.Text.UTF8Encoding]::new($false))
    Assert-Throws {
        & $orchestrator -TaskFile $invalidJsonPath -ProviderFile $providerFile -ValidateOnly
    } "Invalid JSON"

    $baseTask = [ordered]@{
        id = "TEST-1"
        feature = "Test feature"
        status = "queued"
        provider = $null
        previousProvider = $null
        branch = "codex/test-1"
        worktree = "."
        dependencies = @()
        allowedPaths = @("tests/TestFeature/**")
        forbiddenPaths = @("**/*.csproj")
        promptFile = "automation/ai/prompts/TEST-1.md"
        checks = @([ordered]@{ name = "build"; command = "dotnet"; arguments = @("build") })
    }

    $duplicateQueuePath = Join-Path $testRoot "duplicate.json"
    Write-TestJson ([ordered]@{
        version = "1.0.0"
        project = "EnglishMaster"
        tasks = @($baseTask, $baseTask)
    }) $duplicateQueuePath
    Assert-Throws {
        & $orchestrator -TaskFile $duplicateQueuePath -ProviderFile $providerFile -ValidateOnly
    } "Duplicate task id"

    $missingDependencyTask = Copy-TaskContract $baseTask
    $missingDependencyTask.id = "TEST-MISSING"
    $missingDependencyTask.dependencies = @("NOT-THERE")
    $missingDependencyPath = Join-Path $testRoot "missing-dependency.json"
    Write-TestJson ([ordered]@{
        version = "1.0.0"
        project = "EnglishMaster"
        tasks = @($missingDependencyTask)
    }) $missingDependencyPath
    Assert-Throws {
        & $orchestrator -TaskFile $missingDependencyPath -ProviderFile $providerFile -ValidateOnly
    } "missing dependency"

    $cycleA = Copy-TaskContract $baseTask
    $cycleA.id = "CYCLE-A"
    $cycleA.dependencies = @("CYCLE-B")
    $cycleB = Copy-TaskContract $baseTask
    $cycleB.id = "CYCLE-B"
    $cycleB.dependencies = @("CYCLE-A")
    $cyclePath = Join-Path $testRoot "cycle.json"
    Write-TestJson ([ordered]@{
        version = "1.0.0"
        project = "EnglishMaster"
        tasks = @($cycleA, $cycleB)
    }) $cyclePath
    Assert-Throws {
        & $orchestrator -TaskFile $cyclePath -ProviderFile $providerFile -ValidateOnly
    } "Dependency cycle"

    $dryRunTask = Copy-TaskContract $baseTask
    $dryRunTask.id = "DRY-RUN"
    $dryRunTask.branch = (& git -c "core.excludesFile=$GitExcludesFile" branch --show-current 2>&1 | Out-String).Trim()
    $dryRunPath = Join-Path $testRoot "dry-run.json"
    Write-TestJson ([ordered]@{
        version = "1.0.0"
        project = "EnglishMaster"
        tasks = @($dryRunTask)
    }) $dryRunPath

    $before = @(Get-ChildItem -Force -Recurse $testRoot | ForEach-Object FullName)
    & $orchestrator `
        -TaskFile $dryRunPath `
        -ProviderFile $providerFile `
        -RuntimeDirectory (Join-Path $testRoot "runtime") `
        -TaskId "DRY-RUN" `
        -DryRun
    $after = @(Get-ChildItem -Force -Recurse $testRoot | ForEach-Object FullName)
    if (Compare-Object $before $after) {
        throw "Dry-run changed the filesystem."
    }

    $fakeRepository = Join-Path $testRoot "fake-repository"
    [System.IO.Directory]::CreateDirectory((Join-Path $fakeRepository "allowed")) | Out-Null
    [System.IO.Directory]::CreateDirectory((Join-Path $fakeRepository "forbidden")) | Out-Null
    [System.IO.File]::Copy((Join-Path $repositoryRoot "AI_RULES.md"), (Join-Path $fakeRepository "AI_RULES.md"))
    [System.IO.File]::WriteAllText(
        (Join-Path $fakeRepository ".gitignore"),
        "runtime/`r`n",
        [System.Text.UTF8Encoding]::new($false))
    [System.IO.File]::WriteAllText(
        (Join-Path $fakeRepository "prompt.md"),
        "Perform the bounded fake-provider test without changing files.",
        [System.Text.UTF8Encoding]::new($false))
    [System.IO.File]::WriteAllText(
        (Join-Path $fakeRepository "allowed/seed.txt"),
        "seed",
        [System.Text.UTF8Encoding]::new($false))
    [System.IO.File]::WriteAllText(
        (Join-Path $fakeRepository "forbidden/shared.txt"),
        "shared",
        [System.Text.UTF8Encoding]::new($false))

    & git -c "core.excludesFile=$GitExcludesFile" -C $fakeRepository init --quiet
    & git -c "core.excludesFile=$GitExcludesFile" -C $fakeRepository config user.name "EnglishMaster Orchestrator Test"
    & git -c "core.excludesFile=$GitExcludesFile" -C $fakeRepository config user.email "orchestrator-test@englishmaster.invalid"
    & git -c "core.excludesFile=$GitExcludesFile" -C $fakeRepository add .
    & git -c "core.excludesFile=$GitExcludesFile" -C $fakeRepository commit --quiet -m "Create fake orchestrator repository"
    if ($LASTEXITCODE -ne 0) {
        throw "Unable to initialize the fake Git repository."
    }
    $fakeBranch = (& git -c "core.excludesFile=$GitExcludesFile" -C $fakeRepository branch --show-current 2>&1 | Out-String).Trim()
    [System.IO.Directory]::CreateDirectory((Join-Path $fakeRepository "runtime/input")) | Out-Null

    $happyQueuePath = Join-Path $fakeRepository "runtime/input/happy-queue.json"
    $happyProviderPath = Join-Path $fakeRepository "runtime/input/happy-providers.json"
    Write-TestJson ([ordered]@{
        version = "1.0.0"
        project = "Fake"
        tasks = @(New-RunnableTask "HAPPY" $fakeBranch)
    }) $happyQueuePath
    Write-TestJson (New-TestProviderConfiguration) $happyProviderPath

    Push-Location $fakeRepository
    try {
        & $orchestrator `
            -TaskFile $happyQueuePath `
            -ProviderFile $happyProviderPath `
            -RuntimeDirectory "runtime/happy" `
            -TaskId "HAPPY" `
            -ProviderTimeoutSeconds 30
    }
    finally {
        Pop-Location
    }

    $happyState = Get-Content -Raw -LiteralPath (Join-Path $fakeRepository "runtime/happy/state.json") | ConvertFrom-Json
    if ($happyState.tasks.HAPPY.status -ne "ready_to_merge" -or
        $happyState.tasks.HAPPY.provider -ne "fake-success") {
        throw "Happy-path runtime state was not ready_to_merge with fake-success."
    }
    if (-not (Test-Path -LiteralPath (Join-Path $fakeRepository "runtime/happy/checkpoints/HAPPY.json"))) {
        throw "Happy-path checkpoint was not created."
    }

    $preferredQueuePath = Join-Path $fakeRepository "runtime/input/preferred-queue.json"
    $preferredProviderPath = Join-Path $fakeRepository "runtime/input/preferred-providers.json"
    $preferredTask = New-RunnableTask "PREFERRED" $fakeBranch
    $preferredTask.provider = "fake-success"
    Write-TestJson ([ordered]@{
        version = "1.0.0"
        project = "Fake"
        tasks = @($preferredTask)
    }) $preferredQueuePath
    Write-TestJson (New-TestProviderConfiguration -IncludeLimitProvider) $preferredProviderPath

    Push-Location $fakeRepository
    try {
        & $orchestrator `
            -TaskFile $preferredQueuePath `
            -ProviderFile $preferredProviderPath `
            -RuntimeDirectory "runtime/preferred" `
            -TaskId "PREFERRED" `
            -ProviderTimeoutSeconds 30
    }
    finally {
        Pop-Location
    }

    $preferredState = Get-Content -Raw -LiteralPath (Join-Path $fakeRepository "runtime/preferred/state.json") | ConvertFrom-Json
    $limitedLogs = @(Get-ChildItem -Path (Join-Path $fakeRepository "runtime/preferred/logs/PREFERRED") -Filter "*-fake-limit.log" -ErrorAction SilentlyContinue)
    if ($preferredState.tasks.PREFERRED.status -ne "ready_to_merge" -or
        $preferredState.tasks.PREFERRED.provider -ne "fake-success" -or
        $limitedLogs.Count -ne 0) {
        throw "Task-level preferred provider was not attempted first."
    }

    $failoverQueuePath = Join-Path $fakeRepository "runtime/input/failover-queue.json"
    $failoverProviderPath = Join-Path $fakeRepository "runtime/input/failover-providers.json"
    Write-TestJson ([ordered]@{
        version = "1.0.0"
        project = "Fake"
        tasks = @(New-RunnableTask "FAILOVER" $fakeBranch)
    }) $failoverQueuePath
    Write-TestJson (New-TestProviderConfiguration -IncludeLimitProvider) $failoverProviderPath

    Push-Location $fakeRepository
    try {
        & $orchestrator `
            -TaskFile $failoverQueuePath `
            -ProviderFile $failoverProviderPath `
            -RuntimeDirectory "runtime/failover" `
            -TaskId "FAILOVER" `
            -ProviderTimeoutSeconds 30
    }
    finally {
        Pop-Location
    }

    $failoverState = Get-Content -Raw -LiteralPath (Join-Path $fakeRepository "runtime/failover/state.json") | ConvertFrom-Json
    if ($failoverState.tasks.FAILOVER.status -ne "ready_to_merge" -or
        $failoverState.tasks.FAILOVER.provider -ne "fake-success" -or
        $failoverState.tasks.FAILOVER.previousProvider -ne "fake-limit" -or
        $failoverState.providers.'fake-limit'.status -ne "rate_limited") {
        throw "Failover state did not preserve the limited provider and ownership transfer."
    }

    $resumeQueuePath = Join-Path $fakeRepository "runtime/input/resume-queue.json"
    $resumeLimitProviderPath = Join-Path $fakeRepository "runtime/input/resume-limit-providers.json"
    Write-TestJson ([ordered]@{
        version = "1.0.0"
        project = "Fake"
        tasks = @(New-RunnableTask "RESUME" $fakeBranch)
    }) $resumeQueuePath
    Write-TestJson (New-TestProviderConfiguration -LimitOnly) $resumeLimitProviderPath
    Assert-Throws {
        Push-Location $fakeRepository
        try {
            & $orchestrator `
                -TaskFile $resumeQueuePath `
                -ProviderFile $resumeLimitProviderPath `
                -RuntimeDirectory "runtime/resume" `
                -TaskId "RESUME" `
                -ProviderTimeoutSeconds 30
        }
        finally {
            Pop-Location
        }
    } "No configured provider"

    Push-Location $fakeRepository
    try {
        & $orchestrator `
            -TaskFile $resumeQueuePath `
            -ProviderFile $happyProviderPath `
            -RuntimeDirectory "runtime/resume" `
            -TaskId "RESUME" `
            -ProviderTimeoutSeconds 30
    }
    finally {
        Pop-Location
    }
    $resumeState = Get-Content -Raw -LiteralPath (Join-Path $fakeRepository "runtime/resume/state.json") | ConvertFrom-Json
    if ($resumeState.tasks.RESUME.status -ne "ready_to_merge" -or
        $resumeState.tasks.RESUME.previousProvider -ne "fake-limit" -or
        [int]$resumeState.tasks.RESUME.attempt -ne 2) {
        throw "Checkpoint resume did not preserve provider ownership and attempt count."
    }

    $lockQueuePath = Join-Path $fakeRepository "runtime/input/lock-queue.json"
    Write-TestJson ([ordered]@{
        version = "1.0.0"
        project = "Fake"
        tasks = @(New-RunnableTask "LOCKED" $fakeBranch)
    }) $lockQueuePath
    $heldLockPath = Join-Path $fakeRepository "runtime/locked/locks/task-LOCKED.lock"
    [System.IO.Directory]::CreateDirectory((Split-Path -Parent $heldLockPath)) | Out-Null
    [System.IO.File]::WriteAllText($heldLockPath, '{"processId":999999}', [System.Text.UTF8Encoding]::new($false))
    Assert-Throws {
        Push-Location $fakeRepository
        try {
            & $orchestrator `
                -TaskFile $lockQueuePath `
                -ProviderFile $happyProviderPath `
                -RuntimeDirectory "runtime/locked" `
                -TaskId "LOCKED" `
                -ProviderTimeoutSeconds 30
        }
        finally {
            Pop-Location
        }
    } "Lock is already held"
    if (-not (Test-Path -LiteralPath $heldLockPath)) {
        throw "The orchestrator removed a lock that it did not acquire."
    }

    Write-Host "AI failover orchestrator validation tests passed."
}
finally {
    if (Test-Path -LiteralPath $testRoot) {
        Remove-Item -LiteralPath $testRoot -Recurse -Force
    }
}
