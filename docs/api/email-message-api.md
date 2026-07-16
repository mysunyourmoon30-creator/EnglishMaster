# Email Message API

## Admin Endpoints

| Method | Route | Purpose | Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/admin/email-messages` | Search queued email messages. | `email.read` |
| `POST` | `/api/v1/admin/email-messages` | Queue an email message. | `email.manage` |

Search supports status and recipient email filters.

## Queue Request

```json
{
  "toEmail": "learner@example.test",
  "toName": "Learner",
  "subject": "Welcome",
  "body": "Hello from EnglishMaster",
  "isHtml": false
}
```

## Safety Rules

- The development sender does not send real emails.
- Do not commit provider credentials.
- Do not log email bodies unless a dedicated operational policy allows it.
- Production delivery must be implemented behind `IEmailSender`.

## Known Limitations

- No public email API exists.
- No production provider integration exists yet.
- No retry worker or delivery webhook processing exists yet.
