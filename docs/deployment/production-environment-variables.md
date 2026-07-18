# Production Environment Variables

## API Variables

| Variable | Required | Purpose |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Yes | Set to `Production`. |
| `ASPNETCORE_URLS` | Platform-specific | Bind address inside the host/container. |
| `AllowedHosts` | Yes | Production API host names. Avoid `*` in production. |
| `ConnectionStrings__DefaultConnection` | Yes | Production SQL Server connection string from secrets. |
| `DevelopmentSeed__Enabled` | Yes | Set to `false`. |
| `Auth__InitialSuperAdmin__Email` | Temporary | Optional one-time bootstrap email. Remove after setup. |
| `Auth__InitialSuperAdmin__Password` | Temporary | Optional one-time bootstrap password. Rotate/remove after setup. |
| `Media__LocalStoragePath` | Yes | Durable uploaded media storage path. |
| `Publishing__LocalStoragePath` | Yes | Durable published artifact storage path. |
| `Logging__FilePath` | No | Directory for rolling structured log files (Serilog, one file per day, 14-day retention). Defaults to a `logs` folder next to the app binary if unset — point this at durable storage in production so logs survive container restarts. Logs also always go to the console for platform-level log collection. |
| `Email__Provider` | Yes for production | Set to `Smtp` to send real email via the SMTP adapter (`src/Backend/EnglishMaster.Infrastructure/Notifications/SmtpEmailSender.cs`, MailKit-based). Defaults to `Development` (log-only, no real send) if unset — leaving it unset in production means no email is actually delivered. |
| `Email__FromEmail` | Yes if `Provider=Smtp` | Sender address used on outgoing mail. |
| `Email__FromName` | No | Sender display name. Defaults to `EnglishMaster`. |
| `Email__Smtp__Host` | Yes if `Provider=Smtp` | SMTP server host. |
| `Email__Smtp__Port` | Yes if `Provider=Smtp` | SMTP server port. Defaults to `587`. |
| `Email__Smtp__UseSsl` | No | Defaults to `true` (STARTTLS when available). |
| `Email__Smtp__UserName` | Yes if `Provider=Smtp` and auth required | SMTP username, from secrets. |
| `Email__Smtp__Password` | Yes if `Provider=Smtp` and auth required | SMTP password, from secrets. |
| `EmailDeliveryWorker__Enabled` | No | Background worker that automatically drains the pending email queue. Defaults to `true`. Set to `false` to require manual triggering via `POST /api/v1/admin/email-delivery/process` only. |
| `EmailDeliveryWorker__PollingInterval` | No | How often the worker checks for pending messages, as a `TimeSpan` string (e.g. `00:01:00` for 60 seconds). Defaults to 60 seconds. |
| `SystemHealthWorker__Enabled` | No | Background worker that checks DB connectivity and failed email/publish/import job counts, sending an alert email when thresholds are crossed. Defaults to `true`. |
| `SystemHealthWorker__PollingInterval` | No | How often the worker checks system health, as a `TimeSpan` string. Defaults to `00:05:00` (5 minutes). |
| `SystemHealthWorker__ConsecutiveFailuresBeforeAlert` | No | Consecutive DB connectivity failures before an alert fires. Defaults to `3`. |
| `SystemHealthWorker__FailedEmailCountThreshold` | No | Failed email count that triggers an alert. Defaults to `10`. |
| `SystemHealthWorker__FailedPublishJobCountThreshold` | No | Failed publish job count that triggers an alert. Defaults to `5`. |
| `SystemHealthWorker__FailedImportJobCountThreshold` | No | Failed import job count that triggers an alert. Defaults to `5`. |
| `SystemHealthWorker__AlertRecipientEmail` | Yes for alerting | Where alert emails are sent. Empty by default, which means **alerting is a no-op until this is set** — the worker still checks health but never sends anything without a recipient configured. |
| `SystemHealthWorker__AlertCooldown` | No | Minimum time between repeat alerts of the same type, as a `TimeSpan` string. Defaults to `01:00:00` (60 minutes), to avoid alert spam. |

### Gmail SMTP quick reference (see `docs/operations/email-configuration.md` for full setup steps)

| Key | Value |
| --- | --- |
| `Email__Provider` | `Smtp` |
| `Email__Smtp__Host` | `smtp.gmail.com` |
| `Email__Smtp__Port` | `587` |
| `Email__Smtp__UseSsl` | `true` |
| `Email__Smtp__UserName` / `Email__FromEmail` | your Gmail address |
| `Email__Smtp__Password` | a Gmail **App Password** (not your account password) — generate at https://myaccount.google.com/apppasswords, requires 2-Step Verification enabled first |

## Web Variables

| Variable | Required | Purpose |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Yes | Set to `Production`. |
| `ASPNETCORE_URLS` | Platform-specific | Bind address inside the host/container. |
| `AllowedHosts` | Yes | Production Web host names. Avoid `*` in production. |
| `ApiBaseUrl` | Yes | Production HTTPS API base URL. |
| `Logging__FilePath` | No | Directory for rolling structured log files. Same behavior as the API's variable of the same name above. |

## Secret Rules

- Do not commit production connection strings, SQL passwords, bootstrap credentials, or provider credentials.
- Store secrets in the hosting platform secret manager, environment variables, or a vault.
- Remove temporary SuperAdmin bootstrap credentials after first setup.
