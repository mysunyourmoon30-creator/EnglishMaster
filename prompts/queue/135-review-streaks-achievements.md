Review and harden Streaks, Achievements, and Motivation Foundation.

Project:
EnglishMaster

Goal:
Make motivation features secure, consistent, useful, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Motivation logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns motivation use cases.
- Infrastructure handles EF Core only.
- Domain must not depend on Infrastructure, API, or Web.

2. Learning Activity Rules
Verify:
- LearningActivityLog belongs to one student.
- ActivityType is valid.
- OccurredAt is set.
- MinutesSpent never becomes negative.
- MetadataJson does not contain secrets or sensitive auth fields.
- Activity history is paginated or limited.

3. Streak Rules
Verify:
- One StudentStreak per student.
- Same-day activity does not increase streak multiple times.
- Consecutive-day activity increases CurrentStreakDays.
- Missed day resets CurrentStreakDays safely.
- LongestStreakDays updates correctly.
- LastActivityDate updates correctly.
- Time/date handling is consistent and documented.

4. Achievement Definition Rules
Verify:
- AchievementDefinition Code is unique.
- Name is required.
- AchievementType is valid.
- TargetValue is not negative.
- Inactive achievement definitions are not newly awarded unless intended.
- Seed defaults does not create duplicates.

5. Student Achievement Rules
Verify:
- One StudentAchievement per student per AchievementDefinition.
- ProgressValue never becomes negative.
- EarnedAt is set when earned.
- Earned achievement is not awarded repeatedly.
- Locked/InProgress/Earned status behaves consistently.

6. Achievement Evaluation Rules
Verify:
- FirstLessonCompleted works.
- FirstQuizCompleted works if implemented.
- FirstQuizPassed works if implemented.
- FirstPracticeSession works if implemented.
- ThreeDayStreak works.
- SevenDayStreak works if implemented.
- TenLessonsCompleted works if implemented.
- Empty data returns safe empty state.

7. Security
Verify:
- All /me motivation endpoints require authentication.
- User can only access own motivation data.
- Other students' activity and achievements are not exposed.
- Admin achievement definition endpoints require permission.
- Unauthorized returns 401.
- Forbidden returns 403 where applicable.

8. API
Verify:
- Motivation summary endpoint works.
- Activity list endpoint works.
- Recent activity endpoint works.
- Streak endpoint works.
- Achievements endpoints work.
- Achievement definition admin endpoints work.
- Seed defaults endpoint is protected.
- Validation errors are safe.
- Internal exceptions are not leaked.

9. Blazor
Verify:
- /learn/motivation works.
- /learn/achievements works.
- /learn/activity works.
- Student dashboard streak card works.
- Student dashboard achievement card works.
- Admin achievement definition pages work if added.
- Loading state works.
- Empty state works.
- Error state works.
- Logged-out users see safe behavior.

10. Tests
Verify:
- Motivation tests pass.
- Streak tests pass.
- Achievement tests pass.
- Security tests pass.
- Existing admin/public/student modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add leaderboard.
Do not add social features.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Activity log result
4. Streak result
5. Achievement result
6. Security result
7. Build/test result
8. Remaining risks