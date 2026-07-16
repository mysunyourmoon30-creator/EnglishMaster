Create Advanced Import Validation and Content Migration Tools for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Content Review Workflow completed.
- Content Quality / QA System completed.
- Content Versioning completed.
- Content Bulk Operations completed.
- Basic Import / Export already exists.

Goal:
Improve content import safety with validation, preview, import history, and rollback support.

Important:
Do not add AI.
Do not add marketplace.
Do not add mobile.
Do not add payment.
Do not add microservices.
Do not redesign architecture.
Do not replace existing import/export completely.

Scope:
Improve existing Import / Export only.

Target Import Types:
- Words
- Categories
- Tags
- GrammarTopics
- GrammarRules
- Lessons
- Courses
- Books
- Quizzes

Supported Formats:
- CSV
- JSON

Advanced Import Features:
1. Upload import file.
2. Validate file.
3. Preview import result.
4. Show row-level errors.
5. Confirm import.
6. Store import history.
7. Rollback import if possible.
8. Export validation errors.

Domain Entities:

1. ImportJob

Fields:
- Id
- ImportType
- Format
- Status
- FileName
- OriginalFileName
- FileSize
- RequestedBy
- RequestedAt
- ValidatedAt
- ConfirmedAt
- CompletedAt
- FailedAt
- RolledBackAt
- TotalRows
- ValidRows
- InvalidRows
- ImportedRows
- FailedRows
- ErrorMessage
- CreatedAt
- UpdatedAt

Status values:
- Uploaded
- Validating
- ValidationFailed
- PreviewReady
- Confirmed
- Importing
- Completed
- Failed
- RolledBack
- Cancelled

Rules:
- ImportType is required.
- Format is required.
- Status is required.
- OriginalFileName is required.
- FileSize must be greater than zero.
- ImportJob starts as Uploaded.
- Import can only be confirmed after PreviewReady.
- Rollback is allowed only after Completed if rollback data exists.

2. ImportJobRow

Fields:
- Id
- ImportJobId
- RowNumber
- RawDataJson
- ParsedDataJson
- Status
- ErrorMessage
- CreatedEntityType
- CreatedEntityId
- UpdatedEntityType
- UpdatedEntityId
- CreatedAt
- UpdatedAt

Status values:
- Pending
- Valid
- Invalid
- Imported
- Failed
- RolledBack
- Skipped

Rules:
- ImportJobRow belongs to one ImportJob.
- RowNumber must be greater than zero.
- RawDataJson is required.
- Invalid row must have ErrorMessage.

3. ImportValidationError

Fields:
- Id
- ImportJobRowId
- FieldName
- ErrorCode
- ErrorMessage
- Severity
- CreatedAt
- UpdatedAt

Severity values:
- Info
- Warning
- Error
- Critical

Rules:
- ImportValidationError belongs to one ImportJobRow.
- ErrorCode is required.
- ErrorMessage is required.
- Severity is required.

Application Requirements:

Create CQRS-style folders:
- Features/ImportJobs/Commands
- Features/ImportJobs/Queries
- Features/ImportJobs/Dtos
- Features/ImportValidation/Commands
- Features/ImportValidation/Queries
- Features/ImportValidation/Dtos

Use cases:
- UploadImportFile
- ValidateImportJob
- PreviewImportJob
- ConfirmImportJob
- RunImportJob
- CancelImportJob
- RollbackImportJob
- GetImportJobById
- SearchImportJobs
- GetImportJobRows
- GetImportValidationErrors
- ExportImportErrors

Service Abstractions:
Create:
- IImportValidationService
- IImportParser
- IImportPreviewService
- IImportRollbackService

Import Validation Rules:
Words:
- Text required.
- MeaningTh required.
- Duplicate Text should be detected.
- CEFR value should be valid if provided.
- IPA fields optional but recommended.

Categories:
- Name required.
- Duplicate Name/Slug should be detected.

