Perform full v0.2.0 Security and Permission Audit for EnglishMaster.

Project:
EnglishMaster

Goal:
Audit all v0.2.0 modules for authentication, authorization, data privacy, and public content safety.

Important:
Do not add new features.
Do not redesign architecture.
Do not add external security services.
Fix only security, permission, and privacy issues.

Audit Areas:

1. Authentication
Verify:
- Admin pages require login.
- /me endpoints require login.
- Public pages do not require login unless intended.
- Auth state is handled safely in Blazor.
- Logged-out users do not see private data.

2. Authorization
Verify permissions for:
- words
- categories
- tags
- media
- pronunciation
- grammar
- lessons
- courses
- books
- quizzes
- publishing
- content review
- users
- roles
- permissions
- reports
- notifications
- email messages
- content quality
- content revisions
- bulk operations
- import jobs
- achievement definitions

3. Student Data Privacy
Verify:
- StudentProfile is private.
- LessonProgress is private.
- CourseProgress is private.
- BookProgress is private.
- QuizAttempt is private.
- Practice data is private.
- Learning goals are private.
- Study plans are private.
- Motivation data is private.
- Learning reports are private.
- Users cannot access another user's data.

4. Public Content Safety
Verify:
- Public pages show only Published and Active content.
- Draft content is hidden.
- InReview content is hidden.
- ChangesRequested content is hidden.
- Archived content is hidden.
- Admin-only fields are hidden.
- Internal file paths are hidden.
- Quiz correct answers are hidden before submit.

5. File and Import Safety
Verify:
- Upload file size is limited.
- File content type is validated.
- Path traversal is blocked.
- Import files are not executed.
- Media paths are safe.
- Published artifact paths are safe.

6. Secrets and Sensitive Data
Verify:
- No secrets are committed.
- No API keys are committed.
- No real connection strings are committed.
- No email provider credentials are committed.
- No passwords/tokens/security stamps are serialized in snapshots or logs.
- Logs do not expose sensitive data.

7. API Error Safety
Verify:
- Validation errors are safe.
- Unauthorized returns 401.
- Forbidden returns 403.
- Not found returns safe response.
- Internal exceptions are not leaked.

8. Tests
Add or fix tests where reasonable:
- Unauthorized user cannot access admin endpoint.
- User cannot access another user's progress.
- User cannot access another user's practice data.
- User cannot access another user's report.
- Public endpoint hides draft content.
- Quiz endpoint does not expose correct answers.
- Import upload rejects unsafe path.

Run:
- dotnet build
- dotnet test

Output:
1. Security issues found
2. Fixes applied
3. Permission audit summary
4. Student privacy result
5. Public content safety result
6. File/import safety result
7. Secrets/logging result
8. Build/test result
9. Remaining security risks