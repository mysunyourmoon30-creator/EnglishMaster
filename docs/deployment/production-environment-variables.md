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

## Web Variables

| Variable | Required | Purpose |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Yes | Set to `Production`. |
| `ASPNETCORE_URLS` | Platform-specific | Bind address inside the host/container. |
| `AllowedHosts` | Yes | Production Web host names. Avoid `*` in production. |
| `ApiBaseUrl` | Yes | Production HTTPS API base URL. |

## Secret Rules

- Do not commit production connection strings, SQL passwords, bootstrap credentials, or provider credentials.
- Store secrets in the hosting platform secret manager, environment variables, or a vault.
- Remove temporary SuperAdmin bootstrap credentials after first setup.
