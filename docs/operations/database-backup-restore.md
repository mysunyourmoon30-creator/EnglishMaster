# Database Backup And Restore

## Backup Strategy

EnglishMaster uses SQL Server for application data. Production should have:

- Daily differential or full backups depending on database size and recovery needs.
- Weekly full backups at minimum.
- Transaction log backups if point-in-time recovery is required.
- Retention that covers accidental deletion discovery windows, commonly 14 to 35 days for operational backups.

## Safe Storage

Store backups outside the app container and outside the primary database host where possible. Restrict access to operators who need recovery permissions. Encrypt backups when the storage platform supports it.

## Verification

Every backup process should verify:

- Backup command completed successfully.
- Backup file exists and has non-zero size.
- Backup checksum or SQL Server `RESTORE VERIFYONLY` succeeds.
- Restore has been tested in staging or another non-production environment.

## Restore To Staging First

Before restoring production:

1. Restore the backup to staging or an isolated recovery database.
2. Start the app against the restored database.
3. Verify `/health/ready`.
4. Verify SuperAdmin login.
5. Verify admin dashboard, content lists, reports, notifications, and publishing pages.
6. Verify public pages do not expose unpublished content.

## Example SQL Server Commands

Backups and restores should be run by an operator with appropriate SQL Server permissions. Use environment-specific paths and do not store passwords in scripts.

```sql
BACKUP DATABASE [EnglishMaster]
TO DISK = N'<backup-directory>\EnglishMaster_full.bak'
WITH CHECKSUM, COMPRESSION;

RESTORE VERIFYONLY
FROM DISK = N'<backup-directory>\EnglishMaster_full.bak'
WITH CHECKSUM;
```
