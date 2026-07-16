Review and harden Content Quality and QA System.

Project:
EnglishMaster

Goal:
Make content quality checks secure, consistent, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Domain must not depend on Infrastructure, API, or Web.
- Quality check logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns quality check use cases.
- Infrastructure handles EF Core only.

2. Quality Rules
Verify:
- ContentQualityRule has unique Code.
- Rule severity is consistent.
- Rules can be activated and deactivated.
- Inactive rules are not applied if intended.

3. Quality Checks
Verify:
- RunQualityCheck creates ContentQualityCheck.
- Findings are created correctly.
- Score remains 0-100.
- Status reflects findings:
  - Passed when no findings
  - Warning when only warnings/info
  - Failed when errors exist
  - Failed/Critical when critical findings exist
- Latest check per content item works.

4. Findings
Verify:
- Findings have RuleCode.
- Findings have Message.
- Findings can be marked resolved.
- Resolved findings retain history.

5. Module Rules
Verify basic checks:
- Word missing IPA creates warning.
- Word missing MeaningTh creates error.
- Lesson without sections creates warning or error.
- Course without lessons creates warning or error.
- Book without chapters creates warning or error.
- Quiz without questions creates error.
- Choice-based question without correct answer creates error.

6. Security
Verify:
- All endpoints require authentication.
- content-quality.read protects read endpoints.
- content-quality.run protects run endpoints.
- content-quality.manage protects rule management.
- Unauthorized returns 401.
- Forbidden returns 403.

7. Blazor
Verify:
- Content Quality dashboard works.
- Quality check list works.
- Quality check detail works.
- Rule list/create/edit works.
- Run Quality Check button works where implemented.
- Loading, empty, and error states exist.

8. Tests
Verify:
- Quality tests pass.
- Permission tests pass.
- Existing admin/public/student modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add advanced analytics.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Quality rule result
4. Quality check result
5. Security result
6. Build/test result
7. Remaining risks