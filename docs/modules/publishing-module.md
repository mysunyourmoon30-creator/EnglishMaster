# Publishing Module

## Purpose

The Publishing Module manages export jobs for existing EnglishMaster content and prepares generated artifacts for download or later distribution. It is intentionally a foundation slice: it models jobs, templates, artifacts, storage, and admin workflows without adding AI generation, marketplace publishing, payment, mobile, plugins, or microservices.

Supported source types:

- `Lesson`
- `Course`
- `Book`
- `Quiz`

Supported formats:

- `Html`
- `Markdown`
- `Pdf`
- `Docx`

## Domain Model

The module contains three domain entities:

- `PublishJob`: tracks a requested export and its lifecycle.
- `PublishTemplate`: stores reusable template metadata per format.
- `PublishedArtifact`: records generated output files for completed jobs.

Detailed field-level docs:

- [publish-job.md](publish-job.md)
- [publish-template.md](publish-template.md)
- [published-artifact.md](published-artifact.md)

## Status Lifecycle

`PublishJob` starts as `Pending`.

Valid lifecycle operations:

- `MarkRunning`: moves a pending or failed job to `Running`.
- `MarkCompleted`: moves a running job to `Completed`, sets `CompletedAt`, `OutputFileName`, and `OutputPath`.
- `MarkFailed`: moves a non-completed job to `Failed`, sets `ErrorMessage`, and sets `CompletedAt`.
- `Cancel`: moves a non-completed job to `Cancelled` and sets `CompletedAt`.

Completed jobs cannot be cancelled, failed, or started again.

## Application Abstractions

Publishing uses application-layer abstractions so infrastructure details do not leak inward:

- `IPublishingService`: coordinates running a publish job.
- `IPublishContentBuilder`: builds export content for a source and format.
- `IPublishFileStorage`: stores generated output and returns file metadata.

The current `PublishingService` lives in the Application layer and depends on repository/storage/content abstractions. EF Core repositories, local storage, and basic content building live in Infrastructure.

## Local Storage

Generated output is stored under a configurable local folder.

- Configuration key: `Publishing:LocalStoragePath`
- Default folder: `AppContext.BaseDirectory/publishing`
- Public request path: `/publishing`

Stored artifact DTOs expose relative paths and public URLs, not absolute server file paths.

## File Security Rules

- File names are sanitized with `Path.GetFileName`.
- Invalid file-name characters are replaced.
- Storage validates generated paths with `Path.GetRelativePath`.
- Files must remain under the configured publishing root.
- API DTOs do not expose internal absolute server paths.

## Export Behavior

Current implementation:

- `Html`: creates a basic HTML document.
- `Markdown`: creates a basic Markdown document.
- `Pdf`: creates a placeholder `.pdf.txt` text file.
- `Docx`: creates a placeholder `.docx.txt` text file.

PDF and DOCX rendering are intentionally placeholders until a rendering library is selected and added deliberately.

## API

Main API documentation:

- [publishing-api.md](../api/publishing-api.md)

Route groups:

- `/api/v1/publish-jobs`
- `/api/v1/publish-templates`
- `/api/v1/published-artifacts`

## Blazor Admin Pages

- `/admin/publishing/jobs`
- `/admin/publishing/jobs/create`
- `/admin/publishing/jobs/{id:guid}`
- `/admin/publishing/templates`
- `/admin/publishing/templates/create`
- `/admin/publishing/templates/{id:guid}/edit`
- `/admin/publishing/artifacts`

The admin navigation includes Publishing, and the dashboard includes Total Publish Jobs.

Publishing list pages use paginated API responses. Job and artifact detail pages should load a single record rather than expanding large related collections in list views.

## Test Coverage

- `PublishJob` creation and lifecycle transitions.
- Failed-job validation.
- Completed-job cancel/run protection.
- Rerun from failed job to completed job.
- `PublishTemplate` creation and slug generation.
- `PublishedArtifact` file-size invariant.
- Search publish jobs by status.
- API integration flow for creating/running jobs, reading artifacts, and template CRUD.
- Existing architecture, unit, and integration tests still pass.

## Known Limitations

- PDF and DOCX rendering are placeholders.
- Exported HTML and Markdown content is intentionally minimal.
- Publish jobs run synchronously through the API; there is no background queue yet.
- Publishing is suitable for MVP-sized artifacts. Heavy rendering, retries, progress events, and worker-based execution are follow-up work.
- Publishing admin routes and APIs are protected by the existing authentication and permission model.
- Templates are stored but not yet used by the basic content builder.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
