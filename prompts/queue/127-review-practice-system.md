Review and harden Practice System and Spaced Repetition Foundation.

Project:
EnglishMaster

Goal:
Make practice system secure, useful, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Practice logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns practice use cases.
- Infrastructure handles EF Core only.
- Domain must not depend on Infrastructure, API, or Web.

2. Practice Item Rules
Verify:
- PracticeItem belongs to one student.
- Duplicate PracticeItem is prevented for same student + content + practice type.
- DueAt and NextReviewAt are valid.
- ReviewCount, CorrectCount, IncorrectCount never become negative.
- Suspended items do not appear in due practice.

3. Spaced Repetition Rules
Verify:
- Again schedules review soon.
- Hard schedules review later than Again.
- Good schedules review later than Hard.
- Easy schedules review later than Good.
- CurrentIntervalDays stays safe.
- Mastered status is applied only when intended.
- Empty data returns safe empty state.

4. Practice Session Rules
Verify:
- Session starts with valid due items.
- Session item belongs to session.
- Submit updates PracticeSessionItem.
- Submit updates PracticeItem counters.
- CompletePracticeSession updates totals.
- Abandoned or completed sessions cannot be completed twice.

5. Public Content Safety
Verify:
- Draft content is not used.
- InReview content is not used.
- ChangesRequested content is not used.
- Archived content is not used.
- Inactive content is not used.
- Quiz correct answers are not exposed before submission.

6. Security
Verify:
- All /me/practice endpoints require authentication.
- User can only access own practice items and sessions.
- Other students' practice data is not exposed.
- Unauthorized returns 401.
- Forbidden returns 403 where applicable.

7. Query Performance
Verify:
- AsNoTracking is used where appropriate.
- No unbounded due item query.
- Limit/max limit exists.
- No unnecessary Include chains.
- No N+1 query patterns where avoidable.

8. Blazor
Verify:
- /learn/practice works.
- /learn/practice/session/{id} works.
- /learn/practice/history works.
- Student dashboard practice card works.
- Loading state works.
- Empty state works.
- Error state works.
- Logged-out users see safe behavior.

9. Tests
Verify:
- Practice tests pass.
- Security tests pass.
- Existing admin/public/student modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add external SRS service.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Practice item rule result
4. Spaced repetition result
5. Session result
6. Security result
7. Build/test result
8. Remaining risks