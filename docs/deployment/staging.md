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
- `DevelopmentSeed__Enabled=false`
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
- `ENGLISHMASTER_STAGING_SUPERADMIN_EMAIL`
- `ENGLISHMASTER_STAGING_SUPERADMIN_PASSWORD`

## Local-Staging Docker Compose

Validate configuration on a Docker-enabled machine:

```powershell
docker compose -f docker-compose.staging.yml config
```

Start local staging:

```powershell
docker compose -f docker-compose.staging.yml up --build
```

Default local-staging ports:

- Web: `http://localhost:7102`
- API: `http://localhost:7101`
- SQL Server: `localhost,14334`

## Database Preparation

For local-staging Compose, SQL Server runs as `englishmaster-staging-sqlserver` with a persistent named volume.

To create or update the database manually outside Compose:

1. Create a SQL Server database such as `EnglishMasterStaging`.
2. Set `ConnectionStrings__DefaultConnection` to the staging database.
3. Apply EF Core migrations:

```powershell
dotnet ef database update --project src/Backend/EnglishMaster.Infrastructure --startup-project src/Backend/EnglishMaster.Api
```

The API startup seed flow also applies migrations outside the Testing environment. For staging, decide whether startup migration is acceptable or whether the release process should apply migrations explicitly before starting the app.

## Safe Seeding

Keep demo content disabled for staging unless the environment is explicitly a disposable demo:

```text
DevelopmentSeed__Enabled=false
```

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

Local-staging Compose uses named volumes:

- `englishmaster-staging-api-media`
- `englishmaster-staging-api-publishing`

In hosted staging, map `Publishing__LocalStoragePath` and media storage to durable storage according to the hosting platform.

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

## Staging Smoke Test

After deployment:

1. Confirm API `/health/ready` returns healthy.
2. Confirm Web `/health/live` returns healthy.
3. Open the Web app.
4. Confirm unauthenticated `/admin` redirects to `/login`.
5. Login with the staging SuperAdmin.
6. Open Dashboard, Words, Categories, Tags, Media, Pronunciations, Grammar, Lessons, Courses, Books, Quizzes, Publishing, Users, Roles, Permissions, Import, and Export.
7. Upload one valid small media file and one invalid file to verify validation.
8. Run one small Words import with a known invalid row and confirm row-level errors.

## Staging Risks

- Docker validation must be run on a Docker-enabled machine.
- Startup migrations should be reviewed before using them in non-disposable staging.
- Local-staging SQL Server uses Developer edition and is not a production database setup.
- Real HTTPS, host names, certificates, and secret storage are environment responsibilities.
