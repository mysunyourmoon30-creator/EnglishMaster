param(
    [string]$BindAddress = "127.0.0.1",
    [int]$ApiPort = 7101,
    [int]$WebPort = 7102,
    [ValidateSet("InMemory", "SqlServer")]
    [string]$DatabaseProvider = "SqlServer",
    [string]$SqlServer = ".",
    [string]$DatabaseName = "EnglishMasterInternal",
    [string]$SqlUser,
    [string]$SqlPassword,
    [string]$AdminEmail = "internal.admin@englishmaster.local",
    [string]$AdminPassword,
    [switch]$EnableDevelopmentSeed
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$runRoot = Join-Path $repoRoot ".local-internal"
$logsRoot = Join-Path $runRoot "logs"
$mediaRoot = Join-Path $runRoot "media"
$publishingRoot = Join-Path $runRoot "publishing"
$apiKeysRoot = Join-Path $runRoot "keys\api"
$webKeysRoot = Join-Path $runRoot "keys\web"
$buildStamp = Get-Date -Format "yyyyMMddHHmmss"
$apiBuildRoot = Join-Path $runRoot "build\api-$buildStamp"
$webBuildRoot = Join-Path $runRoot "build\web-$buildStamp"
$apiPidFile = Join-Path $runRoot "api.pid"
$webPidFile = Join-Path $runRoot "web.pid"

New-Item -ItemType Directory -Force -Path $runRoot, $logsRoot, $mediaRoot, $publishingRoot, $apiKeysRoot, $webKeysRoot, $apiBuildRoot, $webBuildRoot | Out-Null

if ([string]::IsNullOrWhiteSpace($AdminPassword)) {
    $securePassword = Read-Host "Temporary SuperAdmin password" -AsSecureString
    $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    try {
        $AdminPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr)
    }
    finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
    }
}

if ([string]::IsNullOrWhiteSpace($AdminPassword)) {
    throw "AdminPassword is required."
}

function Stop-TrackedProcess {
    param([string]$PidFile)

    if (Test-Path $PidFile) {
        $trackedPid = Get-Content $PidFile -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($trackedPid) {
            $process = Get-Process -Id ([int]$trackedPid) -ErrorAction SilentlyContinue
            if ($process) {
                Stop-Process -Id $process.Id -Force
            }
        }
        Remove-Item -LiteralPath $PidFile -Force
    }
}

Stop-TrackedProcess -PidFile $apiPidFile
Stop-TrackedProcess -PidFile $webPidFile

$apiUrl = "http://$BindAddress`:$ApiPort"
$webUrl = "http://$BindAddress`:$WebPort"
$webApiBaseUrl = if ($BindAddress -eq "0.0.0.0") { "http://127.0.0.1:$ApiPort/" } else { "http://$BindAddress`:$ApiPort/" }
$sqliteDatabasePath = Join-Path $runRoot "$DatabaseName.db"
$connectionString = if ($DatabaseProvider -eq "SqlServer" -and -not [string]::IsNullOrWhiteSpace($SqlUser)) {
    if ([string]::IsNullOrWhiteSpace($SqlPassword)) {
        $secureSqlPassword = Read-Host "SQL Server password for $SqlUser" -AsSecureString
        $sqlBstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureSqlPassword)
        try {
            $SqlPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($sqlBstr)
        }
        finally {
            [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($sqlBstr)
        }
    }

    "Server=$SqlServer;Database=$DatabaseName;User Id=$SqlUser;Password=$SqlPassword;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False"
} else {
    "Server=$SqlServer;Database=$DatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False"
}
$seedEnabled = if ($EnableDevelopmentSeed.IsPresent) { "true" } else { "false" }

$apiProject = Join-Path $repoRoot "src\Backend\EnglishMaster.Api\EnglishMaster.Api.csproj"
$webProject = Join-Path $repoRoot "src\Frontend\EnglishMaster.Web\EnglishMaster.Web.csproj"
$apiLog = Join-Path $logsRoot "api.log"
$webLog = Join-Path $logsRoot "web.log"
$apiErrorLog = Join-Path $logsRoot "api.err.log"
$webErrorLog = Join-Path $logsRoot "web.err.log"
$apiRunner = Join-Path $runRoot "run-api.ps1"
$webRunner = Join-Path $runRoot "run-web.ps1"
$powershellExe = Join-Path $env:SystemRoot "System32\WindowsPowerShell\v1.0\powershell.exe"
$dotnetExe = (Get-Command dotnet).Source

