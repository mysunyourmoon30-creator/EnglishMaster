# Production Operations Checklist

## Daily Checks

- [ ] API `/health/live` responds.
- [ ] API `/health/ready` responds.
- [ ] Web `/health/live` responds.
- [ ] Database connectivity is healthy.
- [ ] Disk space is healthy for database, media, publishing, and logs.
- [ ] Latest database backup exists and verification succeeded.
- [ ] Latest file storage backup exists.
- [ ] Error logs have no new critical exceptions.
- [ ] Failed publish jobs reviewed.
- [ ] Failed email messages reviewed.
- [ ] Email provider status checked.
- [ ] Certificate templates active and public verification spot-check completed.
- [ ] Failed imports reviewed.
- [ ] Login/authentication issues reviewed.
- [ ] Admin dashboard loads.
- [ ] Admin analytics dashboard loads.
- [ ] Student analytics dashboard loads for an authenticated learner.
- [ ] Public learning pages load.

## Weekly Checks

- [ ] Full backup completed.
- [ ] Restore test completed in staging or isolated recovery environment.
- [ ] SuperAdmin access verified.
- [ ] Permission changes reviewed.
- [ ] Dependency/security advisories reviewed.
- [ ] Go-live checklist remains current after changes.

## After Deployment

- [ ] Confirm release version or commit identifier.
- [ ] Confirm migrations applied.
- [ ] Confirm `/health/ready`.
- [ ] Confirm login.
- [ ] Confirm admin content list.
- [ ] Confirm reports page.
- [ ] Confirm analytics pages if analytics code changed.
- [ ] Confirm notifications page.
- [ ] Confirm email provider status.
- [ ] Send staging/production-approved email test if provider delivery changed.
- [ ] Confirm certificate generation and public verification if certificate migrations changed.
- [ ] Confirm public pages.
- [ ] Watch logs for startup exceptions.

## Escalate When

- `/health/ready` fails for more than one check window.
- Database backup is missing or failed.
- Disk space is close to exhaustion.
- Login fails for all admins.
- Publish jobs fail repeatedly.
- Unexpected 500 errors increase.
- Analytics dashboards fail or become noticeably slow.
- Media or publishing files disappear.
- Public certificate verification fails for valid issued certificates.
