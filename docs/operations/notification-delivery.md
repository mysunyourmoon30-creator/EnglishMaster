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

`POST /api/v1/admin/email-delivery/process` accepts:

```json
{
  "maxItems": 10
}
```

Rules:

- `maxItems` defaults to 10.
- `maxItems` is clamped between 1 and 50.
- Pending messages are processed oldest first.
- Successful sends become `Sent`.
- Provider failures become `Failed` with sanitized error text.

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
- [ ] Process a small queue batch first.
- [ ] Review failed email messages after queue processing.
- [ ] Retry only confirmed transient failures.
- [ ] Switch provider to `Development` if external delivery must be disabled.

## Rollback

Set:

```text
Email:Provider=Development
```

This stops external SMTP delivery while preserving queued email records for later review or retry.
