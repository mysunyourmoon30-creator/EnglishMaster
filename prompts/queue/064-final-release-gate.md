Run the final MVP release gate for EnglishMaster v0.1.0.

Goal:
Decide whether the MVP is ready to tag and release.

Do not add new features.
Only verify and document.

Check:

1. Build
- dotnet build passes.

2. Tests
- dotnet test passes.

3. Security
- No secrets committed.
- Admin routes protected.
- Admin APIs protected.
- Permissions applied.
- File upload safe.
- Import/export safe.
- Correct quiz answers not exposed unintentionally.

4. Database
- Migrations valid.
- Seed data safe.
- SuperAdmin setup documented.

5. Deployment
- Docker config valid.
- Staging config valid.
- GitHub Actions workflow valid.
- Health checks available.

6. Documentation
Verify docs exist:
- MVP release checklist
- Known limitations
- Staging deployment
- Environment variables
- Docker
- GitHub Actions
- Smoke test
- Security checklist
- Release notes v0.1.0

7. MVP Functionality
Verify core admin areas:
- Login/logout
- Dashboard
- Content modules
- Lesson/Course/Book
- Quiz
- Publishing
- Import/export
- User/Role/Permission

Output:
1. Release status: Ready / Not Ready
2. Passed checks
3. Failed checks
4. Critical blockers
5. Non-critical limitations
6. Suggested tag name
7. Suggested release title
8. Final release notes summary