Tags:
- Name required.
- Duplicate Name/Slug should be detected.

Grammar:
- GrammarTopic Title required.
- GrammarRule Title and RuleText required.
- GrammarExample ExampleEn required.

Lessons:
- Title required.
- EstimatedMinutes must not be negative.
- Slug duplicate should be detected.

Courses:
- Title required.
- EstimatedMinutes must not be negative.
- Slug duplicate should be detected.

Books:
- Title required.
- EstimatedPages must not be negative.
- Slug duplicate should be detected.

Quizzes:
- Title required.
- PassingScore must be 0-100.
- Questions must be valid if included.

Infrastructure Requirements:
- Create EF Core configuration for ImportJob.
- Create EF Core configuration for ImportJobRow.
- Create EF Core configuration for ImportValidationError.
- Configure relationships:
  - ImportJob one-to-many ImportJobRow
  - ImportJobRow one-to-many ImportValidationError
- Configure indexes:
  - ImportJob ImportType
  - ImportJob Format
  - ImportJob Status
  - ImportJob RequestedBy
  - ImportJob RequestedAt
  - ImportJobRow ImportJobId
  - ImportJobRow RowNumber
  - ImportJobRow Status
  - ImportValidationError ImportJobRowId
  - ImportValidationError Severity
- Add migrations if EF Core tools are available.
- Do not break existing import/export features.

API Requirements:

Create endpoints:
- GET /api/v1/import-jobs
- GET /api/v1/import-jobs/{id}
- GET /api/v1/import-jobs/{id}/rows
- GET /api/v1/import-jobs/{id}/errors
- POST /api/v1/import-jobs/upload
- POST /api/v1/import-jobs/{id}/validate
- POST /api/v1/import-jobs/{id}/confirm
- POST /api/v1/import-jobs/{id}/run
- POST /api/v1/import-jobs/{id}/cancel
- POST /api/v1/import-jobs/{id}/rollback
- GET /api/v1/import-jobs/{id}/errors/export

Security:
- All import endpoints require authentication.
- Upload/validate/confirm/run import require import permission.
- Rollback requires admin or import rollback permission.
- Validate file size.
- Validate content type.
- Avoid path traversal.
- Do not execute uploaded content.
- Do not expose internal storage paths.

Permissions:
Add:
- import.read
- import.upload
- import.validate
- import.run
- import.rollback

Role mapping:
- SuperAdmin: all
- Admin: all
- ContentEditor: read, upload, validate, run
- Reviewer: read, validate
- Viewer: no import access unless existing pattern allows read-only admin

Blazor Requirements:

Create admin pages:
- /admin/import-jobs
- /admin/import-jobs/upload
- /admin/import-jobs/{id:guid}
- /admin/import-jobs/{id:guid}/rows
- /admin/import-jobs/{id:guid}/errors

Import Job list:
- Filter by ImportType
- Filter by Format
- Filter by Status
- Pagination

Upload page:
- Select import type
- Select format
- Upload file
- Show validation warnings

Import Job detail:
- Show status
- Show counts
- Show row summary
- Validate button
- Confirm button
- Run button
- Cancel button
- Rollback button if allowed
- Export errors button

Preview:
- Show valid row count
- Show invalid row count
- Show sample rows
- Show row-level errors

Update:
- AdminRoutes constants
- Admin navigation
- Admin dashboard if safe

Testing Requirements:
Add tests for:
- Create ImportJob
- Validate Word import with missing Text
- Validate duplicate Word Text
- Preview ImportJob
- Confirm only after PreviewReady
- Run import with valid rows
- Row-level validation errors
- Rollback import if implemented
- Permission checks
- Existing modules still pass

Quality:
- Keep implementation simple.
- Do not overengineer.
- Do not add background queue unless already available.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Domain entities created
3. Import validation service summary
4. API endpoints created
5. Blazor pages created
6. Permissions added
7. Tests created
8. Build/test result
9. Remaining limitations