# Certificate System Operations

## Scope

This document describes v0.3.0 certificate template, generation, and public verification operations.

## Endpoints

| Endpoint | Permission | Purpose |
| --- | --- | --- |
| `GET /api/v1/admin/certificate-templates` | `certificates.read` | Search certificate templates. |
| `POST /api/v1/admin/certificate-templates` | `certificates.manage` | Create a template. |
| `PUT /api/v1/admin/certificate-templates/{id}` | `certificates.manage` | Update a template. |
| `POST /api/v1/admin/certificate-templates/{id}/activate` | `certificates.manage` | Enable a template for generation. |
| `POST /api/v1/admin/certificate-templates/{id}/deactivate` | `certificates.manage` | Disable a template for generation. |
| `GET /api/v1/me/certificates` | Authenticated | List current learner certificates. |
| `POST /api/v1/me/certificates/courses/{courseId}/generate` | Authenticated | Generate a completed-course certificate. |
| `GET /api/v1/public/certificates/{verificationCode}` | Public | Verify certificate validity. |

## Operational Checklist

- [ ] Confirm `certificates.read` and `certificates.manage` are present in `/api/v1/permissions`.
- [ ] Create or confirm at least one active template before enabling certificate generation.
- [ ] Verify a completed-course learner can generate a certificate in staging.
- [ ] Confirm duplicate generation returns the same verification code.
- [ ] Open `/certificates/verify/{code}` and confirm the public result.
- [ ] Confirm unknown verification codes return not found.
- [ ] Confirm revoked seeded/test certificates show as invalid, not missing.

## Template Safety

- Keep template text generic and stable.
- Use only supported placeholders.
- Deactivate old templates instead of deleting records.
- Avoid putting private learner data into template text.

## Rollback

If certificate generation must be paused, deactivate all certificate templates. Existing certificates remain available for learner history and public verification.
