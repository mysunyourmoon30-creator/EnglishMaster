# Staging Deployment

## Purpose

This guide prepares EnglishMaster MVP for staging deployment. It does not describe production deployment, Kubernetes, microservices, paid services, or cloud-specific automation.

## Staging Files

- `docker-compose.staging.yml`
- `src/Backend/EnglishMaster.Api/appsettings.Staging.example.json`
- `src/Frontend/EnglishMaster.Web/appsettings.Staging.example.json`
- `.env.example`

Use the example files as templates only. Do not commit real staging passwords, connection strings, certificates, tokens, or SuperAdmin credentials.

## Required Environment Variables

Core ASP.NET variables:

- `ASPNETCORE_ENVIRONMENT=Staging`
- `ASPNETCORE_URLS=http://+:8080` inside containers
- `AllowedHosts`

API variables:

- `ConnectionStrings__DefaultConnection`
- `Database__ApplyMigrationsOnStartup=false`
- `DevelopmentSeed__Enabled=false`
- `SeedGrammarCurriculum__Enabled=false`
- `Auth__AllowInsecureLoopbackCookies=false`
- `Auth__InitialSuperAdmin__Email`
- `Auth__InitialSuperAdmin__Password`
- `Publishing__LocalStoragePath`

Web variables:

- `ApiBaseUrl`

Local-staging Compose variables:

- `ENGLISHMASTER_STAGING_SQL_PASSWORD`
- `ENGLISHMASTER_STAGING_SQL_PORT`
- `ENGLISHMASTER_STAGING_DATABASE`
- `ENGLISHMASTER_STAGING_API_PORT`
- `ENGLISHMASTER_STAGING_WEB_PORT`
- `ENGLISHMASTER_STAGING_ALLOWED_HOSTS`
- `ENGLISHMASTER_STAGING_API_BASE_URL`
- `ENGLISHMASTER_STAGING_DEVELOPMENT_SEED_ENABLED`
- `ENGLISHMASTER_STAGING_ALLOW_INSECURE_LOOPBACK_COOKIES`
- `ENGLISHMASTER_STAGING_SUPERADMIN_EMAIL`
- `ENGLISHMASTER_STAGING_SUPERADMIN_PASSWORD`

## Local-Staging Docker Compose

Validate configuration on a Docker-enabled machine:

```powershell
docker compose -f docker-compose.staging.yml config
```

Start the staging database first:

```powershell
docker compose -f docker-compose.staging.yml up -d englishmaster-staging-sqlserver
```

Default local-staging ports:

- Web: `http://localhost:7102`
- API: `http://localhost:7101`
- SQL Server: `localhost,14334`

Authenticated HTTP smoke testing cannot normally reuse a `Secure` cookie. For
the disposable local/CI Compose stack only, set
`ENGLISHMASTER_STAGING_ALLOW_INSECURE_LOOPBACK_COOKIES=true`. The override
removes the `Secure` flag only when the request host is `localhost` or a
loopback IP. Keep it `false` for hosted staging, which must use HTTPS.

## Database Preparation

For local-staging Compose, SQL Server runs as `englishmaster-staging-sqlserver` with a persistent named volume.

Staging uses an explicit release migration. The API has
`Database__ApplyMigrationsOnStartup=false` and must not own schema changes.
Before starting the API:

1. Take a backup when the database already exists.
2. Provide `ConnectionStrings__DefaultConnection` from the staging secret
   store to the operator process.
3. Apply the reviewed migrations:

```powershell
dotnet ef database update --project src/Backend/EnglishMaster.Infrastructure --startup-project src/Backend/EnglishMaster.Api
```

4. Remove the connection string from the operator process.
5. Start the complete stack:

```powershell
docker compose -f docker-compose.staging.yml up -d --build
docker compose -f docker-compose.staging.yml ps
```

The release workflow also produces
`artifacts/migrations/englishmaster-migrate`, a self-contained Linux migration
bundle for Linux staging/production operators. Do not pass the connection
string on the command line; the bundle reads
`ConnectionStrings__DefaultConnection` from its environment.

