# Prompt 166 - Notification Delivery Queue

## Goal

Add bounded delivery queue processing for pending email messages.

## Completed

- Added repository query for pending email messages.
- Added `ProcessPendingEmailQueueCommand`.
- Added bounded queue processing with a default limit of 10 and maximum of 50.
- Added admin endpoint:
  - `POST /api/v1/admin/email-delivery/process`
- The queue processor sends pending email through the configured `IEmailSender`.
- Sent messages are marked `Sent`.
- Provider failures are sanitized and marked `Failed`.

## Not Included

- Retry failed email.
- Automatic background processing.
- Notification-to-email enqueue rules.

## Next Prompt

Prompt 167 - Notification Retry And Failure Handling.
