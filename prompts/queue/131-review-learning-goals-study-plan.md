Review and harden Learning Goals and Daily Study Plan.

Project:
EnglishMaster

Goal:
Make learning goals and daily study plans secure, useful, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Goal and study plan logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns use cases.
- Infrastructure handles EF Core only.
- Domain must not depend on Infrastructure, API, or Web.

2. Learning Goal Rules
Verify:
- LearningGoal belongs to one student.
- TargetValue and CurrentValue never become negative.
- Active goal can be Paused.
- Paused goal can be Resumed.
- Active goal can be Completed.
- Active or Paused goal can be Cancelled.
- Completed and Cancelled goals cannot be modified incorrectly.
- User cannot access another user's goals.

3. Daily Study Plan Rules
Verify:
- One DailyStudyPlan per student per day.
- PlanDate is valid.
- TargetMinutes and CompletedMinutes never become negative.
- CompletedItems and TotalItems are consistent.
- CompletedAt is set on completed items.
- Skipped items do not count as completed unless intentionally documented.
- Empty data returns safe empty state.

4. Plan Generation Rules
Verify:
- Due practice items appear first.
- Continue learning items appear when available.
- Next lesson appears when available.
- Weak quiz review appears when available.
- Recommended content appears when available.
- Duplicate content does not appear in same plan.
- Draft content does not appear.
- Archived content does not appear.
- Inactive content does not appear.
- Quiz correct answers are not exposed.

5. Security
Verify:
- All /me/learning-goals endpoints require authentication.
- All /me/study-plan endpoints require authentication.
- User can only access own goals and plans.
- Other students' data is not exposed.
- Unauthorized returns 401.
- Forbidden returns 403 where applicable.

6. Query Performance
Verify:
- AsNoTracking is used where appropriate.
- No unbounded history query.
- Limit/max limit exists.
- No unnecessary Include chains.
- No avoidable N+1 queries.

7. API
Verify:
- Learning goal list works.
- Active goals endpoint works.
- Goal create/update works.
- Pause/resume/complete/cancel works.
- Today study plan endpoint works.
- Generate today plan endpoint works.
- Complete/skip item endpoints work.
- Study plan history works.
- Query parameters validate safely.

8. Blazor
Verify:
- /learn/goals works.
- /learn/goals/create works.
- /learn/goals/{id} works.
- /learn/study-plan works.
- /learn/study-plan/history works.
- Student dashboard still works.
- Loading state works.
- Empty state works.
- Error state works.
- Logged-out users see safe behavior.

9. Tests
Verify:
- Goal tests pass.
- Study plan tests pass.
- Security tests pass.
- Existing admin/public/student modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add external calendar.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Learning goal rule result
4. Daily study plan result
5. Plan generation result
6. Security result
7. Build/test result
8. Remaining risks