## Safe Seeding

Keep demo content disabled for staging unless the environment is explicitly a disposable demo:

```text
DevelopmentSeed__Enabled=false
SeedGrammarCurriculum__Enabled=false
```

When staging needs the managed grammar curriculum, enable `SeedGrammarCurriculum__Enabled` for one startup only and disable it immediately afterward.

Create the initial SuperAdmin only through staging-safe configuration:

```text
Auth__InitialSuperAdmin__Email=<staging-admin-email>
Auth__InitialSuperAdmin__Password=<temporary-staging-password>
```

After first login:

1. Change or rotate the temporary password.
2. Remove the bootstrap password from the staging environment.
3. Restart the app with the bootstrap password unset.

If either SuperAdmin value is empty, the seed does not create the user.

## Storage

The API requires persistent storage for:

- Uploaded media
- Publishing artifacts
- API Data Protection keys
- Structured file logs

The Web app requires separate persistent storage for:

- Web Data Protection keys
- Structured file logs

Local-staging Compose uses named volumes:

- `englishmaster-staging-api-media`
- `englishmaster-staging-api-publishing`
- `englishmaster-staging-api-data-protection`
- `englishmaster-staging-api-logs`
- `englishmaster-staging-web-data-protection`
- `englishmaster-staging-web-logs`

In hosted staging, map `Publishing__LocalStoragePath` and media storage to durable storage according to the hosting platform.
Keep API and Web Data Protection key rings separate and access-restricted.

## Health Checks

Staging should verify:

- API: `/health`
- API liveness: `/health/live`
- API readiness: `/health/ready`
- Web: `/health`
- Web liveness: `/health/live`
- Web readiness: `/health/ready`

API readiness checks SQL Server connectivity. Web readiness confirms the Blazor host is running.

PowerShell examples:

```powershell
Invoke-WebRequest http://localhost:7101/health/live
Invoke-WebRequest http://localhost:7101/health/ready
Invoke-WebRequest http://localhost:7102/health/live
```

## Automated Staging Smoke Gate

Run from a trusted operator machine after deployment. Supply credentials only
through process environment variables; the script never prints them:

```powershell
$env:ENGLISHMASTER_SMOKE_ADMIN_EMAIL = "<staging-admin-email>"
$env:ENGLISHMASTER_SMOKE_ADMIN_PASSWORD = "<staging-admin-password>"
./scripts/Invoke-EnglishMasterReleaseSmoke.ps1 `
  -ApiBaseUrl "https://api.staging.example.com" `
  -WebBaseUrl "https://app.staging.example.com" `
  -RequireAuthenticatedChecks
Remove-Item Env:ENGLISHMASTER_SMOKE_ADMIN_EMAIL
Remove-Item Env:ENGLISHMASTER_SMOKE_ADMIN_PASSWORD
```

The automated gate verifies API/Web health, public grammar topic and rule
access for anonymous users, the unauthenticated admin redirect, protected API
rejection, login, the admin dashboard API, and logout. Override
`PublicGrammarTopicSlug` and `PublicGrammarRuleSlug` when staging uses
different active curriculum records.

The tagged release workflow runs the same gate against disposable Linux
containers after applying the packaged migration bundle. A passing workflow
proves image/startup/network behavior; it does not replace the hosted staging
gate or manual browser checks. The workflow enables the loopback-only cookie
override because its disposable smoke URLs use `127.0.0.1` over HTTP.

## Manual Staging Smoke Test

After the automated gate:

1. Open the Web app in a browser.
2. Confirm the authenticated admin shell and representative list/detail forms.
3. Upload one valid small media file and one invalid file to verify validation.
4. Run one small Words import with a known invalid row and confirm row-level errors.

## Staging Risks

- Docker validation must be run on a Docker-enabled machine.
- The explicit migration must complete before the API starts; an empty or stale
  schema intentionally causes readiness/startup failure.
- Local-staging SQL Server uses Developer edition and is not a production database setup.
- Real HTTPS, host names, certificates, and secret storage are environment responsibilities.
