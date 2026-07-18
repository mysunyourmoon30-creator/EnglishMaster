# V030-DEF-009 Scope: Certificate Admin UI

## Purpose

This is a scoping note, not an implementation plan for immediate execution. It exists so the release owner can decide, with concrete effort/impact in front of them, whether to build the certificate admin UI before tagging `v0.3.0` or ship the release with API-only certificate management. See `docs/testing/v0.3.0-defect-log.md` (V030-DEF-009) and `docs/release/v0.3.0-final-tag-prep.md` for the defect context.

## Current State

Backend is complete and verified (`src/Backend/EnglishMaster.Api/Endpoints/CertificateEndpoints.cs`):

| Route | Verb | Permission | Purpose |
| --- | --- | --- | --- |
| `/api/v1/admin/certificate-templates` | GET | `certificates.read` | Search/list templates |
| `/api/v1/admin/certificate-templates/{id}` | GET | `certificates.read` | Get one template |
| `/api/v1/admin/certificate-templates` | POST | `certificates.manage` | Create template |
| `/api/v1/admin/certificate-templates/{id}` | PUT | `certificates.manage` | Update template |
| `/api/v1/admin/certificate-templates/{id}/activate` | POST | `certificates.manage` | Activate template |
| `/api/v1/admin/certificate-templates/{id}/deactivate` | POST | `certificates.manage` | Deactivate template |
| `/api/v1/me/certificates` | GET | (any authenticated user) | List **my own** issued certificates |
| `/api/v1/me/certificates/courses/{courseId}/generate` | POST | (any authenticated user) | Generate a certificate for my completed course |
| `/api/v1/public/certificates/{verificationCode}` | GET | (anonymous) | Public verification |

DTOs (`src/Backend/EnglishMaster.Contracts/Certificates/CertificateContracts.cs`): `CertificateTemplateDto`, `CertificateTemplateSearchResponse`, `CreateCertificateTemplateRequest`, `UpdateCertificateTemplateRequest`, `IssuedCertificateDto`, `GenerateCourseCertificateRequest`, `PublicCertificateVerificationDto`.

Frontend has **no** admin UI for any of the `/admin/certificate-templates` routes. The only certificate-related page is `src/Frontend/EnglishMaster.Web/Components/Pages/Certificates/CertificateVerificationPage.razor`, which wraps the public verification endpoint only.

## Gap: No Admin-Wide Issued-Certificate Listing

There is currently no endpoint to list issued certificates across all students — only `/api/v1/me/certificates`, scoped to the calling user. If the release owner wants an admin view that browses certificates issued to any student (not just "my own"), that needs a **new backend query + endpoint + DTO + permission check** first. This is separate scope from template management and should be costed separately.

## What Template-CRUD-Only Would Take

This is a bounded, same-shape copy of an existing pattern already in the codebase — the Words admin CRUD (`src/Frontend/EnglishMaster.Web/Components/Pages/Words/WordList.razor`, `WordCreate.razor`, `WordEdit.razor`, plus a shared `WordForm.razor`, backed by `IWordsApiClient`/`WordsApiClient`).

1. **New API client** — `src/Frontend/EnglishMaster.Web/Services/Certificates/ICertificateTemplateApiClient.cs` + `CertificateTemplateApiClient.cs`, alongside the existing `CertificateVerificationApiClient` in the same folder. Wraps the six admin template routes above, following `IWordsApiClient`'s method shape (`SearchAsync`, `GetAsync`, `CreateAsync`, `UpdateAsync`, plus `ActivateAsync`/`DeactivateAsync`).
2. **New pages** — `src/Frontend/EnglishMaster.Web/Components/Pages/Certificates/Admin/`:
   - `CertificateTemplateList.razor` (`/admin/certificate-templates`) — search/filter table, activate/deactivate row actions.
   - `CertificateTemplateCreate.razor` and `CertificateTemplateEdit.razor`, both delegating to a shared `CertificateTemplateForm.razor` (code, name, description, body template, is-active).
3. **Nav + routes** — one `<NavLink>` entry in `src/Frontend/EnglishMaster.Web/Components/Layout/MainLayout.razor`, gated by the existing `HasPermission("certificates.read")` pattern already used for other nav entries; a new `Certificates` (or `CertificateTemplates`) static class in `src/Frontend/EnglishMaster.Web/Routes/AdminRoutes.cs` following the `Words`/`Courses` pattern.
4. Error handling for permission-denied automatically benefits from the V030-DEF-010 fix already applied (`ApiClientResponseHandler.cs`), so no extra work needed there.

**Effort**: low-medium — this is a template-copy job against an established pattern, no new backend work, no new permission model.

## What an Admin-Wide Issued-Certificate Browser Would Add

If the release owner also wants to browse *issued* certificates (not just manage templates) from the admin side:
- New backend: a query/repository method to list `IssuedCertificate` records across students (with pagination and likely a course/student filter), a new endpoint (e.g. `/api/v1/admin/certificates`) gated by a permission decision (reuse `certificates.read`, or introduce a narrower one), and a corresponding DTO.
- New frontend: a list page consuming it, likely with student/course lookups for display.

**Effort**: materially bigger than template CRUD — new backend surface, not just a UI copy.

## Recommendation

Two options for the release owner, in order of least to most scope:

1. **Ship v0.3.0 API-only for certificates.** Defer all admin UI. Backend is fully correct and already verified (see `docs/testing/v0.3.0-uat-execution-record.md`); certificate management stays a direct-API operation for this release.
2. **Build template-CRUD-only before tagging.** Closes the core of V030-DEF-009 (the ability to create/edit/activate templates without curling the API) at low-medium cost, using the exact pattern above. Defer the issued-certificates browser to a later release as a separate, explicitly-scoped item.

Building the full admin-wide issued-certificates browser now is not recommended as a pre-tag blocker for `v0.3.0` — it's the most expensive option and the underlying capability (certificate generation, ownership enforcement, public verification) already works and is verified.
