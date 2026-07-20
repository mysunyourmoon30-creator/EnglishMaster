param(
    [string]$ApiBaseUrl = "http://127.0.0.1:7101",
    [string]$WebBaseUrl = "http://127.0.0.1:7102",
    [string]$AdminEmail = "internal.admin@englishmaster.local",
    [string]$AdminPassword = $env:ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD,
    [string]$QuizDetailPath,
    [switch]$SkipDotNetTests,
    [switch]$SkipLocalSmoke
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $repoRoot

function Write-Step {
    param([string]$Message)
    Write-Host ""
    Write-Host "==> $Message" -ForegroundColor Cyan
}

function Invoke-Step {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    Write-Step $Name
    & $Action
    Write-Host "PASS: $Name" -ForegroundColor Green
}

function Assert-Contains {
    param(
        [string]$Content,
        [string]$Expected,
        [string]$Context
    )

    if ($Content -notlike "*$Expected*") {
        throw "$Context did not contain expected text: $Expected"
    }
}

function Invoke-ReadyRequest {
    param(
        [string]$Uri,
        [int]$Seconds = 30,
        [Microsoft.PowerShell.Commands.WebRequestSession]$WebSession
    )

    $deadline = (Get-Date).AddSeconds($Seconds)
    do {
        try {
            if ($WebSession) {
                return Invoke-WebRequest -Uri $Uri -WebSession $WebSession -UseBasicParsing -TimeoutSec 5
            }

            return Invoke-WebRequest -Uri $Uri -UseBasicParsing -TimeoutSec 5
        }
        catch {
            if ((Get-Date) -ge $deadline) {
                throw
            }

            Start-Sleep -Seconds 1
        }
    } while ($true)
}

if (-not $SkipDotNetTests) {
    Invoke-Step "Build solution (Release)" {
        dotnet build EnglishMaster.sln --configuration Release
    }

    Invoke-Step "Run unit tests" {
        dotnet test tests/EnglishMaster.UnitTests/EnglishMaster.UnitTests.csproj --configuration Release --no-build
    }

    Invoke-Step "Run quiz integration tests" {
        dotnet test tests/EnglishMaster.IntegrationTests/EnglishMaster.IntegrationTests.csproj --configuration Release --no-build --filter "FullyQualifiedName~Quizzes|FullyQualifiedName~FoundationSmokeTests"
    }

    Invoke-Step "Run architecture tests" {
        dotnet test tests/EnglishMaster.ArchitectureTests/EnglishMaster.ArchitectureTests.csproj --configuration Release --no-build
    }
}

if (-not $SkipLocalSmoke) {
    Invoke-Step "Check local API health" {
        $response = Invoke-ReadyRequest -Uri "$ApiBaseUrl/health/ready"
        if ($response.StatusCode -ne 200) {
            throw "API health returned $($response.StatusCode)."
        }
    }

    Invoke-Step "Check local Web health" {
        $response = Invoke-ReadyRequest -Uri "$WebBaseUrl/health/live"
        if ($response.StatusCode -ne 200) {
            throw "Web health returned $($response.StatusCode)."
        }
    }

    if ([string]::IsNullOrWhiteSpace($AdminPassword)) {
        throw "AdminPassword is required for local UI smoke. Set ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD or pass -AdminPassword."
    }

    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

    Invoke-Step "Login through Web admin" {
        $loginResponse = Invoke-WebRequest `
            -Uri "$WebBaseUrl/account/login?returnUrl=%2Fadmin%2Fquizzes" `
            -Method Post `
            -WebSession $session `
            -Body @{
                email = $AdminEmail
                password = $AdminPassword
            } `
            -UseBasicParsing

        Assert-Contains -Content $loginResponse.Content -Expected "Quizzes" -Context "Login result"
    }

    Invoke-Step "Smoke test Quizzes list UX" {
        $response = Invoke-ReadyRequest -Uri "$WebBaseUrl/admin/quizzes" -WebSession $session
        if ($response.StatusCode -ne 200) {
            throw "Quizzes page returned $($response.StatusCode)."
        }

        $content = $response.Content
        Assert-Contains -Content $content -Expected "Exercises /" -Context "Quizzes page"
        Assert-Contains -Content $content -Expected "Find quizzes /" -Context "Quizzes page"
        Assert-Contains -Content $content -Expected "New Quiz /" -Context "Quizzes page"

        if ([string]::IsNullOrWhiteSpace($QuizDetailPath)) {
            $match = [regex]::Match($content, "/admin/quizzes/[0-9a-fA-F-]{36}")
            if ($match.Success) {
                $script:QuizDetailPath = $match.Value
            }
        }
    }

    if (-not [string]::IsNullOrWhiteSpace($QuizDetailPath)) {
        Invoke-Step "Smoke test Quiz detail UX" {
            $path = if ($QuizDetailPath.StartsWith("/")) { $QuizDetailPath } else { "/$QuizDetailPath" }
            $response = Invoke-ReadyRequest -Uri "$WebBaseUrl$path" -WebSession $session
            if ($response.StatusCode -ne 200) {
                throw "Quiz detail page returned $($response.StatusCode)."
            }

            $content = $response.Content
            Assert-Contains -Content $content -Expected "Questions" -Context "Quiz detail page"
            Assert-Contains -Content $content -Expected "Exercise Builder" -Context "Quiz detail page"
            Assert-Contains -Content $content -Expected "Add Question" -Context "Quiz detail page"
        }
    }
}

Write-Host ""
Write-Host "All professional automated checks passed." -ForegroundColor Green
