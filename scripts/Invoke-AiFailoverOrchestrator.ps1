[CmdletBinding()]
param(
    [string]$TaskFile = "automation/ai/tasks.json",
    [string]$ProviderFile = "automation/ai/providers.json",
    [string]$TaskId,
    [string]$RuntimeDirectory = ".ai-orchestrator",
    [switch]$ValidateOnly,
    [switch]$DryRun,
    [switch]$AllowPaidProviders,
    [ValidateRange(30, 86400)]
    [int]$ProviderTimeoutSeconds = 7200
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$AllowedTaskStatuses = @(
    "queued",
    "in_progress",
    "checkpointed",
    "testing",
    "ready_to_merge",
    "blocked",
    "done"
)
$GitExcludesFile = ""

function Get-FullPath {
    param(
        [Parameter(Mandatory)]
        [string]$Path,
        [Parameter(Mandatory)]
        [string]$BasePath
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $BasePath $Path))
}

function Read-JsonFile {
    param([Parameter(Mandatory)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "Required JSON file was not found: $Path"
    }

    try {
        return Get-Content -Raw -LiteralPath $Path | ConvertFrom-Json
    }
    catch {
        throw "Invalid JSON in ${Path}: $($_.Exception.Message)"
    }
}

function Test-Property {
    param(
        [Parameter(Mandatory)]$Object,
        [Parameter(Mandatory)][string]$Name
    )

    if ($null -eq $Object) {
        return $false
    }
    return @($Object.PSObject.Properties | ForEach-Object { $_.Name }) -contains $Name
}

function Assert-RequiredString {
    param(
        [Parameter(Mandatory)]$Object,
        [Parameter(Mandatory)][string]$Name,
        [Parameter(Mandatory)][string]$Context
    )

    if (-not (Test-Property $Object $Name) -or
        [string]::IsNullOrWhiteSpace([string]$Object.$Name)) {
        throw "$Context requires a non-empty '$Name'."
    }
}

function Get-NormalizedScope {
    param([Parameter(Mandatory)][string]$Path)

    $normalized = $Path.Replace("\", "/").Trim().TrimStart("./").ToLowerInvariant()
    if ($normalized.Contains("../") -or $normalized.StartsWith("../") -or
        [System.IO.Path]::IsPathRooted($Path)) {
        throw "Task paths must be repository-relative and cannot traverse: $Path"
    }

    $wildcardIndex = $normalized.IndexOfAny([char[]]@("*", "?", "["))
    if ($wildcardIndex -ge 0) {
        $normalized = $normalized.Substring(0, $wildcardIndex)
    }

    return $normalized.TrimEnd("/")
}

function Test-ScopeOverlap {
    param(
        [Parameter(Mandatory)][string[]]$Left,
        [Parameter(Mandatory)][string[]]$Right
    )

    foreach ($leftPath in $Left) {
        $leftScope = Get-NormalizedScope $leftPath
        foreach ($rightPath in $Right) {
            $rightScope = Get-NormalizedScope $rightPath
            if ($leftScope -eq $rightScope -or
                $leftScope.StartsWith("$rightScope/") -or
                $rightScope.StartsWith("$leftScope/")) {
                return $true
            }
        }
    }

    return $false
}

function Assert-NoDependencyCycle {
    param([Parameter(Mandatory)]$TasksById)

    $visitState = @{}

    function Visit-Task {
        param([Parameter(Mandatory)][string]$Id)

        if ($visitState[$Id] -eq "visiting") {
            throw "Dependency cycle detected at task '$Id'."
        }

        if ($visitState[$Id] -eq "visited") {
            return
        }

        $visitState[$Id] = "visiting"
        $task = $TasksById[$Id]
        foreach ($dependency in @($task.dependencies)) {
            Visit-Task ([string]$dependency)
        }
        $visitState[$Id] = "visited"
    }

    foreach ($id in $TasksById.Keys) {
        Visit-Task ([string]$id)
    }
}

function Assert-Configuration {
    param(
        [Parameter(Mandatory)]$Queue,
        [Parameter(Mandatory)]$ProviderConfiguration
    )

    if ([string]$Queue.version -ne "1.0.0") {
        throw "Unsupported task queue version '$($Queue.version)'. Expected 1.0.0."
    }

    if ([string]$ProviderConfiguration.version -ne "1.0.0") {
        throw "Unsupported provider configuration version '$($ProviderConfiguration.version)'. Expected 1.0.0."
    }

    if (-not (Test-Property $Queue "tasks")) {
        throw "Task queue requires a tasks array."
    }

    if (-not (Test-Property $ProviderConfiguration "providers")) {
        throw "Provider configuration requires a providers array."
    }

    $providerNames = @{}
    foreach ($provider in @($ProviderConfiguration.providers)) {
        Assert-RequiredString $provider "name" "Provider"
        $providerName = ([string]$provider.name).ToLowerInvariant()
        if ($providerNames.ContainsKey($providerName)) {
            throw "Duplicate provider name '$providerName'."
        }
        $providerNames[$providerName] = $true

        if (-not (Test-Property $provider "enabled") -or
            -not (Test-Property $provider "priority") -or
            -not (Test-Property $provider "paid")) {
            throw "Provider '$providerName' requires enabled, priority, and paid fields."
        }

        if ([bool]$provider.enabled) {
            Assert-RequiredString $provider "command" "Provider '$providerName'"
        }

        if ([bool]$provider.enabled -and [bool]$provider.paid -and
            (-not (Test-Property $provider "budgetUsd") -or [decimal]$provider.budgetUsd -le 0)) {
            throw "Enabled paid provider '$providerName' requires a positive budgetUsd."
        }
    }

    $tasksById = @{}
    foreach ($task in @($Queue.tasks)) {
        Assert-RequiredString $task "id" "Task"
        Assert-RequiredString $task "feature" "Task '$($task.id)'"
        Assert-RequiredString $task "status" "Task '$($task.id)'"
        Assert-RequiredString $task "branch" "Task '$($task.id)'"
        Assert-RequiredString $task "worktree" "Task '$($task.id)'"
        Assert-RequiredString $task "promptFile" "Task '$($task.id)'"

        $id = [string]$task.id
        if ($tasksById.ContainsKey($id)) {
            throw "Duplicate task id '$id'."
        }

        if ($AllowedTaskStatuses -notcontains [string]$task.status) {
            throw "Task '$id' has unsupported status '$($task.status)'."
        }

        if (Test-Property $task "provider") {
            $preferredProvider = [string]$task.provider
            if (-not [string]::IsNullOrWhiteSpace($preferredProvider) -and
                -not $providerNames.ContainsKey($preferredProvider.ToLowerInvariant())) {
                throw "Task '$id' references unknown preferred provider '$preferredProvider'."
            }
        }

        foreach ($field in @("dependencies", "allowedPaths", "forbiddenPaths", "checks")) {
            if (-not (Test-Property $task $field)) {
                throw "Task '$id' requires a '$field' array."
            }
        }

        if (@($task.allowedPaths).Count -eq 0) {
            throw "Task '$id' requires at least one allowed path."
        }

        foreach ($path in @($task.allowedPaths) + @($task.forbiddenPaths)) {
            [void](Get-NormalizedScope ([string]$path))
        }

        foreach ($check in @($task.checks)) {
            Assert-RequiredString $check "name" "Check in task '$id'"
            Assert-RequiredString $check "command" "Check '$($check.name)' in task '$id'"
            if (-not (Test-Property $check "arguments")) {
                throw "Check '$($check.name)' in task '$id' requires an arguments array."
            }
        }

        $tasksById[$id] = $task
    }

    foreach ($task in @($Queue.tasks)) {
        foreach ($dependency in @($task.dependencies)) {
            if (-not $tasksById.ContainsKey([string]$dependency)) {
                throw "Task '$($task.id)' references missing dependency '$dependency'."
            }
        }
    }

    Assert-NoDependencyCycle $tasksById
}

function New-RuntimeState {
    return [ordered]@{
        version = "1.0.0"
        revision = 0
        providers = [ordered]@{}
        tasks = [ordered]@{}
    }
}

function Read-RuntimeState {
    param([Parameter(Mandatory)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return New-RuntimeState
    }

    $state = Read-JsonFile $Path
    if ([string]$state.version -ne "1.0.0") {
        throw "Unsupported runtime state version '$($state.version)'."
    }

    if (Test-Property $state "tasks") {
        foreach ($property in $state.tasks.PSObject.Properties) {
            if ($AllowedTaskStatuses -notcontains [string]$property.Value.status) {
                throw "Runtime task '$($property.Name)' has unsupported status '$($property.Value.status)'."
            }
        }
    }

    return $state
}

function Write-JsonAtomic {
    param(
        [Parameter(Mandatory)]$Value,
        [Parameter(Mandatory)][string]$Path
    )

    $directory = Split-Path -Parent $Path
    [System.IO.Directory]::CreateDirectory($directory) | Out-Null
    $temporaryPath = Join-Path $directory (".{0}.{1}.tmp" -f
        [System.IO.Path]::GetFileName($Path), [guid]::NewGuid().ToString("N"))
    $json = $Value | ConvertTo-Json -Depth 30
    [System.IO.File]::WriteAllText($temporaryPath, $json, [System.Text.UTF8Encoding]::new($false))

    if (Test-Path -LiteralPath $Path) {
        $backupPath = Join-Path $directory (".{0}.{1}.bak" -f
            [System.IO.Path]::GetFileName($Path), [guid]::NewGuid().ToString("N"))
        [System.IO.File]::Replace($temporaryPath, $Path, $backupPath)
        [System.IO.File]::Delete($backupPath)
    }
    else {
        [System.IO.File]::Move($temporaryPath, $Path)
    }
}

function Open-ExclusiveLock {
    param(
        [Parameter(Mandatory)][string]$Path,
        [int]$WaitSeconds = 0
    )

    [System.IO.Directory]::CreateDirectory((Split-Path -Parent $Path)) | Out-Null
    $deadline = [DateTimeOffset]::UtcNow.AddSeconds($WaitSeconds)

    do {
        try {
            $stream = [System.IO.File]::Open(
                $Path,
                [System.IO.FileMode]::CreateNew,
                [System.IO.FileAccess]::ReadWrite,
                [System.IO.FileShare]::None)
            $payload = [System.Text.Encoding]::UTF8.GetBytes((@{
                processId = $PID
                machine = [Environment]::MachineName
                acquiredAt = [DateTimeOffset]::UtcNow.ToString("o")
            } | ConvertTo-Json -Compress))
            $stream.Write($payload, 0, $payload.Length)
            $stream.Flush($true)
            return $stream
        }
        catch [System.IO.IOException] {
            if ([DateTimeOffset]::UtcNow -ge $deadline) {
                throw "Lock is already held: $Path. Inspect the recorded process before manually removing a stale lock."
            }
            Start-Sleep -Milliseconds 200
        }
    } while ($true)
}

function Close-ExclusiveLock {
    param(
        $Stream,
        [Parameter(Mandatory)][string]$Path
    )

    if ($null -eq $Stream) {
        return
    }

    $Stream.Dispose()
    if (Test-Path -LiteralPath $Path) {
        Remove-Item -LiteralPath $Path -Force
    }
}

function Get-OrAddNoteProperty {
    param(
        [Parameter(Mandatory)]$Object,
        [Parameter(Mandatory)][string]$Name,
        [Parameter(Mandatory)]$DefaultValue
    )

    if (-not (Test-Property $Object $Name)) {
        $Object | Add-Member -NotePropertyName $Name -NotePropertyValue $DefaultValue
    }
    return $Object.$Name
}

function Get-EffectiveTaskStatus {
    param(
        [Parameter(Mandatory)]$Task,
        [Parameter(Mandatory)]$State
    )

    $runtimeTasks = Get-OrAddNoteProperty $State "tasks" ([pscustomobject]@{})
    if (Test-Property $runtimeTasks ([string]$Task.id)) {
        return [string]$runtimeTasks.([string]$Task.id).status
    }
    return [string]$Task.status
}

function Set-RuntimeTaskState {
    param(
        [Parameter(Mandatory)]$State,
        [Parameter(Mandatory)]$Task,
        [Parameter(Mandatory)][string]$Status,
        [string]$Provider,
        [string]$PreviousProvider,
        [int]$Attempt,
        [string]$Message
    )

    $runtimeTasks = Get-OrAddNoteProperty $State "tasks" ([pscustomobject]@{})
    $record = [pscustomobject][ordered]@{
        id = [string]$Task.id
        status = $Status
        provider = $Provider
        previousProvider = $PreviousProvider
        attempt = $Attempt
        branch = [string]$Task.branch
        processId = $PID
        machine = [Environment]::MachineName
        updatedAt = [DateTimeOffset]::UtcNow.ToString("o")
        message = $Message
    }

    if (Test-Property $runtimeTasks ([string]$Task.id)) {
        $runtimeTasks.([string]$Task.id) = $record
    }
    else {
        $runtimeTasks | Add-Member -NotePropertyName ([string]$Task.id) -NotePropertyValue $record
    }
}

function Set-ProviderState {
    param(
        [Parameter(Mandatory)]$State,
        [Parameter(Mandatory)][string]$Provider,
        [Parameter(Mandatory)][string]$Status,
        [AllowNull()][Nullable[DateTimeOffset]]$BlockedUntil,
        [string]$Message
    )

    $providers = Get-OrAddNoteProperty $State "providers" ([pscustomobject]@{})
    $record = [pscustomobject][ordered]@{
        status = $Status
        blockedUntil = if ($null -eq $BlockedUntil) { $null } else { ([DateTimeOffset]$BlockedUntil).ToString("o") }
        updatedAt = [DateTimeOffset]::UtcNow.ToString("o")
        message = $Message
    }

    if (Test-Property $providers $Provider) {
        $providers.$Provider = $record
    }
    else {
        $providers | Add-Member -NotePropertyName $Provider -NotePropertyValue $record
    }
}

function Save-RuntimeState {
    param(
        [Parameter(Mandatory)]$State,
        [Parameter(Mandatory)][string]$StatePath
    )

    $State.revision = [int]$State.revision + 1
    Write-JsonAtomic $State $StatePath
}

function Invoke-WithCoordinationLock {
    param(
        [Parameter(Mandatory)][string]$LockPath,
        [Parameter(Mandatory)][scriptblock]$Action
    )

    $lock = $null
    try {
        $lock = Open-ExclusiveLock $LockPath 10
        & $Action
    }
    finally {
        Close-ExclusiveLock $lock $LockPath
    }
}

function Test-DependenciesComplete {
    param(
        [Parameter(Mandatory)]$Task,
        [Parameter(Mandatory)]$Queue,
        [Parameter(Mandatory)]$State
    )

    foreach ($dependencyId in @($Task.dependencies)) {
        $dependency = @($Queue.tasks | Where-Object { [string]$_.id -eq [string]$dependencyId })[0]
        if ((Get-EffectiveTaskStatus $dependency $State) -ne "done") {
            return $false
        }
    }
    return $true
}

function Test-ConflictsWithActiveTask {
    param(
        [Parameter(Mandatory)]$Task,
        [Parameter(Mandatory)]$Queue,
        [Parameter(Mandatory)]$State
    )

    foreach ($otherTask in @($Queue.tasks)) {
        if ([string]$otherTask.id -eq [string]$Task.id) {
            continue
        }
        $otherStatus = Get-EffectiveTaskStatus $otherTask $State
        if ($otherStatus -in @("in_progress", "testing") -and
            (Test-ScopeOverlap @($Task.allowedPaths) @($otherTask.allowedPaths))) {
            return [string]$otherTask.id
        }
    }
    return $null
}

function Resolve-Worktree {
    param(
        [Parameter(Mandatory)]$Task,
        [Parameter(Mandatory)][string]$RepositoryRoot
    )

    $worktree = Get-FullPath ([string]$Task.worktree) $RepositoryRoot
    if (-not (Test-Path -LiteralPath $worktree -PathType Container)) {
        throw "Task '$($Task.id)' worktree does not exist: $worktree"
    }

    $actualRoot = (& git -c "core.excludesFile=$GitExcludesFile" -C $worktree rev-parse --show-toplevel 2>&1 | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($actualRoot)) {
        throw "Task '$($Task.id)' path is not a Git worktree: $worktree"
    }

    $actualBranch = (& git -c "core.excludesFile=$GitExcludesFile" -C $worktree branch --show-current 2>&1 | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or $actualBranch -ne [string]$Task.branch) {
        throw "Task '$($Task.id)' requires branch '$($Task.branch)' but worktree is on '$actualBranch'."
    }

    return [System.IO.Path]::GetFullPath($actualRoot)
}

function Get-ChangedFiles {
    param(
        [Parameter(Mandatory)][string]$Worktree,
        [string]$BaseHead
    )

    $lines = @(& git -c "core.excludesFile=$GitExcludesFile" -C $Worktree status --porcelain=v1 --untracked-files=all 2>&1)
    if ($LASTEXITCODE -ne 0) {
        throw "Unable to inspect Git changes in $Worktree."
    }

    $paths = [System.Collections.Generic.List[string]]::new()
    foreach ($line in $lines) {
        if ([string]::IsNullOrWhiteSpace([string]$line) -or $line.Length -lt 4) {
            continue
        }
        $path = $line.Substring(3).Trim()
        if ($path.Contains(" -> ")) {
            $path = $path.Split(@(" -> "), [System.StringSplitOptions]::None)[-1]
        }
        $paths.Add($path.Replace("\", "/").Trim('"'))
    }

    if (-not [string]::IsNullOrWhiteSpace($BaseHead)) {
        $committedPaths = @(& git -c "core.excludesFile=$GitExcludesFile" -C $Worktree diff --name-only --diff-filter=ACDMRTUXB "$BaseHead..HEAD" 2>&1)
        if ($LASTEXITCODE -ne 0) {
            throw "Unable to inspect committed changes since $BaseHead in $Worktree."
        }
        foreach ($path in $committedPaths) {
            if (-not [string]::IsNullOrWhiteSpace([string]$path)) {
                $paths.Add(([string]$path).Replace("\", "/"))
            }
        }
    }
    return @($paths | Sort-Object -Unique)
}

function Assert-NoPathEscape {
    param(
        [Parameter(Mandatory)][string]$Worktree,
        [Parameter(Mandatory)][string]$RelativePath
    )

    $root = [System.IO.Path]::GetFullPath($Worktree).TrimEnd("\", "/")
    $candidate = [System.IO.Path]::GetFullPath((Join-Path $root $RelativePath))
    if (-not $candidate.StartsWith("$root$([System.IO.Path]::DirectorySeparatorChar)", [StringComparison]::OrdinalIgnoreCase)) {
        throw "Changed path escapes the task worktree: $RelativePath"
    }

    $current = $root
    foreach ($segment in $RelativePath.Replace("/", "\").Split("\")) {
        if ([string]::IsNullOrWhiteSpace($segment)) {
            continue
        }
        $current = Join-Path $current $segment
        if (Test-Path -LiteralPath $current) {
            $item = Get-Item -Force -LiteralPath $current
            if (($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0) {
                throw "Changed path traverses a reparse point and requires integrator review: $RelativePath"
            }
        }
    }
}

function Test-PathMatchesContract {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)]$Task
    )

    $normalized = $Path.Replace("\", "/")
    foreach ($forbidden in @($Task.forbiddenPaths)) {
        if ($normalized -like ([string]$forbidden).Replace("\", "/")) {
            return $false
        }
    }
    foreach ($allowed in @($Task.allowedPaths)) {
        if ($normalized -like ([string]$allowed).Replace("\", "/")) {
            return $true
        }
    }
    return $false
}

function Assert-ChangedFilesAllowed {
    param(
        [Parameter(Mandatory)][AllowEmptyCollection()][string[]]$ChangedFiles,
        [Parameter(Mandatory)]$Task,
        [Parameter(Mandatory)][string]$Worktree
    )

    foreach ($changedFile in $ChangedFiles) {
        Assert-NoPathEscape $Worktree $changedFile
    }

    $unauthorized = @($ChangedFiles | Where-Object {
        -not (Test-PathMatchesContract $_ $Task)
    })
    if ($unauthorized.Count -gt 0) {
        throw "Task '$($Task.id)' changed unauthorized paths: $($unauthorized -join ', ')"
    }
}

function Get-AvailableProviderCommand {
    param(
        [Parameter(Mandatory)]$Provider,
        [Parameter(Mandatory)][string]$RepositoryRoot
    )

    $candidates = @([string]$Provider.command)
    if (Test-Property $Provider "fallbackCommands") {
        $candidates += @($Provider.fallbackCommands | ForEach-Object { [string]$_ })
    }

    foreach ($candidate in $candidates) {
        if ([string]::IsNullOrWhiteSpace($candidate)) {
            continue
        }

        $source = $null
        if ([System.IO.Path]::IsPathRooted($candidate) -or
            $candidate.Contains("/") -or $candidate.Contains("\")) {
            $candidatePath = Get-FullPath $candidate $RepositoryRoot
            if (Test-Path -LiteralPath $candidatePath -PathType Leaf) {
                $source = $candidatePath
            }
        }
        else {
            $command = Get-Command $candidate -CommandType Application -ErrorAction SilentlyContinue
            if ($null -ne $command) {
                $source = $command.Source
            }
        }

        if ($null -eq $source) {
            continue
        }

        try {
            if (@($Provider.versionArguments).Count -gt 0) {
                $versionOutput = & $source @($Provider.versionArguments) 2>&1
                if ($LASTEXITCODE -ne 0) {
                    Write-Warning "Provider '$($Provider.name)' candidate '$source' failed its version probe: $versionOutput"
                    continue
                }
            }
            return $source
        }
        catch {
            Write-Warning "Provider '$($Provider.name)' candidate '$source' cannot run: $($_.Exception.Message)"
        }
    }

    return $null
}

function Protect-LogText {
    param([AllowEmptyString()][string]$Text)

    if ([string]::IsNullOrEmpty($Text)) {
        return $Text
    }
    $redacted = $Text -replace '(?i)(api[_-]?key|authorization|token|password)\s*[:=]\s*\S+', '$1=[REDACTED]'
    return $redacted -replace '(?i)bearer\s+[a-z0-9._~+/-]+=*', 'Bearer [REDACTED]'
}

function ConvertTo-NativeArgumentString {
    param([Parameter(Mandatory)][string[]]$Arguments)

    $escapedArguments = foreach ($argument in $Arguments) {
        if ($argument -notmatch '[\s"]') {
            $argument
            continue
        }

        # Windows CommandLineToArgvW-compatible quoting: double backslashes that
        # precede a quote and double trailing backslashes before the closing quote.
        $escaped = $argument -replace '(\\*)"', '$1$1\"'
        $escaped = $escaped -replace '(\\+)$', '$1$1'
        '"' + $escaped + '"'
    }
    return $escapedArguments -join ' '
}

function Stop-ProviderProcessTree {
    param([Parameter(Mandatory)][System.Diagnostics.Process]$Process)

    try {
        $killTreeMethod = $Process.GetType().GetMethod("Kill", [type[]]@([bool]))
        if ($null -ne $killTreeMethod) {
            [void]$killTreeMethod.Invoke($Process, @($true))
            return
        }

        if ($env:OS -eq "Windows_NT") {
            & taskkill.exe /PID $Process.Id /T /F 2>&1 | Out-Null
            return
        }

        $Process.Kill()
    }
    catch {
        Write-Warning "Unable to terminate the complete provider process tree: $($_.Exception.Message)"
        try { $Process.Kill() } catch { Write-Warning $_.Exception.Message }
    }
}

function Invoke-ProviderProcess {
    param(
        [Parameter(Mandatory)][string]$Command,
        [Parameter(Mandatory)][string[]]$Arguments,
        [Parameter(Mandatory)][string]$InputText,
        [Parameter(Mandatory)][string]$WorkingDirectory,
        [Parameter(Mandatory)][int]$TimeoutSeconds
    )

    $startInfo = [System.Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = $Command
    $startInfo.WorkingDirectory = $WorkingDirectory
    $startInfo.UseShellExecute = $false
    $startInfo.RedirectStandardInput = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.CreateNoWindow = $true
    $startInfo.Arguments = ConvertTo-NativeArgumentString $Arguments

    $process = [System.Diagnostics.Process]::new()
    $process.StartInfo = $startInfo
    if (-not $process.Start()) {
        throw "Provider process could not be started: $Command"
    }

    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    $process.StandardInput.Write($InputText)
    $process.StandardInput.Close()

    $timedOut = -not $process.WaitForExit($TimeoutSeconds * 1000)
    if ($timedOut) {
        Stop-ProviderProcessTree $process
        $process.WaitForExit()
    }

    $stdout = $stdoutTask.GetAwaiter().GetResult()
    $stderr = $stderrTask.GetAwaiter().GetResult()
    $exitCode = if ($timedOut) { 124 } else { $process.ExitCode }
    $process.Dispose()

    return [pscustomobject]@{
        ExitCode = $exitCode
        TimedOut = $timedOut
        Output = (($stdout, $stderr) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }) -join "`n"
    }
}

function Test-IsRateLimit {
    param(
        [AllowEmptyString()][string]$Output,
        [Parameter(Mandatory)]$ProviderConfiguration
    )

    foreach ($pattern in @($ProviderConfiguration.rateLimitPatterns)) {
        if (-not [string]::IsNullOrWhiteSpace([string]$pattern) -and
            $Output.IndexOf([string]$pattern, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
            return $true
        }
    }
    return $false
}

function Invoke-TaskChecks {
    param(
        [Parameter(Mandatory)]$Task,
        [Parameter(Mandatory)][string]$Worktree
    )

    $results = [System.Collections.Generic.List[object]]::new()
    foreach ($check in @($Task.checks)) {
        $started = [DateTimeOffset]::UtcNow
        $output = & ([string]$check.command) @($check.arguments) 2>&1
        $exitCode = $LASTEXITCODE
        $results.Add([pscustomobject]@{
            name = [string]$check.name
            exitCode = $exitCode
            startedAt = $started.ToString("o")
            finishedAt = [DateTimeOffset]::UtcNow.ToString("o")
            output = Protect-LogText (($output | Out-String).Trim())
        })
        if ($exitCode -ne 0) {
            throw "Check '$($check.name)' failed with exit code $exitCode."
        }
    }
    return @($results)
}

function Write-Checkpoint {
    param(
        [Parameter(Mandatory)][string]$CheckpointDirectory,
        [Parameter(Mandatory)]$Task,
        [Parameter(Mandatory)][string]$Status,
        [string]$Provider,
        [string]$PreviousProvider,
        [Parameter(Mandatory)][string]$Worktree,
        [Parameter(Mandatory)][string]$BaseHead,
        [Parameter(Mandatory)][AllowEmptyCollection()][string[]]$ChangedFiles,
        [object[]]$Checks = @(),
        [Parameter(Mandatory)][string]$NextAction,
        [string]$Message
    )

    $head = (& git -c "core.excludesFile=$GitExcludesFile" -C $Worktree rev-parse HEAD 2>&1 | Out-String).Trim()
    $checkpoint = [ordered]@{
        taskId = [string]$Task.id
        feature = [string]$Task.feature
        status = $Status
        currentProvider = $Provider
        previousProvider = $PreviousProvider
        branch = [string]$Task.branch
        baseHead = $BaseHead
        head = $head
        completed = if ($Status -eq "ready_to_merge") { @("Provider execution", "Allowed-path validation", "Task checks") } else { @() }
        remaining = if ($Status -eq "ready_to_merge") { @("Integrator review", "Sequential merge", "Full test suite") } else { @("Resolve blocker and resume from this checkpoint") }
        changedFiles = @($ChangedFiles)
        checks = @($Checks)
        nextAction = $NextAction
        message = $Message
        timestamp = [DateTimeOffset]::UtcNow.ToString("o")
    }

    [System.IO.Directory]::CreateDirectory($CheckpointDirectory) | Out-Null
    Write-JsonAtomic $checkpoint (Join-Path $CheckpointDirectory "$($Task.id).json")
}

$repositoryRootOutput = (& git -c "core.excludesFile=$GitExcludesFile" rev-parse --show-toplevel 2>&1 | Out-String).Trim()
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($repositoryRootOutput)) {
    throw "Run the orchestrator from an EnglishMaster Git worktree."
}
$repositoryRoot = [System.IO.Path]::GetFullPath($repositoryRootOutput)
$taskPath = Get-FullPath $TaskFile $repositoryRoot
$providerPath = Get-FullPath $ProviderFile $repositoryRoot
$runtimePath = Get-FullPath $RuntimeDirectory $repositoryRoot

$queue = Read-JsonFile $taskPath
$providerConfiguration = Read-JsonFile $providerPath
Assert-Configuration $queue $providerConfiguration

if ($ValidateOnly) {
    Write-Host "Configuration is valid: $(@($queue.tasks).Count) task(s), $(@($providerConfiguration.providers).Count) provider(s)."
    return
}

$statePath = Join-Path $runtimePath "state.json"
$state = Read-RuntimeState $statePath
$selectedTask = $null

if (-not [string]::IsNullOrWhiteSpace($TaskId)) {
    $selectedTask = @($queue.tasks | Where-Object { [string]$_.id -eq $TaskId }) | Select-Object -First 1
    if ($null -eq $selectedTask) {
        throw "Task '$TaskId' was not found."
    }
}
else {
    $selectedTask = @($queue.tasks | Where-Object {
        (Get-EffectiveTaskStatus $_ $state) -in @("queued", "checkpointed") -and
        (Test-DependenciesComplete $_ $queue $state)
    }) | Select-Object -First 1
}

if ($null -eq $selectedTask) {
    Write-Host "No runnable task is available."
    return
}

$selectedStatus = Get-EffectiveTaskStatus $selectedTask $state
if ($selectedStatus -notin @("queued", "checkpointed")) {
    throw "Task '$($selectedTask.id)' cannot start from status '$selectedStatus'. Integrator action is required."
}

$providers = @($providerConfiguration.providers | Where-Object {
    [bool]$_.enabled -and (-not [bool]$_.paid -or $AllowPaidProviders)
} | Sort-Object priority)
if ($providers.Count -eq 0) {
    throw "No enabled provider is permitted for this invocation."
}

$preferredProviderName = if (Test-Property $selectedTask "provider") {
    [string]$selectedTask.provider
}
else {
    $null
}
if (-not [string]::IsNullOrWhiteSpace($preferredProviderName)) {
    $preferredProvider = @($providers | Where-Object {
        [string]$_.name -ieq $preferredProviderName
    }) | Select-Object -First 1
    if ($null -eq $preferredProvider) {
        throw "Preferred provider '$preferredProviderName' for task '$($selectedTask.id)' is not enabled or permitted."
    }
    $providers = @($preferredProvider) + @($providers | Where-Object {
        [string]$_.name -ine $preferredProviderName
    })
}

if ($DryRun) {
    Write-Host "[DryRun] Task: $($selectedTask.id) - $($selectedTask.feature)"
    Write-Host "[DryRun] Branch: $($selectedTask.branch)"
    Write-Host "[DryRun] Worktree: $($selectedTask.worktree)"
    Write-Host "[DryRun] Providers: $((@($providers.name)) -join ' -> ')"
    Write-Host "[DryRun] Checks: $((@($selectedTask.checks.name)) -join ', ')"
    Write-Host "[DryRun] No files, state, providers, builds, or tests were touched."
    return
}

$worktree = Resolve-Worktree $selectedTask $repositoryRoot
$initialStatus = Get-EffectiveTaskStatus $selectedTask $state
$initialChanges = @(Get-ChangedFiles $worktree)
if ($initialStatus -eq "queued" -and $initialChanges.Count -gt 0) {
    throw "Queued task '$($selectedTask.id)' requires a clean worktree. Existing changes: $($initialChanges -join ', ')"
}
$currentHead = (& git -c "core.excludesFile=$GitExcludesFile" -C $worktree rev-parse HEAD 2>&1 | Out-String).Trim()
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($currentHead)) {
    throw "Unable to record the baseline HEAD for task '$($selectedTask.id)'."
}

$coordinationLockPath = Join-Path $runtimePath "locks/coordination.lock"
$taskLockPath = Join-Path $runtimePath "locks/task-$($selectedTask.id).lock"
$checkpointDirectory = Join-Path $runtimePath "checkpoints"
$logDirectory = Join-Path $runtimePath "logs/$($selectedTask.id)"
$baseHead = $currentHead
if ($initialStatus -eq "checkpointed") {
    $resumeCheckpointPath = Join-Path $checkpointDirectory "$($selectedTask.id).json"
    $resumeCheckpoint = Read-JsonFile $resumeCheckpointPath
    Assert-RequiredString $resumeCheckpoint "baseHead" "Checkpoint for task '$($selectedTask.id)'"
    Assert-RequiredString $resumeCheckpoint "head" "Checkpoint for task '$($selectedTask.id)'"
    if ([string]$resumeCheckpoint.head -ne $currentHead) {
        throw "Task '$($selectedTask.id)' HEAD differs from its checkpoint. Integrator review is required before resume."
    }
    $baseHead = [string]$resumeCheckpoint.baseHead
    & git -c "core.excludesFile=$GitExcludesFile" -C $worktree cat-file -e "$baseHead^{commit}" 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Task '$($selectedTask.id)' checkpoint baseline '$baseHead' is not a valid commit."
    }
}
$taskLock = $null
$providerLock = $null
$providerLockPath = $null
$previousProvider = if (Test-Property $selectedTask "previousProvider") { [string]$selectedTask.previousProvider } else { $null }
$attempt = 0
if (Test-Property $state.tasks ([string]$selectedTask.id)) {
    $runtimeTaskRecord = $state.tasks.([string]$selectedTask.id)
    if (-not [string]::IsNullOrWhiteSpace([string]$runtimeTaskRecord.provider)) {
        $previousProvider = [string]$runtimeTaskRecord.provider
    }
    if (Test-Property $runtimeTaskRecord "attempt") {
        $attempt = [int]$runtimeTaskRecord.attempt
    }
}

try {
    $taskLock = Open-ExclusiveLock $taskLockPath

    Invoke-WithCoordinationLock $coordinationLockPath {
        $script:state = Read-RuntimeState $statePath
        if (-not (Test-DependenciesComplete $selectedTask $queue $script:state)) {
            throw "Task '$($selectedTask.id)' has an incomplete dependency."
        }
        $conflict = Test-ConflictsWithActiveTask $selectedTask $queue $script:state
        if ($null -ne $conflict) {
            throw "Task '$($selectedTask.id)' overlaps active task '$conflict'."
        }
    }

    $completed = $false
    foreach ($provider in $providers) {
        $providerName = [string]$provider.name
        $providerCommand = Get-AvailableProviderCommand $provider $repositoryRoot
        if ($null -eq $providerCommand) {
            Write-Warning "Skipping unavailable provider '$providerName'."
            continue
        }

        $state = Read-RuntimeState $statePath
        if (Test-Property $state.providers $providerName) {
            $blockedUntilText = [string]$state.providers.$providerName.blockedUntil
            if (-not [string]::IsNullOrWhiteSpace($blockedUntilText) -and
                [DateTimeOffset]::Parse($blockedUntilText) -gt [DateTimeOffset]::UtcNow) {
                Write-Warning "Skipping provider '$providerName' until $blockedUntilText."
                continue
            }
        }

        $providerLockPath = Join-Path $runtimePath "locks/provider-$providerName.lock"
        try {
            $providerLock = Open-ExclusiveLock $providerLockPath
        }
        catch {
            Write-Warning "Provider '$providerName' is already running another task."
            continue
        }

        $attempt++
        Invoke-WithCoordinationLock $coordinationLockPath {
            $script:state = Read-RuntimeState $statePath
            if (-not (Test-DependenciesComplete $selectedTask $queue $script:state)) {
                throw "Task '$($selectedTask.id)' has an incomplete dependency."
            }
            $claimConflict = Test-ConflictsWithActiveTask $selectedTask $queue $script:state
            if ($null -ne $claimConflict) {
                throw "Task '$($selectedTask.id)' overlaps active task '$claimConflict'."
            }
            Set-RuntimeTaskState $script:state $selectedTask "in_progress" $providerName $previousProvider $attempt "Provider started."
            Set-ProviderState $script:state $providerName "available" $null "Provider invocation started."
            Save-RuntimeState $script:state $statePath
        }

        $promptPath = Get-FullPath ([string]$selectedTask.promptFile) $repositoryRoot
        if (-not (Test-Path -LiteralPath $promptPath -PathType Leaf)) {
            throw "Task prompt was not found: $promptPath"
        }
        $rulesPath = Join-Path $repositoryRoot "AI_RULES.md"
        $promptText = @"
Follow the repository rules below.

$(Get-Content -Raw -LiteralPath $rulesPath)

Task contract (do not modify its scope):
$($selectedTask | ConvertTo-Json -Depth 20)

Task prompt:
$(Get-Content -Raw -LiteralPath $promptPath)
"@

        $timestamp = [DateTimeOffset]::UtcNow.ToString("yyyyMMdd-HHmmssfff")
        [System.IO.Directory]::CreateDirectory($logDirectory) | Out-Null
        $logPath = Join-Path $logDirectory ("{0}-{1}-{2}.log" -f $timestamp, $attempt, $providerName)
        Write-Host "Running task $($selectedTask.id) with $providerName in $worktree"
        $result = Invoke-ProviderProcess $providerCommand @($provider.arguments) $promptText $worktree $ProviderTimeoutSeconds
        [System.IO.File]::WriteAllText(
            $logPath,
            (Protect-LogText $result.Output),
            [System.Text.UTF8Encoding]::new($false))

        $changedFiles = @(Get-ChangedFiles $worktree $baseHead)
        if ($result.TimedOut) {
            Write-Checkpoint $checkpointDirectory $selectedTask "blocked" $providerName $previousProvider $worktree $baseHead $changedFiles @() "Inspect the timeout and resume explicitly." "Provider timed out; process tree was terminated."
            Invoke-WithCoordinationLock $coordinationLockPath {
                $script:state = Read-RuntimeState $statePath
                Set-RuntimeTaskState $script:state $selectedTask "blocked" $providerName $previousProvider $attempt "Provider timed out."
                Save-RuntimeState $script:state $statePath
            }
            throw "Provider '$providerName' timed out. Failover is not automatic for timeouts."
        }

        if ($result.ExitCode -ne 0 -and (Test-IsRateLimit $result.Output $providerConfiguration)) {
            try {
                Assert-ChangedFilesAllowed $changedFiles $selectedTask $worktree
            }
            catch {
                Write-Checkpoint $checkpointDirectory $selectedTask "blocked" $providerName $previousProvider $worktree $baseHead $changedFiles @() "Integrator reviews unauthorized changes; do not transfer ownership." $_.Exception.Message
                Invoke-WithCoordinationLock $coordinationLockPath {
                    $script:state = Read-RuntimeState $statePath
                    Set-RuntimeTaskState $script:state $selectedTask "blocked" $providerName $previousProvider $attempt $_.Exception.Message
                    Save-RuntimeState $script:state $statePath
                }
                throw
            }
            $blockedUntil = [DateTimeOffset]::UtcNow.AddMinutes([int]$provider.retryAfterMinutes)
            Write-Checkpoint $checkpointDirectory $selectedTask "checkpointed" $providerName $previousProvider $worktree $baseHead $changedFiles @() "Transfer ownership to the next configured provider." "Recognized provider limit."
            Invoke-WithCoordinationLock $coordinationLockPath {
                $script:state = Read-RuntimeState $statePath
                Set-ProviderState $script:state $providerName "rate_limited" $blockedUntil "Recognized limit; see immutable attempt log."
                Set-RuntimeTaskState $script:state $selectedTask "checkpointed" $providerName $previousProvider $attempt "Recognized provider limit."
                Save-RuntimeState $script:state $statePath
            }
            $previousProvider = $providerName
            Close-ExclusiveLock $providerLock $providerLockPath
            $providerLock = $null
            $providerLockPath = $null
            continue
        }

        if ($result.ExitCode -ne 0) {
            Write-Checkpoint $checkpointDirectory $selectedTask "blocked" $providerName $previousProvider $worktree $baseHead $changedFiles @() "Review the task/provider error; do not fail over automatically." "Provider exited with code $($result.ExitCode)."
            Invoke-WithCoordinationLock $coordinationLockPath {
                $script:state = Read-RuntimeState $statePath
                Set-RuntimeTaskState $script:state $selectedTask "blocked" $providerName $previousProvider $attempt "Non-limit provider failure."
                Save-RuntimeState $script:state $statePath
            }
            throw "Provider '$providerName' failed with exit code $($result.ExitCode). See $logPath"
        }

        try {
            Assert-ChangedFilesAllowed $changedFiles $selectedTask $worktree
            Invoke-WithCoordinationLock $coordinationLockPath {
                $script:state = Read-RuntimeState $statePath
                Set-RuntimeTaskState $script:state $selectedTask "testing" $providerName $previousProvider $attempt "Running task checks."
                Save-RuntimeState $script:state $statePath
            }
            Push-Location $worktree
            try {
                $checkResults = @(Invoke-TaskChecks $selectedTask $worktree)
            }
            finally {
                Pop-Location
            }
        }
        catch {
            $changedFiles = @(Get-ChangedFiles $worktree $baseHead)
            Write-Checkpoint $checkpointDirectory $selectedTask "blocked" $providerName $previousProvider $worktree $baseHead $changedFiles @() "Fix the contract or check failure in the same task." $_.Exception.Message
            Invoke-WithCoordinationLock $coordinationLockPath {
                $script:state = Read-RuntimeState $statePath
                Set-RuntimeTaskState $script:state $selectedTask "blocked" $providerName $previousProvider $attempt $_.Exception.Message
                Save-RuntimeState $script:state $statePath
            }
            throw
        }

        Write-Checkpoint $checkpointDirectory $selectedTask "ready_to_merge" $providerName $previousProvider $worktree $baseHead $changedFiles $checkResults "Integrator reviews, commits if needed, merges sequentially, then runs the full suite." "Provider execution and task checks passed."
        Invoke-WithCoordinationLock $coordinationLockPath {
            $script:state = Read-RuntimeState $statePath
            Set-RuntimeTaskState $script:state $selectedTask "ready_to_merge" $providerName $previousProvider $attempt "Task checks passed; integrator review required."
            Set-ProviderState $script:state $providerName "available" $null "Last task completed."
            Save-RuntimeState $script:state $statePath
        }
        $completed = $true
        Write-Host "Task $($selectedTask.id) is ready_to_merge. It is not done until integrator review and full tests pass."
        break
    }

    if (-not $completed) {
        throw "No configured provider could run task '$($selectedTask.id)'."
    }
}
finally {
    if ($null -ne $providerLock -and $null -ne $providerLockPath) {
        Close-ExclusiveLock $providerLock $providerLockPath
    }
    Close-ExclusiveLock $taskLock $taskLockPath
}
