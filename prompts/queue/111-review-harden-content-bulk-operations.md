Review and harden Content Bulk Operations.

Project:
EnglishMaster

Goal:
Make bulk operations secure, consistent, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Domain must not depend on Infrastructure, API, or Web.
- Bulk operation logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns bulk operation use cases.
- Infrastructure handles EF Core only.

2. Bulk Operation Rules
Verify:
- BulkOperation starts as Pending.
- Running status is set when operation starts.
- Completed status is set when all items succeed.
- PartiallyCompleted status is set when some items fail.
- Failed status is set when operation fails fully.
- Cancelled status works only when allowed.
- TotalItems, SucceededItems, and FailedItems are consistent.

3. Bulk Item Rules
Verify:
- Each selected content item creates BulkOperationItem.
- Success items are marked Success.
- Failed items are marked Failed with ErrorMessage.
- Skipped items are marked Skipped when applicable.
- Failed item does not stop all other items unless operation requires it.

4. Workflow Safety
Verify:
- Bulk publish cannot publish Draft content directly unless rules allow.
- Bulk approve requires InReview status.
- Bulk archive follows workflow rules.
- Bulk quality check does not modify content unexpectedly.
- Bulk activate/deactivate follows existing domain rules.

5. Permission Safety
Verify:
- bulk-operations.read protects read endpoints.
- bulk-operations.run protects run endpoints.
- bulk-operations.cancel protects cancel endpoints.
- Underlying permissions are checked.
- Viewer cannot run bulk actions.
- Unauthorized returns 401.
- Forbidden returns 403.

6. API
Verify:
- Bulk operation list works.
- Bulk operation detail works.
- Bulk operation item list works.
- Create endpoint validates input.
- Run endpoint validates workflow and permissions.
- Cancel endpoint works if implemented.
- Internal exceptions are not leaked.

7. Blazor
Verify:
- Bulk operation list page works.
- Create page works.
- Detail page works.
- Run button works.
- Cancel button works if implemented.
- Loading, empty, error states exist.
- Navigation links work.

8. Tests
Verify:
- Bulk operation tests pass.
- Permission tests pass.
- Existing admin/public/student modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add background worker unless already simple.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Bulk status result
4. Workflow safety result
5. Permission result
6. Build/test result
7. Remaining risks