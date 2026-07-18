# Production Monitoring

## Logging Review

EnglishMaster uses ASP.NET Core logging. Production logging should collect API and Web logs through the hosting platform or container runtime.

Logging rules:

- Do not log passwords, connection strings, tokens, provider credentials, or full cookies.
- Login failures may be logged as failed attempts without passwords.
- Import/export failures should include operation id, row number, and safe validation details where available.
- Publish job failures should include publish job id, status, and bounded error details.
- Email failures should include email message id, recipient, subject, and bounded error details, but not provider secrets.
- Critical exceptions should be captured by hosting logs and incident notes without exposing details to users.

## Health Monitoring

Use health endpoints for uptime checks and deployment validation:

| Endpoint | Purpose |
| --- | --- |
| `/health` | General health endpoint. |
| `/health/live` | Process liveness. |
| `/health/ready` | Readiness; API includes database connectivity. |

Database connectivity is checked by API readiness. Startup issues usually show up as failed container start, failed `/health/live`, failed `/health/ready`, or missing expected logs.

## Admin Health Page

**Implemented.** `/admin/system-health` (gated by the `system-health.read` permission) shows live database connectivity status and current failed-email/failed-publish-job/failed-import-job counts, backed by `GET /api/v1/admin/system-health`. Each page load performs a fresh on-demand check — it does not depend on the background worker's internal state.

## Alerting

**Implemented (self-contained, no external service):** `SystemHealthWorker` (`src/Backend/EnglishMaster.Infrastructure/Monitoring/SystemHealthWorker.cs`) polls DB connectivity and failed-job counts on a timer (default every 5 minutes) and queues an alert email — through the existing email queue/`EmailDeliveryWorker`, so no separate send path — when:
- DB connectivity fails `SystemHealthWorker__ConsecutiveFailuresBeforeAlert` times in a row (default 3), or
- failed email/publish-job/import-job counts cross their configured thresholds (defaults 10/5/5).

Each alert type has an independent cooldown (`SystemHealthWorker__AlertCooldown`, default 60 minutes) to avoid repeat-alert spam. Alerting is a no-op until `SystemHealthWorker__AlertRecipientEmail` is set — see `docs/deployment/production-environment-variables.md`.

**Still future, not built** (not requested, and would need a separate operations decision on tooling/budget per the caution below):

- Slack or Teams webhook alert for P0/P1 incidents.
- Cloud platform native uptime checks / external uptime monitor (e.g. UptimeRobot).
- Backup missing or verification failed alert (the backup/restore *mechanism* is verified — see `docs/testing/v0.3.0-backup-restore-verification.md` — but no automated check-and-alert exists for it yet).

Do not add paid services or a complex observability stack without a separate operations decision.
