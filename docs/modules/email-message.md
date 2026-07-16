# Email Message

## Purpose

`EmailMessage` is the durable queue record for future email delivery. The first implementation is intentionally provider-neutral and uses a development sender so EnglishMaster can build notification workflows before choosing a production email provider.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Email message identifier. |
| `ToEmail` | Recipient email address. |
| `ToName` | Optional recipient display name. |
| `Subject` | Email subject. |
| `Body` | Email body. |
| `IsHtml` | Whether the body should be treated as HTML. |
| `Status` | Queue and delivery state. |
| `SentAt` | When delivery was marked sent. |
| `FailedAt` | When delivery was marked failed. |
| `ErrorMessage` | Bounded failure detail. |
| `CreatedAt` / `UpdatedAt` | Audit timestamps. |

## Status Values

- `Pending`
- `Sent`
- `Failed`

## Development Sender Behavior

The development sender logs that an email was queued for a recipient and subject. It does not contact SMTP, SendGrid, SES, or any other external email provider. It does not require provider credentials.

## Why A Real Provider Is Not Included Yet

Real provider integration needs environment-specific credentials, retry policies, delivery webhooks, bounce handling, and operational monitoring. Those concerns should be added after the core queue model and permission boundaries are stable.

## Known Limitations

- No background worker sends queued records yet.
- No provider-specific message id is stored yet.
- No retry schedule or dead-letter process is included yet.
