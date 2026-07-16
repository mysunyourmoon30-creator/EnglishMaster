# Content Migration Operations

## Purpose

Advanced import can be used for small, controlled content migration batches. It is designed for validation, preview, and auditability before content is inserted.

## Recommended Process

1. Prepare CSV or JSON content with stable headers.
2. Upload the content through `/admin/import-jobs/upload`.
3. Validate the job.
4. Review preview counts, rows, and validation errors.
5. Export errors if cleanup is needed.
6. Confirm only after all critical row errors are resolved.
7. Run the job.
8. Roll back only when the completed job created incorrect Word rows.

## CSV Rules

- First row must contain headers.
- Headers should match expected field names such as `Text`, `MeaningTh`, `Title`, `Slug`, or `PassingScore`.
- Avoid complex quoted values with commas until a stronger CSV parser is added.

## JSON Rules

- Use either one object or an array of objects.
- Property names should match the same field names used by CSV headers.
- Invalid JSON is rejected before a job is created.

## Adding A New Import Type

1. Add the normalized type to the upload allow-list.
2. Add validation logic in the import validation service.
3. Add a run implementation that creates or updates only the intended entity.
4. Record `CreatedEntityType` and `CreatedEntityId` for rollback when creation is supported.
5. Add rollback logic if the new type supports rollback.
6. Add API and UI tests for upload, validate, confirm, run, and rollback behavior.
7. Update this documentation and the security notes.

## Background Processing

Background processing is intentionally not included. Add it only after retry rules, cancellation semantics, progress storage, monitoring, and idempotent execution are designed.

## Next Recommended Step

Add dedicated run and rollback implementations for Categories, Tags, Lessons, Courses, Books, and Quizzes, starting with the lowest-risk content type.

