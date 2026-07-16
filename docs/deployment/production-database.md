# Production Database

## Database Separation

Use separate databases for staging and production. Do not point staging, local Compose, or automated tests at the production database.

## Applying Migrations

Apply EF Core migrations from a reviewed release artifact or controlled operator machine:

```powershell
dotnet ef database update --project src/Backend/EnglishMaster.Infrastructure --startup-project src/Backend/EnglishMaster.Api
```

Use production connection strings from secrets, not committed settings.

## Backup

Before every production migration:

1. Take a full SQL Server backup.
2. Verify the backup completed successfully.
3. Record the backup location and timestamp.
4. Keep backup access restricted.

See also [Database Backup And Restore](../operations/database-backup-restore.md) and [Backup Restore Checklist](../operations/backup-restore-checklist.md).

## Restore

Practice restore in a non-production environment before relying on it in production. A restore plan should include database backup restore, media storage restore, publishing artifact restore, and application version rollback.

See [Restore Procedure](../operations/restore-procedure.md) for the step-by-step operational flow.

## Initial SuperAdmin

The API can create a SuperAdmin only when both bootstrap email and password are configured. Use this only during controlled setup, then rotate the password and remove the bootstrap variables.

## Development Seed Data

Production must set:

```text
DevelopmentSeed__Enabled=false
```

Demo seed data is for development only.
