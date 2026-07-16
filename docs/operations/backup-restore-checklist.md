# Backup Restore Checklist

## Daily Checks

- [ ] Latest database backup exists.
- [ ] Backup file size is plausible.
- [ ] Backup verification succeeded.
- [ ] File storage backup completed for media.
- [ ] File storage backup completed for publishing artifacts.
- [ ] Backup logs contain no secrets.

## Weekly Checks

- [ ] Full database backup completed.
- [ ] Restore tested in staging or isolated recovery environment.
- [ ] Admin login verified after restore.
- [ ] Public pages verified after restore.
- [ ] Media and publishing files verified after restore.

## Before Migration

- [ ] Full database backup taken.
- [ ] File storage backup current.
- [ ] Rollback plan documented.
- [ ] Release artifact identified.
- [ ] Operator confirms staging migration test passed.

## After Restore

- [ ] `/health/live` passes.
- [ ] `/health/ready` passes.
- [ ] SuperAdmin login works.
- [ ] Admin dashboard loads.
- [ ] Reports load.
- [ ] Notifications load.
- [ ] Public pages load.
- [ ] Media links load.
- [ ] Publishing artifact links load.

## Blockers

Do not proceed with production changes if backups are missing, restore has never been tested, durable file storage is unavailable, or operators cannot access required secrets safely.
