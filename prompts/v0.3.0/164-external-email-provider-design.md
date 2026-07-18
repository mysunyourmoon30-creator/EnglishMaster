# Prompt 164 - External Email Provider Design

## Goal

Design the v0.3.0 external email provider foundation before implementation.

## Completed

- Reviewed existing email/notification foundation.
- Confirmed `IEmailSender`, `EmailMessage`, repository, admin queue endpoint, and permissions already exist.
- Designed provider status, provider selection, SMTP configuration, test-send behavior, and secret handling.
- Added `docs/architecture/v0.3.0-external-email-provider-design.md`.

## Design Decision

Extend the existing email foundation:

- Keep Application abstractions provider-neutral.
- Keep SMTP/provider implementation in Infrastructure.
- Keep Development sender as the default fallback.
- Add status/test-send without exposing secrets.

## Next Prompt

Prompt 165 - External Email Provider Implementation.
