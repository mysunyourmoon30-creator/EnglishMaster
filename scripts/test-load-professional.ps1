param(
    [string]$SqlServer = "localhost",
    [string]$DatabaseName = "EnglishMasterLoadTest",
    [int]$Records = 1000000,
    [int]$ApiPort = 7201,
    [int]$WebPort = 7202,
    [string]$AdminEmail = "load.admin@englishmaster.local",
    [string]$AdminPassword = $env:ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD,
    [switch]$NoReset,
    [switch]$SkipSeed,
    [switch]$StopAfter
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $repoRoot

if ([string]::IsNullOrWhiteSpace($AdminPassword)) {
    $AdminPassword = "LoadTestPassword1!"
}

$runRoot = Join-Path $repoRoot ".local-internal"
$logsRoot = Join-Path $runRoot "logs"
$apiPidFile = Join-Path $runRoot "load-api.pid"
$webPidFile = Join-Path $runRoot "load-web.pid"
New-Item -ItemType Directory -Force -Path $runRoot, $logsRoot | Out-Null

function Stop-TrackedProcess {
    param([string]$PidFile)

    if (-not (Test-Path $PidFile)) {
        return
    }

    $pidValue = Get-Content $PidFile -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($pidValue) {
        $process = Get-Process -Id ([int]$pidValue) -ErrorAction SilentlyContinue
        if ($process) {
            Stop-Process -Id $process.Id -Force
        }
    }

    Remove-Item -LiteralPath $PidFile -Force
}

function Wait-HttpOk {
    param(
        [string]$Url,
        [int]$Seconds = 90
    )

    $deadline = (Get-Date).AddSeconds($Seconds)
    do {
        try {
            $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 3
            if ($response.StatusCode -eq 200) {
                return
            }
        }
        catch {
            if ((Get-Date) -ge $deadline) {
                throw
            }
        }

        Start-Sleep -Seconds 1
    } while ($true)
}

function Measure-Check {
    param(
        [string]$Name,
        [scriptblock]$Action,
        [int]$MaxMilliseconds = 10000
    )

    Write-Host ""
    Write-Host "==> $Name" -ForegroundColor Cyan
    $elapsed = Measure-Command { & $Action }
    $ms = [math]::Round($elapsed.TotalMilliseconds)
    if ($ms -gt $MaxMilliseconds) {
        throw "$Name took ${ms}ms, above ${MaxMilliseconds}ms."
    }

    Write-Host "PASS: $Name (${ms}ms)" -ForegroundColor Green
}

if (-not $SkipSeed) {
    $seedArgs = @(
        "--sql-server", $SqlServer,
        "--database", $DatabaseName,
        "--records", $Records.ToString(),
        "--admin-email", $AdminEmail,
        "--admin-password", $AdminPassword
    )
    if ($NoReset) {
        $seedArgs += "--no-reset"
    }

    Write-Host "==> Seed SQL Server load-test database" -ForegroundColor Cyan
    dotnet run --project tools/EnglishMaster.LoadTest/EnglishMaster.LoadTest.csproj --configuration Release --no-build -- @seedArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Load-test seed failed with exit code $LASTEXITCODE."
    }
}

Stop-TrackedProcess -PidFile $apiPidFile
Stop-TrackedProcess -PidFile $webPidFile

$connectionString = "Server=$SqlServer;Database=$DatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False"

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://127.0.0.1:$ApiPort"
$env:Database__Provider = "SqlServer"
$env:Database__Name = $DatabaseName
$env:ConnectionStrings__DefaultConnection = $connectionString
$env:DevelopmentSeed__Enabled = "false"
$env:Auth__InitialSuperAdmin__Email = $AdminEmail
$env:Auth__InitialSuperAdmin__Password = $AdminPassword
$env:Media__LocalStoragePath = Join-Path $runRoot "load-media"
$env:Publishing__LocalStoragePath = Join-Path $runRoot "load-publishing"
$env:Logging__FilePath = $logsRoot
$env:DataProtection__KeysPath = Join-Path $runRoot "keys\load-api"

