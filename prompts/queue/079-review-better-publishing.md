Review and harden the Better Publishing v0.2.0 implementation.

Project:
EnglishMaster

Goal:
Make improved publishing secure, consistent, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Domain must not depend on Infrastructure, API, or Web.
- Publishing logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- File storage implementation must stay in Infrastructure.
- Template rendering should be behind Application/Infrastructure abstraction if appropriate.

2. Publishing Workflow
Verify:
- Preview does not create final artifact unless intended.
- Run publish creates artifact only when valid.
- Draft content cannot be published.
- InReview content cannot be published.
- ChangesRequested content cannot be published.
- Approved content can be published.
- Published content can be republished if allowed.
- Archived content cannot be published.

3. Templates
Verify:
- Selected template works.
- Default template fallback works.
- Template format matches publish format.
- Template variables render safely.
- Missing variables do not crash the system.

4. File Safety
Verify:
- Output filename is safe.
- Output path prevents path traversal.
- Internal storage path is not exposed unintentionally.
- PublicUrl and FilePath are separated correctly.

5. API
Verify:
- Preview endpoint returns proper HTTP status codes.
- Run endpoint returns proper HTTP status codes.
- Validation errors are safe.
- Internal exceptions are not leaked.
- Permissions are enforced.

6. Blazor
Verify:
- Create publish job page works.
- Template selection works.
- Publish job detail page works.
- Preview button works.
- Run button works.
- Error messages are readable.
- Artifacts show correctly after publish.
- No broken navigation links.

7. Tests
Verify:
- Publishing tests pass.
- Template tests pass.
- Preview tests pass.
- Existing Word, Grammar, Lesson, Course, Book, Quiz, Auth, Review Workflow tests still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Publishing validation result
4. Template result
5. File safety result
6. Build/test result
7. Remaining risks