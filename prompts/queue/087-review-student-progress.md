Review and harden Student Progress and Learning Tracking.

Project:
EnglishMaster

Goal:
Make student progress tracking secure, consistent, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Domain must not depend on Infrastructure, API, or Web.
- Progress logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns use cases.

2. Student Profile
Verify:
- StudentProfile belongs to one user.
- UserId is required.
- Duplicate StudentProfile per user is prevented.
- Profile data does not expose sensitive user data.

3. Lesson Progress
Verify:
- StartLesson creates or updates progress safely.
- CompleteLesson sets Status to Completed.
- CompletedAt is set.
- ProgressPercent remains 0-100.
- Duplicate lesson progress is prevented.

4. Course Progress
Verify:
- Course progress belongs to student and course.
- Progress recalculation works if implemented.
- CompletedLessonCount and TotalLessonCount are consistent.
- ProgressPercent remains 0-100.

5. Book Progress
Verify:
- Book progress belongs to student and book.
- ProgressPercent remains 0-100.
- Counts are safe and do not crash when empty.

6. Quiz Attempts
Verify:
- Quiz attempt belongs to student.
- Quiz attempt belongs to quiz.
- Submit calculates score safely.
- Correct answers are not exposed before submit.
- Student can only see their own attempts.

7. Security
Verify:
- All /me endpoints require authentication.
- A user cannot access another user's progress.
- Admin endpoints remain protected.
- No sensitive Identity fields are exposed.

8. Blazor
Verify:
- /learn/dashboard works.
- Lesson progress UI works.
- Course progress UI works.
- Book progress UI works.
- Quiz attempt UI works.
- Logged-out users see safe behavior.
- Loading, empty, error states exist.

9. Tests
Verify:
- Student progress tests pass.
- Quiz attempt tests pass.
- Auth/security tests pass.
- Existing modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add certificates.
Do not add advanced analytics.

Output:
1. Problems found
2. Fixes applied
3. Progress validation result
4. Quiz attempt result
5. Security result
6. Build/test result
7. Remaining risks