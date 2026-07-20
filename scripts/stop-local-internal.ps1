$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$runRoot = Join-Path $repoRoot ".local-internal"
$pidFiles = @(
    Join-Path $runRoot "api.pid"
    Join-Path $runRoot "web.pid"
)

foreach ($pidFile in $pidFiles) {
    if (-not (Test-Path $pidFile)) {
        continue
    }

    $trackedPid = Get-Content $pidFile -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($trackedPid) {
        $process = Get-Process -Id ([int]$trackedPid) -ErrorAction SilentlyContinue
        if ($process) {
            Stop-Process -Id $process.Id -Force
            Write-Host "Stopped process $($process.Id)."
        }
    }

    Remove-Item -LiteralPath $pidFile -Force
}

Write-Host "EnglishMaster local internal stack is stopped."
