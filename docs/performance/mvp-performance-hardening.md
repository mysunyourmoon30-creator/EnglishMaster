# MVP Performance Hardening

## Purpose

This note captures the current MVP performance guardrails for EnglishMaster. It is not a redesign plan; it documents the lightweight constraints that keep admin operations predictable.

## Database Queries

- Common list and search queries use `AsNoTracking` for read-only access.
- Large module lists use page number and page size inputs.
- Page size defaults to 20 and is capped at 100 for the main paginated modules.
- Detail queries can load related records, but list queries should avoid large child collections unless the list UI needs them.
- Common filter and lookup fields are indexed through EF Core configuration.

## API Lists

Paginated list/search APIs exist for Words, Media, Pronunciations, Grammar Topics, Grammar Rules, Lessons, Courses, Books, Quizzes, Publish Jobs, Publish Templates, Published Artifacts, Users, and Roles.

Category and Tag searches remain small lookup-style responses in the current MVP. If they grow beyond simple admin lookups, add paginated response DTOs before using them for large datasets.

## Blazor Admin Lists

- Admin list pages should use lightweight search DTOs and avoid loading full graphs.
- Dropdowns that need lookup data should use bounded requests where the API supports page size.
- Loading, empty, and error states should remain in place so slow requests do not produce broken screens.

## Media and Files

- Media list views use metadata and public URLs.
- File bytes should only be read when an upload or download action explicitly requires it.
- Internal storage paths should not be exposed as public API fields.

## Import and Export

- Import files are capped at 1 MB.
- Uploaded content is parsed as data only.
- Export endpoints are intended for small MVP content operations and review, not bulk archival data movement.

## Publishing

- Publish Job and Published Artifact lists are paginated.
- Publishing runs synchronously in the MVP.
- Heavy rendering, retries, progress updates, and long-running artifact generation should be moved to a background worker in a future slice.

## Remaining Risks

- Category and Tag lookup APIs are not yet paginated.
- Some admin dropdowns intentionally use bounded lookup requests, but very large production catalogs will need dedicated typeahead endpoints.
- Export and publishing are synchronous and should stay limited to MVP-sized data until background processing is introduced.
