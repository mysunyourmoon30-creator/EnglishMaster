# Certificate API

## Purpose

The certificate API supports v0.3.0 certificate templates, course-completion certificate issuance, learner certificate history, and public verification.

## Permissions

| Permission | Use |
| --- | --- |
| `certificates.read` | Search and view certificate templates from admin tooling. |
| `certificates.manage` | Create, update, activate, and deactivate certificate templates. |

Learner certificate generation and history require authentication. Public verification does not require authentication.

## Endpoints

```text
GET  /api/v1/admin/certificate-templates
GET  /api/v1/admin/certificate-templates/{id}
POST /api/v1/admin/certificate-templates
PUT  /api/v1/admin/certificate-templates/{id}
POST /api/v1/admin/certificate-templates/{id}/activate
POST /api/v1/admin/certificate-templates/{id}/deactivate

GET  /api/v1/me/certificates
POST /api/v1/me/certificates/courses/{courseId}/generate

GET  /api/v1/public/certificates/{verificationCode}
```

## Template Management

Create a template:

```json
{
  "code": "course-completion",
  "name": "Course Completion",
  "description": "Issued when a learner completes a course.",
  "bodyTemplate": "{{student}} completed {{course}} on {{issuedAt}}."
}
```

Supported placeholders:

```text
{{student}}
{{studentName}}
{{course}}
{{courseTitle}}
{{issuedAt}}
```

Template `code` is unique. Deactivated templates cannot be used for new certificate generation.

## Certificate Generation

Generate a course certificate:

```text
POST /api/v1/me/certificates/courses/{courseId}/generate
```

Request:

```json
{
  "templateId": "00000000-0000-0000-0000-000000000000"
}
```

Rules:

- The request uses the authenticated user from the current session.
- The course must be active, published, and completed by the current user.
- Course progress must be `Completed` with `ProgressPercent >= 100`.
- Generation is idempotent per user and course. A second request returns the existing certificate.
- If `templateId` is omitted, the first active template is used.

## Public Verification

Verify a certificate:

```text
GET /api/v1/public/certificates/{verificationCode}
```

Public response:

```json
{
  "verificationCode": "cert-abc123",
  "recipientName": "Learner Name",
  "courseTitle": "Course Title",
  "templateCode": "course-completion",
  "issuedAt": "2026-07-16T00:00:00+00:00",
  "isRevoked": false,
  "isValid": true
}
```

Unknown verification codes return `404 Not Found`. Revoked certificates remain discoverable but return `isValid: false`.
