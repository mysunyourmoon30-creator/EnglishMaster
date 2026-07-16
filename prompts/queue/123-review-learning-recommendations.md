Review and harden Learning Recommendations and Continue Learning.

Project:
EnglishMaster

Goal:
Make rule-based recommendations safe, useful, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Recommendation logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns recommendation use cases.
- Infrastructure handles EF Core queries only.
- Domain must not depend on Infrastructure, API, or Web.

2. Continue Learning Rules
Verify:
- InProgress lessons appear.
- InProgress courses appear.
- InProgress books appear.
- Completed items do not appear as continue learning unless intentionally shown as recent history.
- Results are sorted by LastAccessedAt descending.
- Limit/max limit exists.
- Deleted/archived/draft content does not appear.

3. Recommendation Rules
Verify:
- Recommended courses exclude completed courses.
- Recommended lessons exclude completed lessons.
- CEFR matching works when available.
- Category/tag matching works if implemented.
- Next lesson in course returns first incomplete lesson by order.
- If all lessons are completed, next lesson returns empty/safe result.
- Low quiz score can create review recommendation if implemented.
- Empty data produces safe empty state.

4. Public Content Safety
Verify:
- Draft content does not appear.
- InReview content does not appear.
- ChangesRequested content does not appear.
- Archived content does not appear.
- Only Published and Active content appears where applicable.
- Admin-only fields are not exposed.
- Internal file paths are not exposed.
- Quiz correct answers are not exposed.

5. Security
Verify:
- All /me/learning endpoints require authentication.
- User can only access own recommendations.
- Other students' progress is not exposed.
- Unauthorized returns 401.
- Forbidden returns 403 where applicable.

6. Query Performance
Verify:
- AsNoTracking is used where appropriate.
- No unbounded result sets.
- No unnecessary Include chains.
- No N+1 query patterns where avoidable.
- Limit/max limit is enforced.

7. API
Verify:
- GET /api/v1/me/learning/continue works.
- GET /api/v1/me/learning/recommendations works.
- Recommended courses endpoint works.
- Recommended lessons endpoint works.
- Recommended words endpoint works.
- Recommended grammar endpoint works.
- Recommended quizzes endpoint works.
- Next lesson endpoint works.
- Query parameters validate safely.

8. Blazor
Verify:
- /learn/dashboard still works.
- /learn/recommendations works.
- Continue Learning section works.
- Recommendation groups render correctly.
- Reason text displays safely.
- Loading state works.
- Empty state works.
- Error state works.
- Logged-out users see safe behavior.

9. Tests
Verify:
- Recommendation tests pass.
- Security tests pass.
- Existing admin/public/student modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add external recommendation service.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Continue learning result
4. Recommendation rule result
5. Public content safety result
6. Security result
7. Build/test result
8. Remaining risks