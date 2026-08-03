[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [uri] $ApiBaseUrl,

    [Parameter(Mandatory = $true)]
    [uri] $WebBaseUrl,

    [string] $PublicGrammarTopicSlug = "present-simple",

    [string] $PublicGrammarRuleSlug = "present-simple-for-habits",

    [ValidateRange(1, 120)]
    [int] $TimeoutSeconds = 20,

    [switch] $RequireAuthenticatedChecks
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Net.Http

$script:SmokeResults = @()

function New-SmokeClient {
    param(
        [bool] $AllowAutoRedirect = $false,
        [System.Net.CookieContainer] $CookieContainer
    )

    $handler = New-Object System.Net.Http.HttpClientHandler
    $handler.AllowAutoRedirect = $AllowAutoRedirect
    if ($null -ne $CookieContainer) {
        $handler.CookieContainer = $CookieContainer
        $handler.UseCookies = $true
    }

    $client = New-Object System.Net.Http.HttpClient($handler)
    $client.Timeout = [TimeSpan]::FromSeconds($TimeoutSeconds)
    return $client
}

function Join-SmokeUri {
    param(
        [uri] $BaseUrl,
        [string] $RelativePath
    )

    $normalizedBase = [uri]::new($BaseUrl.AbsoluteUri.TrimEnd("/") + "/")
    return [uri]::new($normalizedBase, $RelativePath.TrimStart("/"))
}

function Add-SmokeResult {
    param(
        [string] $Check,
        [int] $StatusCode,
        [string] $Outcome
    )

    $script:SmokeResults += [pscustomobject]@{
        Check = $Check
        Status = $StatusCode
        Outcome = $Outcome
    }
}

function Invoke-SmokeRequest {
    param(
        [System.Net.Http.HttpClient] $Client,
        [System.Net.Http.HttpMethod] $Method,
        [uri] $Uri,
        [System.Net.Http.HttpContent] $Content
    )

    $request = New-Object System.Net.Http.HttpRequestMessage($Method, $Uri)
    if ($null -ne $Content) {
        $request.Content = $Content
    }

    try {
        $response = $Client.SendAsync($request).GetAwaiter().GetResult()
        try {
            $body = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            return [pscustomobject]@{
                StatusCode = [int] $response.StatusCode
                Body = $body
                Location = $response.Headers.Location
            }
        }
        finally {
            $response.Dispose()
        }
    }
    finally {
        $request.Dispose()
    }
}

function Assert-Status {
    param(
        [string] $Check,
        [object] $Response,
        [int[]] $ExpectedStatusCodes
    )

    if ($Response.StatusCode -notin $ExpectedStatusCodes) {
        throw "$Check failed with HTTP $($Response.StatusCode); expected $($ExpectedStatusCodes -join ', ')."
    }

    Add-SmokeResult -Check $Check -StatusCode $Response.StatusCode -Outcome "Passed"
}

$adminEmail = [Environment]::GetEnvironmentVariable("ENGLISHMASTER_SMOKE_ADMIN_EMAIL")
$adminPassword = [Environment]::GetEnvironmentVariable("ENGLISHMASTER_SMOKE_ADMIN_PASSWORD")
$hasEmail = -not [string]::IsNullOrWhiteSpace($adminEmail)
$hasPassword = -not [string]::IsNullOrWhiteSpace($adminPassword)

if ($hasEmail -xor $hasPassword) {
    throw "Set both ENGLISHMASTER_SMOKE_ADMIN_EMAIL and ENGLISHMASTER_SMOKE_ADMIN_PASSWORD, or neither."
}

$runAuthenticatedChecks = $hasEmail -and $hasPassword
if ($RequireAuthenticatedChecks -and -not $runAuthenticatedChecks) {
    throw "Authenticated smoke checks are required, but the smoke credential environment variables are not set."
}

if ($runAuthenticatedChecks -and
    $ApiBaseUrl.Scheme -ne [uri]::UriSchemeHttps -and
    -not $ApiBaseUrl.IsLoopback) {
    throw "Authenticated smoke checks require HTTPS unless ApiBaseUrl is loopback."
}

$anonymousClient = New-SmokeClient
$authenticatedClient = $null

try {
    $apiLive = Invoke-SmokeRequest `
        -Client $anonymousClient `
        -Method ([System.Net.Http.HttpMethod]::Get) `
        -Uri (Join-SmokeUri $ApiBaseUrl "/health/live")
    Assert-Status "API liveness" $apiLive @(200)

    $apiReady = Invoke-SmokeRequest `
        -Client $anonymousClient `
        -Method ([System.Net.Http.HttpMethod]::Get) `
        -Uri (Join-SmokeUri $ApiBaseUrl "/health/ready")
    Assert-Status "API readiness" $apiReady @(200)

    $webLive = Invoke-SmokeRequest `
        -Client $anonymousClient `
        -Method ([System.Net.Http.HttpMethod]::Get) `
        -Uri (Join-SmokeUri $WebBaseUrl "/health/live")
    Assert-Status "Web liveness" $webLive @(200)

    $topicSlug = [uri]::EscapeDataString($PublicGrammarTopicSlug)
    $topicApi = Invoke-SmokeRequest `
        -Client $anonymousClient `
        -Method ([System.Net.Http.HttpMethod]::Get) `
        -Uri (Join-SmokeUri $ApiBaseUrl "/api/v1/public/grammar/topics/$topicSlug")
    Assert-Status "Anonymous grammar topic API" $topicApi @(200)

    $ruleSlug = [uri]::EscapeDataString($PublicGrammarRuleSlug)
    $ruleApi = Invoke-SmokeRequest `
        -Client $anonymousClient `
        -Method ([System.Net.Http.HttpMethod]::Get) `
        -Uri (Join-SmokeUri $ApiBaseUrl "/api/v1/public/grammar/rules/$ruleSlug")
    Assert-Status "Anonymous grammar rule API" $ruleApi @(200)

    $topicPage = Invoke-SmokeRequest `
        -Client $anonymousClient `
        -Method ([System.Net.Http.HttpMethod]::Get) `
        -Uri (Join-SmokeUri $WebBaseUrl "/grammar/topics/$topicSlug")
    Assert-Status "Anonymous grammar topic page" $topicPage @(200)

    $rulePage = Invoke-SmokeRequest `
        -Client $anonymousClient `
        -Method ([System.Net.Http.HttpMethod]::Get) `
        -Uri (Join-SmokeUri $WebBaseUrl "/grammar/rules/$ruleSlug")
    Assert-Status "Anonymous grammar rule page" $rulePage @(200)

    $adminPage = Invoke-SmokeRequest `
        -Client $anonymousClient `
        -Method ([System.Net.Http.HttpMethod]::Get) `
        -Uri (Join-SmokeUri $WebBaseUrl "/admin")
    Assert-Status "Anonymous admin redirect" $adminPage @(301, 302, 303, 307, 308)
    $adminRedirectPath = if ($null -ne $adminPage.Location -and $adminPage.Location.IsAbsoluteUri) {
        $adminPage.Location.AbsolutePath
    }
    elseif ($null -ne $adminPage.Location) {
        $adminPage.Location.OriginalString.Split("?")[0]
    }
    else {
        ""
    }
    if (-not $adminRedirectPath.StartsWith("/login")) {
        throw "Anonymous admin redirect did not target /login."
    }

    $protectedDashboard = Invoke-SmokeRequest `
        -Client $anonymousClient `
        -Method ([System.Net.Http.HttpMethod]::Get) `
        -Uri (Join-SmokeUri $ApiBaseUrl "/api/v1/reports/admin-dashboard")
    Assert-Status "Anonymous protected API" $protectedDashboard @(401)

    if ($runAuthenticatedChecks) {
        $cookies = New-Object System.Net.CookieContainer
        $authenticatedClient = New-SmokeClient -CookieContainer $cookies
        $loginJson = @{
            email = $adminEmail
            password = $adminPassword
        } | ConvertTo-Json -Compress
        $loginContent = New-Object System.Net.Http.StringContent(
            $loginJson,
            [System.Text.Encoding]::UTF8,
            "application/json")

        $login = Invoke-SmokeRequest `
            -Client $authenticatedClient `
            -Method ([System.Net.Http.HttpMethod]::Post) `
            -Uri (Join-SmokeUri $ApiBaseUrl "/api/v1/auth/login") `
            -Content $loginContent
        Assert-Status "Admin login" $login @(200)

        $dashboard = Invoke-SmokeRequest `
            -Client $authenticatedClient `
            -Method ([System.Net.Http.HttpMethod]::Get) `
            -Uri (Join-SmokeUri $ApiBaseUrl "/api/v1/reports/admin-dashboard")
        Assert-Status "Authenticated admin dashboard API" $dashboard @(200)

        $logout = Invoke-SmokeRequest `
            -Client $authenticatedClient `
            -Method ([System.Net.Http.HttpMethod]::Post) `
            -Uri (Join-SmokeUri $ApiBaseUrl "/api/v1/auth/logout")
        Assert-Status "Admin logout" $logout @(204)
    }
    else {
        Add-SmokeResult -Check "Authenticated checks" -StatusCode 0 -Outcome "Skipped (credentials not supplied)"
    }

    $script:SmokeResults | Format-Table -AutoSize | Out-String | Write-Host
    Write-Host "EnglishMaster release smoke checks passed."
}
finally {
    if ($null -ne $authenticatedClient) {
        $authenticatedClient.Dispose()
    }

    $anonymousClient.Dispose()
}
