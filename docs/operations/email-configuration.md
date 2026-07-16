# Email Configuration

## Current Behavior

EnglishMaster supports a configurable email provider behind the `IEmailSender` abstraction.

Supported providers:

- `Development`: logs recipient and subject information and does not send external email.
- `Smtp`: sends email through configured SMTP settings.

`Development` is the safe local default. Staging and production should use `Smtp` only when provider secrets are supplied by the deployment environment.

## Configuration

```json
{
  "Email": {
    "Provider": "Smtp",
    "FromEmail": "noreply@example.com",
    "FromName": "EnglishMaster",
    "Smtp": {
      "Host": "<smtp-host>",
      "Port": 587,
      "UseSsl": true,
      "UserName": "<smtp-username>",
      "Password": "<set-from-secret>"
    }
  }
}
```

Rules:

- Store SMTP passwords in deployment secrets or environment variables.
- Do not commit provider API keys or SMTP passwords.
- Keep provider-specific settings out of domain and application layers.
- Do not expose host, username, password, tokens, or provider payloads in API responses.
- Use the provider status endpoint to confirm safe configuration status.

## Verification

Verify configuration and delivery support with:

```text
GET  /api/v1/admin/email-provider/status
POST /api/v1/admin/email-provider/test-send
GET  /api/v1/admin/email-messages
POST /api/v1/admin/email-messages
POST /api/v1/admin/email-delivery/process
POST /api/v1/admin/email-delivery/{id}/retry
```

The status endpoint requires `email.read`. Test-send, queue processing, queueing, and retry require `email.manage`.

## Failure Handling

- Provider failures are stored as sanitized failure summaries.
- Failed email can be retried manually by an authorized admin.
- Queue processing is bounded and should be run with a small `MaxItems` value until staging behavior is validated.

## Rollback

Set `Email:Provider` to `Development` to disable external delivery without deleting queued email records.