function Start-HiddenPowerShellProcess {
    param(
        [string]$Command,
        [string]$LogPath,
        [string]$ErrorLogPath,
        [string]$RunnerPath,
        [string]$PidPath
    )

    $wrappedCommand = @"
try {
`$PID | Set-Content -Path '$PidPath'
`$ErrorActionPreference = 'Continue'
& {
$Command
} 1>> '$LogPath' 2>> '$ErrorLogPath'
} catch {
    Write-Error `$_
    exit 1
}
"@

    Set-Content -Path $RunnerPath -Value $wrappedCommand -Encoding UTF8
    $process = Start-Process -FilePath $powershellExe -WindowStyle Hidden -PassThru -ArgumentList @("-NoProfile", "-ExecutionPolicy", "Bypass", "-File", $RunnerPath)
    return $process.Id
}

$apiCommand = @"
`$env:ASPNETCORE_ENVIRONMENT='Development'
`$env:ASPNETCORE_URLS='$apiUrl'
`$env:Database__Provider='$DatabaseProvider'
`$env:Database__Name='$DatabaseName'
`$env:ConnectionStrings__DefaultConnection='$connectionString'
`$env:DevelopmentSeed__Enabled='$seedEnabled'
`$env:Auth__InitialSuperAdmin__Email='$AdminEmail'
`$env:Auth__InitialSuperAdmin__Password='$AdminPassword'
`$env:Media__LocalStoragePath='$mediaRoot'
`$env:Publishing__LocalStoragePath='$publishingRoot'
`$env:Logging__FilePath='$logsRoot'
`$env:DataProtection__KeysPath='$apiKeysRoot'
& '$dotnetExe' build '$apiProject' --no-restore -p:UseAppHost=false -p:OutputPath='$apiBuildRoot\'
if (`$LASTEXITCODE -ne 0) { exit `$LASTEXITCODE }
& '$dotnetExe' '$apiBuildRoot\EnglishMaster.Api.dll'
"@

$webCommand = @"
`$env:ASPNETCORE_ENVIRONMENT='Development'
`$env:ASPNETCORE_URLS='$webUrl'
`$env:ApiBaseUrl='$webApiBaseUrl'
`$env:Logging__FilePath='$logsRoot'
`$env:DataProtection__KeysPath='$webKeysRoot'
& '$dotnetExe' build '$webProject' --no-restore -p:UseAppHost=false -p:OutputPath='$webBuildRoot\'
if (`$LASTEXITCODE -ne 0) { exit `$LASTEXITCODE }
& '$dotnetExe' '$webBuildRoot\EnglishMaster.Web.dll'
"@

$apiPid = Start-HiddenPowerShellProcess -Command $apiCommand -LogPath $apiLog -ErrorLogPath $apiErrorLog -RunnerPath $apiRunner -PidPath $apiPidFile

$apiReady = $false
for ($i = 0; $i -lt 60; $i++) {
    Start-Sleep -Seconds 1
    try {
        $response = Invoke-WebRequest -Uri "http://127.0.0.1:$ApiPort/health/ready" -UseBasicParsing -TimeoutSec 2
        if ($response.StatusCode -eq 200) {
            $apiReady = $true
            break
        }
    }
    catch {
    }
}

if (-not $apiReady) {
    throw "API did not become ready. See $apiLog and $apiErrorLog"
}

$webPid = Start-HiddenPowerShellProcess -Command $webCommand -LogPath $webLog -ErrorLogPath $webErrorLog -RunnerPath $webRunner -PidPath $webPidFile

$webReady = $false
for ($i = 0; $i -lt 60; $i++) {
    Start-Sleep -Seconds 1
    try {
        $response = Invoke-WebRequest -Uri "http://127.0.0.1:$WebPort/health/live" -UseBasicParsing -TimeoutSec 2
        if ($response.StatusCode -eq 200) {
            $webReady = $true
            break
        }
    }
    catch {
    }
}

if (-not $webReady) {
    throw "Web did not become ready. See $webLog and $webErrorLog"
}

Write-Host "EnglishMaster local internal stack is running."
Write-Host "Web: http://127.0.0.1:$WebPort"
Write-Host "API: http://127.0.0.1:$ApiPort"
Write-Host "Admin email: $AdminEmail"
Write-Host "Logs: $logsRoot"
