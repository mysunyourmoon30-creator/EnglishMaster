# Notification Delivery Operations

## Scope

This document describes v0.3.0 notification email delivery operations.

## Endpoints

| Endpoint | Permission | Purpose |
| --- | --- | --- |
| `GET /api/v1/admin/email-provider/status` | `email.read` | View safe provider status. |
| `POST /api/v1/admin/email-provider/test-send` | `email.manage` | Send a direct provider test email. |
| `GET /api/v1/admin/email-messages` | `email.read` | Search queued, sent, and failed email messages. |
| `POST /api/v1/admin/email-messages` | `email.manage` | Queue an email message. |
| `POST /api/v1/admin/email-delivery/process` | `email.manage` | Process pending email messages. |
| `POST /api/v1/admin/email-delivery/{id}/retry` | `email.manage` | Retry one failed email message. |

## Queue Processing

Pending messages are drained two ways:

1. **Automatic (background worker).** `EmailDeliveryWorker` (`src/Backend/EnglishMaster.Infrastructure/Notifications/EmailDeliveryWorker.cs`) runs inside the API process and polls the pending queue on a timer — no manual trigger needed in any long-running environment. Configure via `EmailDeliveryWorker__Enabled` (default `true`) and `EmailDeliveryWorker__PollingInterval` (default `00:01:00`, i.e. 60 seconds); see `docs/deployment/production-environment-variables.md`.
2. **Manual (admin endpoint).** `POST /api/v1/admin/email-delivery/process` accepts:

```json
{
  "maxItems": 10
}
```

Useful for processing immediately (e.g. right after seeding a test fixture) rather than waiting for the next automatic tick.

Rules (apply to both paths — the worker calls the same command handler):

- `maxItems` defaults to 10.
- `maxItems` is clamped between 1 and 50.
- Pending messages are processed oldest first.
- Successful sends become `Sent`.
- Provider failures become `Failed` with sanitized error text.
- Neither path auto-retries `Failed` messages — failures require a deliberate retry (see below), by design.

## Retry

Use retry only for failed messages:

```text
POST /api/v1/admin/email-delivery/{id}/retry
```

Retry outcomes:

- Success: email becomes `Sent`, `SentAt` is set, and error text is cleared.
- Failure: email remains `Failed` with a sanitized failure summary.
- Non-failed message: request is rejected.

## Security Notes

- Provider secrets are not returned by the status endpoint.
- Provider exceptions are sanitized before being returned or persisted.
- Test-send validates recipient email format before invoking the provider.
- Admin operations require `email.read` or `email.manage`.

## Operational Checklist

- [ ] Confirm provider status after deployment.
- [ ] Send a test email from staging before production go-live.
- [ ] Confirm the background worker is enabled (`EmailDeliveryWorker__Enabled=true`) and its polling interval is appropriate for expected volume.
- [ ] Process a small queue batch manually first, before relying on the automatic worker, to validate the provider end-to-end.
- [ ] Review failed email messages after queue processing.
- [ ] Retry only confirmed transient failures.
- [ ] Switch provider to `Development` if external delivery must be disabled.

## Rollback

Set:

```text
Email:Provider=Development
```

This stops external SMTP delivery while preserving queued email records for later review or retry. To also stop the background worker from ticking (e.g. during an incident), set `EmailDeliveryWorker__Enabled=false` — pending messages remain queued and can still be processed manually via `POST /api/v1/admin/email-delivery/process` once ready.
