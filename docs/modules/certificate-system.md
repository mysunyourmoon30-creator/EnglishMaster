# Certificate System

## Overview

The certificate system provides template management, course-completion issuance, learner certificate history, and public verification for v0.3.0.

## Domain

- `CertificateTemplate` stores reusable certificate text, a unique code, active state, and timestamps.
- `IssuedCertificate` stores one certificate per learner/course pair, verification code, recipient snapshot, course title snapshot, rendered body, issued date, and revocation date.

The issued certificate stores display snapshots so public verification remains stable even if the learner name, course title, or template changes later.

## Application Flow

1. Admin creates and activates at least one certificate template.
2. Learner completes an active, published course.
3. Learner calls certificate generation for that course.
4. The application verifies completion and renders the selected active template.
5. The issued certificate can be listed by the learner and verified publicly by code.

## Persistence

`CertificateTemplates` indexes:

- Unique `Code`.
- `IsActive`.

`IssuedCertificates` indexes:

- Unique `VerificationCode`.
- Unique `UserId, CourseId`.
- `UserId, IssuedAt`.
- `CourseId`.

## UI

- `/certificates/verify` lets a public visitor enter a verification code.
- `/certificates/verify/{code}` opens verification results directly.

## Security

- Template management requires certificate permissions.
- Learner generation and history use the authenticated user id from claims.
- Public verification returns only public-safe certificate fields.
- Public verification does not expose internal ids, rendered body, user id, or course id.

## Known Limitations

- Admin revocation tooling is not yet exposed.
- Course-specific template assignment is not yet modeled.
- Certificate rendering is text-based; PDF/image generation can be added later without changing issued-certificate identity.
