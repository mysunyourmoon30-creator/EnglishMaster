Create Basic Reporting and Admin Dashboard Analytics for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Student-facing pages completed.
- Student Progress and Learning Tracking completed.

Goal:
Add basic admin reporting using existing data.

Important:
Do not add AI.
Do not add advanced analytics.
Do not add data warehouse.
Do not add external BI tools.
Do not add payment or marketplace.
Do not redesign architecture.

Scope:
Create basic read-only reporting for admin dashboard.

Reports Required:

1. Overview Metrics
- Total students
- Total active students
- Total words
- Total lessons
- Total courses
- Total books
- Total quizzes
- Total quiz attempts
- Average quiz score

2. Content Metrics
- Published lessons count
- Draft lessons count
- InReview content count
- Published courses count
- Published books count
- Published quizzes count

3. Learning Metrics
- Most accessed lessons if data exists
- Recently started lessons
- Recently completed lessons
- Course progress summary
- Book progress summary

4. Quiz Metrics
- Recent quiz attempts
- Average score by quiz
- Pass/fail count
- Top 5 quizzes by attempts

Application Requirements:
Create read-only reporting queries:
- GetAdminDashboardSummary
- GetContentStatusSummary
- GetLearningProgressSummary
- GetQuizAnalyticsSummary
- GetRecentActivitySummary

Create DTOs under:
- Features/Reports/Dtos
- Features/Reports/Queries

Infrastructure Requirements:
- Use efficient EF Core queries.
- Use AsNoTracking where appropriate.
- Avoid loading large related collections.
- Use grouped aggregate queries.
- Add pagination for recent activity if needed.
- Do not create heavy reporting tables yet.

API Requirements:
Create admin reporting endpoints:
- GET /api/v1/reports/admin-dashboard
- GET /api/v1/reports/content-status
- GET /api/v1/reports/learning-progress
- GET /api/v1/reports/quiz-analytics
- GET /api/v1/reports/recent-activity

Security:
- All reporting endpoints require authentication.
- Reporting endpoints require admin/report permission.
- Do not expose sensitive student identity fields unnecessarily.
- Do not expose passwords, tokens, security stamps, or private auth data.

Permissions:
Add permission:
- reports.read

Role mapping:
- SuperAdmin: reports.read
- Admin: reports.read
- Reviewer: optional read access if appropriate
- ContentEditor: no reporting access unless existing pattern suggests otherwise
- Viewer: no reporting access unless read-only admin role is intended

Blazor Requirements:
Update Admin Dashboard:
- Show overview metric cards.
- Show content status summary.
- Show quiz summary.
- Show recent activity.
- Show simple loading, empty, and error states.

Create reporting page:
- /admin/reports

Admin Reports page should show:
- Overview
- Content Status
- Learning Progress
- Quiz Analytics
- Recent Activity

Update:
- AdminRoutes constants
- Admin navigation
- Dashboard route docs if needed

Testing Requirements:
Add tests for:
- Admin dashboard summary query
- Content status summary query
- Quiz analytics summary query
- Reports require authorization
- Existing modules still pass

Quality:
- Keep implementation simple.
- Do not overengineer.
- Avoid expensive queries.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Report queries created
3. API endpoints created
4. Admin dashboard updated
5. Admin reports page created
6. Permissions added
7. Tests created
8. Build/test result
9. Remaining limitations