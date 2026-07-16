Create staging smoke test checklist and scripts for EnglishMaster MVP.

Goal:
Verify that staging deployment works after deployment.

Do not add new features.

Tasks:

1. Create smoke test documentation
Create:
- docs/release/staging-smoke-test.md

Include checks:
- App loads
- /health works
- /health/live works
- /health/ready works
- Login works
- Admin dashboard works
- Navigation works
- CRUD smoke test for:
  - Words
  - Categories
  - Tags
  - Media
  - Pronunciation
  - Grammar
  - Lessons
  - Courses
  - Books
  - Quizzes
  - Publishing
  - Users
  - Roles
  - Permissions

2. Create API smoke test script if simple
Create under scripts:
- scripts/smoke-test-api.ps1

The script should:
- accept BaseUrl parameter
- call health endpoints
- optionally call authenticated endpoints only if token/cookie flow is simple
- not store secrets

3. Create manual UI smoke test checklist
Document:
- login
- create word
- create lesson
- create quiz
- create publish job
- logout

4. Quality
Run:
- dotnet build
- dotnet test

Output:
1. Smoke test docs created
2. Smoke test script created if possible
3. Manual test checklist
4. Build/test result
5. Remaining limitations