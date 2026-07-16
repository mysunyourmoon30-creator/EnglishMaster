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

No dedicated system health admin dashboard exists yet. Keep this as a TODO unless a future prompt adds a small read-only operations page.

## Alerting TODO

Future simple alerting options:

- Email alert when `/health/ready` fails.
- Slack or Teams webhook alert for P0/P1 incidents.
- Cloud platform uptime checks.
- External uptime monitor.
- Failed publish job count threshold.
- Failed email message count threshold.
- Backup missing or verification failed alert.

Do not add paid services or a complex observability stack without a separate operations decision.
