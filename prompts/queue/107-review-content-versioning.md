Review and harden Content Versioning and Revision History.

Project:
EnglishMaster

Goal:
Make content versioning secure, consistent, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Domain must not depend on Infrastructure, API, or Web.
- Revision logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns revision use cases.
- Infrastructure handles EF Core and serialization implementation.

2. Revision Rules
Verify:
- ContentRevision has required ContentType.
- ContentRevision has required ContentId.
- RevisionNumber increments correctly per content item.
- EventType is valid.
- ChangedAt is set.
- SnapshotJson is created safely.
- DiffJson does not crash if empty or unsupported.

3. Snapshot Safety
Verify:
- No passwords are serialized.
- No tokens are serialized.
- No security stamps are serialized.
- No connection strings or secrets are serialized.
- Only content-safe fields are included.

4. Restore Request Rules
Verify:
- Restore request starts as Pending.
- Pending can be Approved.
- Pending can be Rejected.
- Approved can be Completed.
- Rejected cannot be Completed.
- Cancelled request cannot be Completed.
- Review note is saved where appropriate.

5. Permissions
Verify:
- content-revisions.read protects read endpoints.
- restore request requires correct permission.
- restore approval requires correct permission.
- Viewer cannot restore content.
- Unauthorized returns 401.
- Forbidden returns 403.

6. API
Verify:
- Revision list works.
- Revision detail works.
- Revisions by content works.
- Latest revision works.
- Restore request endpoints work.
- Validation errors are safe.
- Internal exceptions are not leaked.

7. Blazor
Verify:
- Revision list page works.
- Revision detail page works.
- Revision timeline works.
- Restore request pages work.
- Content detail pages link to revision history.
- Loading, empty, error states exist.

8. Tests
Verify:
- Revision tests pass.
- Restore request tests pass.
- Permission tests pass.
- Existing admin/public/student modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add advanced audit platform.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Revision rule result
4. Snapshot safety result
5. Restore workflow result
6. Security result
7. Build/test result
8. Remaining risks