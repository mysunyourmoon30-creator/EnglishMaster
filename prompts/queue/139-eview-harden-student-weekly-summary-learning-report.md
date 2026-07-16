Review and harden Student Weekly Summary and Learning Report.

Project:
EnglishMaster

Goal:
Make weekly learning reports secure, accurate, useful, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Report generation logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns report use cases.
- Infrastructure handles EF Core only.
- Domain must not depend on Infrastructure, API, or Web.

2. WeeklyLearningReport Rules
Verify:
- WeeklyLearningReport belongs to one student.
- One report per student per WeekStartDate.
- WeekEndDate is after or equal WeekStartDate.
- Numeric counts never become negative.
- AverageQuizScore stays between 0 and 100 if available.
- Current week report can be regenerated.
- Past report behavior is documented.

3. Report Calculation Rules
Verify:
- Study minutes are calculated from safe existing data.
- ActiveStudyDays count is correct.
- CompletedDailyPlans count is correct.
- LessonsStarted and LessonsCompleted are correct.
- PracticeSessionsCompleted count is correct.
- QuizAttempts count is correct.
- QuizzesPassed count is correct.
- AchievementsEarned count is correct.
- Empty data returns a safe report.

4. Insight Rules
Verify:
- Low study time insight works.
- No activity insight works.
- Practice due insight works if implemented.
- Low quiz score insight works.
- Streak positive insight works.
- Achievement positive insight works.
- Insights are rule-based only.
- No AI-generated text exists.

5. Security
Verify:
- All /me/learning-reports endpoints require authentication.
- User can only access own reports.
- Other students' report data is not exposed.
- Quiz correct answers are not exposed.
- Admin-only fields are not exposed.
- Unauthorized returns 401.
- Forbidden returns 403 where applicable.

6. Query Performance
Verify:
- AsNoTracking is used where appropriate.
- Report history is paginated.
- No unbounded queries.
- No unnecessary Include chains.
- No avoidable N+1 queries.

7. API
Verify:
- Current week endpoint works.
- Report history endpoint works.
- Report detail endpoint works.
- By-date endpoint works.
- Generate endpoint works.
- Regenerate endpoint works.
- Insights endpoint works.
- Query parameters validate safely.
- Internal exceptions are not leaked.

8. Blazor
Verify:
- /learn/reports works.
- /learn/reports/current-week works.
- /learn/reports/{id} works.
- Student dashboard Weekly Summary card works.
- Generate report button works.
- Regenerate button works if implemented.
- Loading state works.
- Empty state works.
- Error state works.
- Logged-out users see safe behavior.

9. Tests
Verify:
- Weekly report tests pass.
- Insight tests pass.
- Security tests pass.
- Existing admin/public/student modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add external analytics.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Report calculation result
4. Insight rule result
5. Security result
6. Build/test result
7. Remaining risks