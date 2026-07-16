Review and harden Advanced Import Validation and Content Migration Tools.

Project:
EnglishMaster

Goal:
Make import validation safe, consistent, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Domain must not depend on Infrastructure, API, or Web.
- Import logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns import use cases.
- Infrastructure handles file parsing/storage and EF Core implementation.

2. Import Job Lifecycle
Verify:
- ImportJob starts as Uploaded.
- Uploaded can go to Validating.
- Validation success creates PreviewReady.
- Validation failure creates ValidationFailed.
- PreviewReady can be Confirmed.
- Confirmed can be Importing.
- Importing can be Completed or Failed.
- Completed can be RolledBack if rollback is supported.
- Invalid transitions are blocked.

3. Row Validation
Verify:
- Each row has RowNumber.
- RawDataJson is stored safely.
- ParsedDataJson is stored if valid.
- Invalid rows have validation errors.
- Valid rows can be imported.
- Failed rows do not crash the entire import unless necessary.

4. File Safety
Verify:
- File size limit exists.
- Content type validation exists.
- File extension is not trusted alone.
- Path traversal is prevented.
- Uploaded content is not executed.
- Internal storage paths are not exposed.

5. Import Validation Rules
Verify:
- Word required fields are checked.
- Duplicate words are detected.
- Category duplicate names are detected.
- Tag duplicate names are detected.
- Grammar required fields are checked.
- Lesson/Course/Book numeric constraints are checked.
- Quiz PassingScore is checked.

6. Rollback Safety
Verify:
- Rollback only works for completed jobs.
- Rollback does not delete unrelated data.
- Rollback results are recorded per row.
- Rollback failures are reported safely.
- If rollback is placeholder/TODO, docs clearly state limitation.

7. Permissions
Verify:
- import.read protects read endpoints.
- import.upload protects upload endpoint.
- import.validate protects validate endpoint.
- import.run protects confirm/run.
- import.rollback protects rollback.
- Unauthorized returns 401.
- Forbidden returns 403.

8. API
Verify:
- Import job list works.
- Import job detail works.
- Rows endpoint works.
- Errors endpoint works.
- Upload/validate/confirm/run/cancel/rollback endpoints work.
- Export errors endpoint works if implemented.
- Internal exceptions are not leaked.

9. Blazor
Verify:
- Import job list works.
- Upload page works.
- Detail page works.
- Rows page works.
- Errors page works.
- Buttons show only when valid and allowed.
- Loading, empty, error states exist.

10. Tests
Verify:
- Import tests pass.
- Permission tests pass.
- Existing admin/public/student modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add new business modules.
Do not redesign architecture.

Output:
1. Problems found
2. Fixes applied
3. Import lifecycle result
4. Validation safety result
5. File safety result
6. Permission result
7. Build/test result
8. Remaining risks