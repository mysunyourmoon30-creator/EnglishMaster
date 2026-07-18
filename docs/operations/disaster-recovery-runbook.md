# Disaster Recovery Runbook

## Database Failure

1. Confirm whether the issue is connectivity, corruption, full disk, or bad migration.
2. Stop write traffic if data integrity is at risk.
3. Restore the latest verified backup to staging first.
4. Restore production only after the recovery point and impact are approved.
5. Verify `/health/ready`, login, admin pages, public pages, and core workflows.

## File Storage Failure

1. Identify affected storage path: media, publishing artifacts, or retained import/export files.
2. Stop uploads or publishing if writes may worsen the problem.
3. Restore files from backup to the configured durable path.
4. Verify files referenced by database records are present.
5. Check `/media` and `/publishing` links.

## App Container Failure

1. Check recent deployment and configuration changes.
2. Restart the failed container or service.
3. Verify health endpoints.
4. Roll back to the previous release artifact if restart does not recover.

## Migration Failure

1. Stop the deployment.
2. Capture logs and migration name.
3. Restore the pre-migration database backup to staging.
4. Fix the migration or app code in a new release.
5. Restore production from backup only if data integrity was affected.

For the current release, check `docs/release/v0.3.0-rollback-plan.md` first if the failing migration is one of the three FK cascade-path fixes from this cycle (`AddMediaModule`, `AddPronunciationModule`, `AddQuizModule`) — do not attempt a down-migration through them without reading that caution.

## Bad Deployment Rollback

1. Disable traffic to the bad release.
2. Deploy the previous known-good release artifact.
3. Restore database only if the bad release changed schema or data incompatibly.
4. Restore files only if the bad release corrupted durable storage.

## Lost Admin Access

1. Verify whether the issue is authentication, role assignment, cookie/session, or user status.
2. Use a controlled operational process to restore a SuperAdmin account.
3. If bootstrap settings are used, remove them after recovery and rotate the password.
4. Audit role and permission changes.

## Secret And Key Rotation

Rotate secrets after suspected exposure, operator change, or recovery event. This includes SQL passwords, bootstrap credentials, future email provider credentials, certificates, and deployment tokens.

## Incident Notes

Record timeline, affected release, database backup timestamp, file backup timestamp, operator actions, validation results, and follow-up tasks.
