# Email Configuration

## Current Behavior

EnglishMaster currently uses a development email sender. It logs queued recipient and subject information and does not send email through a real provider.

No SMTP, SendGrid, SES, Mailgun, or other provider settings are required for the current implementation.

## Production Provider Not Included Yet

Production email requires provider selection, secret storage, retry behavior, bounce handling, delivery monitoring, and environment-specific operations. Those should be added in a later prompt behind the existing `IEmailSender` abstraction.

## Future Configuration Guidance

When a real provider is added:

- Store credentials in deployment secrets or environment variables.
- Do not commit provider API keys or SMTP passwords.
- Keep provider-specific settings out of domain and application layers.
- Add health checks or operational diagnostics without exposing secrets.
- Define retry and dead-letter behavior before enabling automated sends.

## Verification

For the current foundation, verify email support by checking:

```text
GET  /api/v1/admin/email-messages
POST /api/v1/admin/email-messages
```

Both endpoints require admin permissions.
