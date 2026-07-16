# Restore Procedure

## Restore Order

1. Stop or isolate the affected production app.
2. Restore the SQL Server database backup.
3. Restore uploaded media files to `Media__LocalStoragePath`.
4. Restore published artifacts to `Publishing__LocalStoragePath`.
5. Apply EF Core migrations if restoring to a newer app version.
6. Start the API and Web apps.
7. Verify `/health/live` and `/health/ready`.
8. Verify SuperAdmin login.
9. Verify admin pages for content, reports, notifications, users, and publishing.
10. Verify public pages render only published content.

## Migration Considerations

If the restored database is older than the deployed application schema, apply migrations before allowing normal traffic. If a migration caused the incident, follow the disaster recovery runbook migration-failure section.

## Validation Checks

- API `/health/ready` returns healthy.
- Web `/health/live` returns healthy.
- Login works for an authorized admin.
- Media previews load from `/media`.
- Published artifacts load from `/publishing`.
- Reports and notification pages load without server errors.

## Rollback

Rollback may require both application artifact rollback and database restore. Keep the release version, migration list, and backup timestamp together in the incident notes.
