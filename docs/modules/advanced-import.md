# Advanced Import Validation

## Purpose

Advanced Import Validation adds a safer admin workflow for bringing content into EnglishMaster. It records an import job, stores each parsed row, validates rows before import, shows preview counts, records row-level errors, and supports rollback for imported Word rows.

The module extends the existing import/export area. It does not replace the basic import/export module and does not add AI, marketplace, mobile, payment, microservices, or background processing.

## Supported Import Types

- `words`
- `categories`
- `tags`
- `grammartopics`
- `grammarrules`
- `lessons`
- `courses`
- `books`
- `quizzes`

## Supported Formats

- `CSV`
- `JSON`

CSV input uses the first row as headers. Each following row is converted to JSON and stored as an `ImportJobRow`. The current parser is simple and does not support complex quoted comma scenarios.

JSON input may be a single object or an array of objects.

## Lifecycle

```text
Uploaded
  -> Validating
    -> PreviewReady
    -> ValidationFailed
PreviewReady
  -> Confirmed
Confirmed
  -> Importing
    -> Completed
    -> Failed
Completed
  -> RolledBack
Any non-completed/non-rolled-back state
  -> Cancelled
```

Validation can only start from `Uploaded`. Confirmation can only happen from `PreviewReady`. Import can only run from `Confirmed`. Rollback can only run from `Completed`.

## Validation Flow

The upload endpoint stores an import job and rows. Validation reads the stored rows and marks each row `Valid` or `Invalid`.

Current validation checks include:

- Words: required `Text`, required `MeaningTh`, duplicate `Text`, valid `CefrLevel`.
- Categories: required `Name`, duplicate `Name` or `Slug`.
- Tags: required `Name`, duplicate `Name` or `Slug`.
- Grammar topics: required `Title`, duplicate `Title`.
- Grammar rules: required `Title`, required `RuleText`, duplicate `Title`, optional `ExampleEn` if supplied.
- Lessons: required `Title`, duplicate `Title` or `Slug`, non-negative `EstimatedMinutes`.
- Courses: required `Title`, duplicate `Title` or `Slug`, non-negative `EstimatedMinutes`.
- Books: required `Title`, duplicate `Title` or `Slug`, non-negative `EstimatedPages`.
- Quizzes: required `Title`, `PassingScore` from 0 to 100.

## Preview, Confirm, And Run

After successful validation, the job becomes `PreviewReady`. Admin users can inspect counts and rows, then confirm the import. Confirmed imports can be run.

The first execution implementation imports valid Word rows. Other import types currently validate and preview, but their run behavior is intentionally blocked with row failures until dedicated import runners are added.

## Rollback

Rollback currently removes Word records created by the completed import job. It uses the per-row `CreatedEntityType` and `CreatedEntityId` values and does not delete records unrelated to the job.

## Why No Background Processing Yet

The first implementation is synchronous so validation, preview, run, and rollback behavior are clear and testable. Background processing should wait until retry rules, cancellation behavior, progress monitoring, idempotency, and operational alerts are defined.

## Known Limitations

- Run and rollback are fully implemented for Words first.
- CSV parsing is intentionally simple.
- The admin upload page posts pasted CSV/JSON content instead of multipart file storage.
- Large file processing is limited to 1 MB content per request.
- No background queue, scheduled migration runner, or retry dashboard is included yet.

