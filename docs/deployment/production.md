# Production Deployment

## Purpose

This guide prepares EnglishMaster for a safer production deployment. It does not prescribe a specific hosting provider and does not automate deployment.

## Required Before Go-Live

- Use HTTPS at the edge or platform load balancer.
- Set production host names in `AllowedHosts`.
- Set `ASPNETCORE_ENVIRONMENT=Production` for API and Web.
- Provide `ConnectionStrings__DefaultConnection` from deployment secrets.
- Set `DevelopmentSeed__Enabled=false`.
- Configure durable paths for `Media__LocalStoragePath` and `Publishing__LocalStoragePath`.
- Configure the Web app `ApiBaseUrl` to the production HTTPS API URL.
- Keep email provider settings as placeholders until a production email adapter is added.

## Application Boundaries

The API exposes protected `/api/v1/*` endpoints, health checks, `/media`, and `/publishing` static file paths. The Web app serves the Blazor UI and calls the API through typed HTTP clients.

Admin routes are protected by Web middleware and API permission policies. API authorization remains the primary security boundary.

## Health Checks

Use:

```text
/health
/health/live
/health/ready
```

The API readiness endpoint includes database connectivity. Do not expose detailed exception output from health checks publicly.

## Deployment Flow

1. Build and test from a clean source revision.
2. Publish API and Web artifacts.
3. Provision a production SQL Server database.
4. Apply EF Core migrations.
5. Configure environment variables and secrets.
6. Mount durable media and publishing storage.
7. Start API and Web behind HTTPS.
8. Create the initial SuperAdmin through temporary bootstrap settings or a controlled operational process.
9. Rotate or remove bootstrap credentials.
10. Run smoke checks without exposing secrets in logs.

## Not Included

- No production deployment was performed.
- No payment, mobile, AI, marketplace, or microservice changes are included.
- No real email provider integration is enabled.
