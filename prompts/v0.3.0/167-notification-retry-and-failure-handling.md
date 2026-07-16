# Prompt 167 - Notification Retry And Failure Handling

## Goal

Add safe retry behavior for failed email delivery.

## Completed

- Added email lookup by ID.
- Added retry command for failed email messages.
- Added admin retry endpoint:
  - `POST /api/v1/admin/email-delivery/{id}/retry`
- Retry is limited to failed messages.
- Successful retry marks the email as `Sent`.
- Failed retry keeps the email `Failed` with sanitized failure text.

## Not Included

- Bulk retry.
- Automatic scheduled retry.
- Retry counters or backoff metadata.

## Next Prompt

Prompt 168 - Review Notification Delivery.
