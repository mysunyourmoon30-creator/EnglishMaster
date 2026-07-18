# Email Configuration

## Current Behavior

EnglishMaster supports a configurable email provider behind the `IEmailSender` abstraction.

Supported providers:

- `Development`: logs recipient and subject information and does not send external email.
- `Smtp`: sends email through configured SMTP settings.

`Development` is the safe local default. Staging and production should use `Smtp` only when provider secrets are supplied by the deployment environment.

The `Smtp` provider is implemented by `SmtpEmailSender` (`src/Backend/EnglishMaster.Infrastructure/Notifications/SmtpEmailSender.cs`), built on MailKit.

## Gmail SMTP Quick Setup

Gmail SMTP is a reasonable fit for low email volume (personal/small projects, not high-throughput production). It cannot be set up by an AI agent — creating the Google account, enabling 2-Step Verification, and generating the App Password are all account actions only the account owner can perform, and the password itself should never be pasted into a chat session or any tool that isn't your own terminal.

1. On the Gmail account that will send mail, enable **2-Step Verification** if it isn't already on: https://myaccount.google.com/security
2. Generate an **App Password**: https://myaccount.google.com/apppasswords — choose "Mail" (or "Other" and name it "EnglishMaster"). Google will show a 16-character password once; copy it.
3. Set these directly in your own terminal/environment (never share the password in chat):

   | Key | Value |
   | --- | --- |
   | `Email__Provider` | `Smtp` |
   | `Email__FromEmail` | your Gmail address |
   | `Email__Smtp__Host` | `smtp.gmail.com` |
   | `Email__Smtp__Port` | `587` |
   | `Email__Smtp__UseSsl` | `true` |
   | `Email__Smtp__UserName` | your Gmail address |
   | `Email__Smtp__Password` | the 16-character App Password (not your regular Gmail password) |

4. Verify with `GET /api/v1/admin/email-provider/status` (`Provider: "Smtp"`, `IsConfigured: true`) and `POST /api/v1/admin/email-provider/test-send` to confirm real delivery — both endpoints work without exposing the App Password back to whoever calls them.

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
