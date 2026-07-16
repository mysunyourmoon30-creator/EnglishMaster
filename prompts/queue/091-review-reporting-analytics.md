Review and harden Basic Reporting and Admin Dashboard Analytics.

Project:
EnglishMaster

Goal:
Make reporting secure, efficient, buildable, and consistent.

Check and fix:

1. Clean Architecture
- Reporting logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns reporting queries.
- Infrastructure handles EF Core implementation only.

2. Query Performance
Verify:
- AsNoTracking is used for read-only queries where appropriate.
- List/recent activity queries are paginated or limited.
- No unnecessary Include chains.
- No N+1 query patterns where avoidable.
- Aggregates are calculated efficiently.

3. Security
Verify:
- Reporting endpoints require authentication.
- reports.read permission is required.
- Sensitive user/security fields are not exposed.
- Student data is summarized safely.
- No internal exceptions are leaked.

4. API
Verify endpoints:
- /api/v1/reports/admin-dashboard
- /api/v1/reports/content-status
- /api/v1/reports/learning-progress
- /api/v1/reports/quiz-analytics
- /api/v1/reports/recent-activity

5. Blazor
Verify:
- /admin/reports works.
- Admin dashboard still works.
- Metric cards render safely.
- Empty state works when there is no data.
- Error state works.
- Navigation link works.

6. Permissions
Verify:
- SuperAdmin can access reports.
- Admin can access reports.
- Unauthorized users cannot access reports.
- Users without reports.read are forbidden.

7. Tests
Verify:
- Reporting tests pass.
- Authorization tests pass.
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
3. Query performance result
4. Security result
5. Build/test result
6. Remaining risksReview and harden Basic Reporting and Admin Dashboard Analytics.

Project:
EnglishMaster

Goal:
Make reporting secure, efficient, buildable, and consistent.

Check and fix:

1. Clean Architecture
- Reporting logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns reporting queries.
- Infrastructure handles EF Core implementation only.

2. Query Performance
Verify:
- AsNoTracking is used for read-only queries where appropriate.
- List/recent activity queries are paginated or limited.
- No unnecessary Include chains.
- No N+1 query patterns where avoidable.
- Aggregates are calculated efficiently.

3. Security
Verify:
- Reporting endpoints require authentication.
- reports.read permission is required.
- Sensitive user/security fields are not exposed.
- Student data is summarized safely.
- No internal exceptions are leaked.

4. API
Verify endpoints:
- /api/v1/reports/admin-dashboard
- /api/v1/reports/content-status
- /api/v1/reports/learning-progress
- /api/v1/reports/quiz-analytics
- /api/v1/reports/recent-activity

5. Blazor
Verify:
- /admin/reports works.
- Admin dashboard still works.
- Metric cards render safely.
- Empty state works when there is no data.
- Error state works.
- Navigation link works.

6. Permissions
Verify:
- SuperAdmin can access reports.
- Admin can access reports.
- Unauthorized users cannot access reports.
- Users without reports.read are forbidden.

7. Tests
Verify:
- Reporting tests pass.
- Authorization tests pass.
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
3. Query performance result
4. Security result
5. Build/test result
6. Remaining risks