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
| `GET /api/v1/public/certificates/{verificationCode}` | Public, rate-limited (10 requests/minute per IP) | Verify certificate validity. |

## Operational Checklist

- [x] Confirm `certificates.read` and `certificates.manage` are present in `/api/v1/permissions` — verified 2026-07-18 via `CertificateTemplateEndpointsTests.CertificatePermissions_AreListed` (logs in, calls `GET /api/v1/permissions`, asserts both keys are present).
- [x] Create or confirm at least one active template before enabling certificate generation — verified 2026-07-18 via `CertificateTemplateEndpointsTests.CertificateTemplates_CanCreateSearchAndDeactivate` (creates a template, confirms `IsActive: true`, finds it via search).
- [x] Verify a completed-course learner can generate a certificate in staging — verified 2026-07-18 via `CertificateGenerationEndpointsTests.GenerateCourseCertificate_IssuesCertificateForCompletedCourse` (seeds a `Completed` course progress record, calls the real generate endpoint, asserts the issued certificate).
- [x] Confirm duplicate generation returns the same verification code — verified 2026-07-18 via `CertificateGenerationEndpointsTests.GenerateCourseCertificate_IsIdempotentForSameCourse` (calls generate twice, asserts identical certificate id and verification code).
- [x] Open `/certificates/verify/{code}` and confirm the public result — verified 2026-07-18 via `CertificateGenerationEndpointsTests.PublicCertificateVerification_ReturnsIssuedCertificateWithoutLogin` (unauthenticated call to the public endpoint, asserts course title and `IsValid: true`).
- [x] Confirm unknown verification codes return not found — verified 2026-07-18 via `CertificateGenerationEndpointsTests.PublicCertificateVerification_ReturnsNotFoundForUnknownCode` (asserts HTTP 404 for a code that was never issued).
- [x] Confirm revoked seeded/test certificates show as invalid, not missing — verified 2026-07-18 via `CertificateGenerationEndpointsTests.PublicCertificateVerification_MarksRevokedCertificateInvalid` (seeds a revoked certificate, asserts it is still found with `IsRevoked: true` and `IsValid: false`, not a 404).
- [x] Confirm the public verification endpoint is rate-limited — verified 2026-07-18: the first 10 calls in a window succeed normally, the 11th+ returns `429 Too Many Requests`. Registered via `AddRateLimiter`/`UseRateLimiter` in `src/Backend/EnglishMaster.Api/Program.cs`, applied to the endpoint in `CertificateEndpoints.cs`.

## Template Safety

- Keep template text generic and stable.
- Use only supported placeholders.
- Deactivate old templates instead of deleting records.
- Avoid putting private learner data into template text.

## Rollback

If certificate generation must be paused, deactivate all certificate templates. Existing certificates remain available for learner history and public verification.
