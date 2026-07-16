Perform MVP bug bash for EnglishMaster.

Goal:
Find and fix critical MVP bugs before release v0.1.0.

Do not add new features.
Do not add new modules.
Only fix bugs that block MVP usage.

Bug Areas:

1. Authentication / Authorization
- Login/logout issues
- Admin access issues
- Role/permission issues
- 401/403 behavior

2. Admin Navigation
- Broken links
- Missing routes
- Wrong active menu
- Pages not reachable

3. Core CRUD
Verify and fix:
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
- Users
- Roles
- Permissions

4. Validation
- Required fields
- Invalid input handling
- Safe error messages

5. Database
- Migration issues
- Relationship issues
- Foreign key issues
- Seed data issues

6. Import / Export
- File validation
- Permission protection
- Error handling

7. Publishing
- Publish job status transitions
- Artifact creation
- Safe file paths

8. UI
- Loading states
- Empty states
- Error states
- Form submit issues

Quality:
- Keep fixes minimal.
- Do not redesign.
- Do not add new scope.
- Run dotnet build.
- Run dotnet test.
- Fix until all pass.

Output:
1. Bugs found
2. Bugs fixed
3. Critical blockers
4. Non-critical issues
5. Build/test result
6. Release readiness recommendation