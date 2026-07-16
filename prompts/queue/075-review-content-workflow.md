Review and harden the Content Review and Approval Workflow.

Project:
EnglishMaster

Goal:
Make the workflow clean, secure, consistent, and buildable.

Check and fix:

1. Clean Architecture
- Domain must not depend on Infrastructure, API, or Web.
- Application workflow logic must not be placed in API controllers.
- Blazor pages must not access DbContext directly.
- Permissions should be centralized.

2. Workflow Rules
Verify valid transitions:
- Draft -> InReview
- InReview -> Approved
- InReview -> ChangesRequested
- ChangesRequested -> InReview
- Approved -> Published
- Published -> Archived

Verify invalid transitions are blocked:
- Draft -> Published directly unless explicitly allowed.
- Archived -> Published directly.
- Published -> Draft without clear restore flow.

3. Existing Publish Behavior
- Existing IsPublished behavior still works.
- Existing Publish / Unpublish endpoints do not break.
- Published content has consistent ReviewStatus.
- Unpublish behavior is documented if it maps back to Approved or Draft.

4. Permissions
- Submit for review requires correct permission.
- Approve requires reviewer/admin permission.
- Request changes requires reviewer/admin permission.
- Publish requires publish permission.
- Archive requires archive permission.
- Viewer cannot modify workflow.

5. API
- Workflow endpoints return proper HTTP status codes.
- Invalid transition returns safe validation response.
- Unauthorized returns 401.
- Forbidden returns 403.
- Internal exceptions are not leaked.

6. Blazor
- Content Review page works.
- Detail pages show current status.
- Buttons only show when valid and allowed.
- Review note UI works if implemented.
- No broken admin navigation links.

7. Tests
- Workflow tests pass.
- Permission tests pass if implemented.
- Existing modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Workflow validation result
4. Permission result
5. Build/test result
6. Remaining risks