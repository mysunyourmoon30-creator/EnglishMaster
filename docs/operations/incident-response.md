# Incident Response

## Severity Levels

| Severity | Meaning | Examples |
| --- | --- | --- |
| P0 | Complete outage or active data loss. | API down, database unavailable, destructive migration. |
| P1 | Major user-facing failure. | Admin login broken, public pages down, publishing broken. |
| P2 | Degraded important workflow. | Reports failing, email queue failing, import failures. |
| P3 | Minor issue or low-risk defect. | Documentation issue, isolated validation problem. |

## Who Responds

Assign an incident owner for every P0/P1. The owner coordinates diagnosis, communication, rollback decisions, and notes. Add subject-matter helpers for database, deployment, or application code as needed.

## What To Check First

1. Current deployment or configuration change.
2. `/health/live` and `/health/ready`.
3. API and Web startup logs.
4. Database connectivity.
5. Disk space for database, media, publishing, and logs.
6. Recent migrations.
7. Recent role/permission changes.
8. Failed publish jobs, imports, and email messages.

## Rollback

Rollback to the previous known-good application artifact when the incident is caused by a deployment. Restore the database only when schema or data integrity is affected. Restore file storage only when durable files were corrupted or deleted.

## Communication

For P0/P1:

- State what is affected.
- State whether data loss is suspected.
- Share the next update time.
- Avoid sharing secrets, internal stack traces, or customer-sensitive data.

## Root Cause Notes

Record:

- Timeline.
- Detection source.
- Impact.
- Release or configuration involved.
- Backup and restore actions.
- Rollback actions.
- Final fix.
- Follow-up prevention tasks.

## Closeout

An incident is closed when health checks pass, core workflows are verified, users are informed if needed, and follow-up work is tracked.
