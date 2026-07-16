Verify the EnglishMaster MVP staging deployment.

Project:
EnglishMaster

Current Status:
- MVP release preparation completed.
- GitHub Actions release build completed.
- Staging configuration completed.
- Smoke test docs and scripts completed.

Goal:
Verify that the deployed staging environment works correctly.

Do not add new features.
Do not add new modules.
Only verify staging, fix deployment issues, and document findings.

Tasks:

1. Verify Build
- Run dotnet build.
- Run dotnet test.
- Confirm no failing tests.

2. Verify Docker / Staging Config
- Validate docker-compose.staging.yml.
- Validate .env.example.
- Validate appsettings.Staging.example.json.
- Confirm environment variables are documented.
- Confirm no real secrets are committed.

3. Verify Database
- Confirm migrations can be applied.
- Confirm Identity tables work.
- Confirm application tables work.
- Confirm seed data works safely.
- Confirm SuperAdmin setup is documented.

4. Verify Health Checks
Check:
- /health
- /health/live
- /health/ready

5. Verify Authentication
Check:
- Login page works.
- Logout works.
- /admin requires login.
- Authenticated SuperAdmin can access dashboard.
- Unauthorized users are blocked.

6. Verify Admin Areas
Check these pages are reachable:
- Dashboard
- Words
- Categories
- Tags
- Media
- Pronunciations
- Grammar
- Lessons
- Courses
- Books
- Quizzes
- Publishing
- Import
- Export
- Users
- Roles
- Permissions
- System Health

7. Verify Critical CRUD
Perform smoke test for:
- Create Word
- Create Category
- Upload or create Media metadata
- Create Lesson
- Create Course
- Create Book
- Create Quiz
- Create Publish Job
- Create User
- Assign Role

8. Fix Issues
Fix only:
- broken routes
- missing DI registrations
- migration issues
- auth issues
- permission issues
- docker config issues
- app startup issues
- broken smoke test issues

Do not redesign architecture.
Do not add new features.

Output:
1. Staging verification result
2. Health check result
3. Auth result
4. Admin route result
5. CRUD smoke test result
6. Issues found
7. Fixes applied
8. Build/test result
9. Remaining blockers