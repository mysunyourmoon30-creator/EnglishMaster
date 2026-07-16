# Prompt 165 - External Email Provider Implementation

## Goal

Implement the external email provider foundation without adding notification queue processing yet.

## Completed

- Added provider-neutral `EmailSendRequest`.
- Added safe `EmailProviderStatusDto` and provider status service abstraction.
- Added provider status endpoint:
  - `GET /api/v1/admin/email-provider/status`
- Added test-send endpoint:
  - `POST /api/v1/admin/email-provider/test-send`
- Added Infrastructure provider selection:
  - `Development`
  - `Smtp`
- Added SMTP adapter using configured host/port/from address and optional credentials.
- Kept provider secrets out of status responses.
- Updated API appsettings examples with non-secret email provider configuration shape.
- Added focused integration coverage for provider status, test send, and validation.

## Not Included

- Durable queue processor.
- Retry handling.
- Notification-to-email delivery integration.
- Provider health-check endpoint beyond status/test-send.

## Verification

- `dotnet build` passed.
- Notification integration tests passed.

## Next Prompt

Prompt 166 - Notification Delivery Queue.