$apiProcess = Start-Process -FilePath dotnet `
    -ArgumentList @("run", "--project", "src\Backend\EnglishMaster.Api\EnglishMaster.Api.csproj", "--configuration", "Release", "--no-build", "--no-launch-profile", "--no-restore") `
    -WindowStyle Hidden `
    -RedirectStandardOutput (Join-Path $logsRoot "load-api.out.log") `
    -RedirectStandardError (Join-Path $logsRoot "load-api.err.log") `
    -PassThru
$apiProcess.Id | Set-Content $apiPidFile
Wait-HttpOk -Url "http://127.0.0.1:$ApiPort/health/ready"

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://127.0.0.1:$WebPort"
$env:ApiBaseUrl = "http://127.0.0.1:$ApiPort/"
$env:Logging__FilePath = $logsRoot
$env:DataProtection__KeysPath = Join-Path $runRoot "keys\load-web"

$webProcess = Start-Process -FilePath dotnet `
    -ArgumentList @("run", "--project", "src\Frontend\EnglishMaster.Web\EnglishMaster.Web.csproj", "--configuration", "Release", "--no-build", "--no-launch-profile", "--no-restore") `
    -WindowStyle Hidden `
    -RedirectStandardOutput (Join-Path $logsRoot "load-web.out.log") `
    -RedirectStandardError (Join-Path $logsRoot "load-web.err.log") `
    -PassThru
$webProcess.Id | Set-Content $webPidFile
Wait-HttpOk -Url "http://127.0.0.1:$WebPort/health/live"

$apiBaseUrl = "http://127.0.0.1:$ApiPort"
$webBaseUrl = "http://127.0.0.1:$WebPort"
$apiSession = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

Measure-Check -Name "API login under load" -MaxMilliseconds 5000 -Action {
    $response = Invoke-WebRequest `
        -Uri "$apiBaseUrl/api/v1/auth/login" `
        -Method Post `
        -Body (@{ email = $AdminEmail; password = $AdminPassword } | ConvertTo-Json) `
        -ContentType "application/json" `
        -WebSession $apiSession `
        -UseBasicParsing `
        -TimeoutSec 20
    if ($response.StatusCode -ne 200) { throw "API login failed." }
}

Measure-Check -Name "API words search under load" -MaxMilliseconds 5000 -Action {
    $response = Invoke-WebRequest -Uri "$apiBaseUrl/api/v1/words?search=load-word-0001001&pageSize=20" -WebSession $apiSession -UseBasicParsing -TimeoutSec 20
    if ($response.StatusCode -ne 200) { throw "API words search failed." }
}

Measure-Check -Name "API quiz list under load" -MaxMilliseconds 5000 -Action {
    $response = Invoke-WebRequest -Uri "$apiBaseUrl/api/v1/quizzes?pageSize=20" -WebSession $apiSession -UseBasicParsing -TimeoutSec 20
    if ($response.StatusCode -ne 200) { throw "API quiz list failed." }
}

Measure-Check -Name "Web admin login under load" -MaxMilliseconds 10000 -Action {
    $response = Invoke-WebRequest `
        -Uri "$webBaseUrl/account/login?returnUrl=%2Fadmin%2Fquizzes" `
        -Method Post `
        -Body @{ email = $AdminEmail; password = $AdminPassword } `
        -WebSession $session `
        -UseBasicParsing `
        -TimeoutSec 20
    if ($response.Content -notlike "*Quizzes*") { throw "Login did not reach quizzes page." }
}

Measure-Check -Name "Web quizzes page under load" -MaxMilliseconds 10000 -Action {
    $response = Invoke-WebRequest -Uri "$webBaseUrl/admin/quizzes" -WebSession $session -UseBasicParsing -TimeoutSec 20
    if ($response.Content -notlike "*Find quizzes / *") { throw "Quizzes UX smoke text not found." }
}

Write-Host ""
Write-Host "Load test passed." -ForegroundColor Green
Write-Host "Load Web: $webBaseUrl"
Write-Host "Load API: $apiBaseUrl"
Write-Host "Database: $DatabaseName"

if ($StopAfter) {
    Stop-TrackedProcess -PidFile $apiPidFile
    Stop-TrackedProcess -PidFile $webPidFile
}
