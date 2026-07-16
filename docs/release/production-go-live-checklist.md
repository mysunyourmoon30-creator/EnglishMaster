# Production Go-Live Checklist

## Build And Test

- [ ] Release build completed from the intended source revision.
- [ ] Unit tests passed.
- [ ] Architecture tests passed.
- [ ] Integration tests passed.
- [ ] Release artifacts were produced without secrets in logs.

## Configuration

- [ ] `ASPNETCORE_ENVIRONMENT=Production`.
- [ ] Production `AllowedHosts` configured.
- [ ] Web `ApiBaseUrl` points to the production HTTPS API URL.
- [ ] Production SQL connection string stored in secrets.
- [ ] `DevelopmentSeed__Enabled=false`.
- [ ] SuperAdmin bootstrap values removed after setup.

## Database

- [ ] Production database is separate from staging.
- [ ] Backup taken before migration.
- [ ] Restore process tested in non-production.
- [ ] EF Core migrations applied.

## File Storage

- [ ] `Media__LocalStoragePath` points to durable storage.
- [ ] `Publishing__LocalStoragePath` points to durable storage.
- [ ] File backup process includes media and publishing artifacts.

## Security

- [ ] HTTPS configured.
- [ ] Secure cookies verified.
- [ ] CORS remains absent or restrictive.
- [ ] Admin routes require login.
- [ ] API endpoints require authentication and permissions.
- [ ] Health endpoints do not expose sensitive details.

## Operations

- [ ] `/health/live` responds.
- [ ] `/health/ready` responds.
- [ ] Logs are collected without secrets.
- [ ] Daily operations checklist owner assigned.
- [ ] Incident response owner assigned.
- [ ] Rollback plan documented.
- [ ] On-call owner identified.

## Blockers

Do not go live if production secrets are committed, database backup cannot be restored, durable file storage is missing, or HTTPS is not configured.
