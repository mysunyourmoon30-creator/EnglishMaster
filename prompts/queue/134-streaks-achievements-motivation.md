Create Streaks, Achievements, and Motivation Foundation for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Student-facing pages completed.
- Student Progress completed.
- Practice System completed.
- Learning Goals completed.
- Daily Study Plan completed.

Goal:
Add simple rule-based motivation features for authenticated learners.

Important:
Do not add AI.
Do not add machine learning.
Do not add social features.
Do not add leaderboard.
Do not add payment.
Do not add marketplace.
Do not add mobile.
Do not add microservices.
Do not redesign architecture.

Scope:
Create simple learner motivation foundation:
- Study streak
- Activity log
- Achievement definitions
- Student achievements
- Basic motivation summary

Motivation Activity Types:
- LessonStarted
- LessonCompleted
- CourseStarted
- CourseCompleted
- BookStarted
- BookCompleted
- QuizAttempted
- QuizPassed
- PracticeSessionCompleted
- DailyStudyPlanCompleted
- LearningGoalCompleted

Achievement Types:
- FirstLessonCompleted
- FirstQuizCompleted
- FirstQuizPassed
- FirstPracticeSession
- ThreeDayStreak
- SevenDayStreak
- FourteenDayStreak
- ThirtyDayStreak
- TenLessonsCompleted
- FirstCourseCompleted
- FirstBookCompleted
- DailyPlanCompleted
- WeeklyGoalCompleted
- PracticeItemsCompleted

Domain Entities:

1. LearningActivityLog

Fields:
- Id
- StudentProfileId
- ActivityType
- ContentType
- ContentId
- Title
- OccurredAt
- MinutesSpent
- MetadataJson
- CreatedAt
- UpdatedAt

Rules:
- StudentProfileId is required.
- ActivityType is required.
- OccurredAt is required.
- MinutesSpent must not be negative.
- MetadataJson must not contain secrets or sensitive auth data.

2. StudentStreak

Fields:
- Id
- StudentProfileId
- CurrentStreakDays
- LongestStreakDays
- LastActivityDate
- StreakStartDate
- UpdatedAt
- CreatedAt

Rules:
- StudentProfileId is required.
- One StudentStreak per student.
- CurrentStreakDays must not be negative.
- LongestStreakDays must not be negative.
- LastActivityDate is updated when learner completes a qualifying activity.
- Same-day activities do not increase streak multiple times.
- Missing one day resets current streak to 1 on next qualifying activity.

3. AchievementDefinition

Fields:
- Id
- Code
- Name
- Description
- AchievementType
- TargetValue
- IconName
- IsActive
- SortOrder
- CreatedAt
- UpdatedAt

Rules:
- Code is required and unique.
- Name is required.
- AchievementType is required.
- TargetValue must not be negative.
- AchievementDefinition can be activated and deactivated.

4. StudentAchievement

Fields:
- Id
- StudentProfileId
- AchievementDefinitionId
- Status
- ProgressValue
- EarnedAt
- CreatedAt
- UpdatedAt

Status values:
- Locked
- InProgress
- Earned

Rules:
- StudentAchievement belongs to one student.
- StudentAchievement belongs to one AchievementDefinition.
- One StudentAchievement per student per AchievementDefinition.
- ProgressValue must not be negative.
- EarnedAt is set when status becomes Earned.

Application Requirements:

Create CQRS-style folders:
- Features/Motivation/Commands
- Features/Motivation/Queries
- Features/Motivation/Dtos
- Features/Achievements/Commands
- Features/Achievements/Queries
- Features/Achievements/Dtos
- Features/Streaks/Commands
- Features/Streaks/Queries
- Features/Streaks/Dtos

Use cases:
- RecordLearningActivity
- GetMyLearningActivity
- GetMyRecentLearningActivity
- UpdateMyStreak
- GetMyStreak
- GetMyMotivationSummary
- SeedDefaultAchievementDefinitions
- SearchAchievementDefinitions
- CreateAchievementDefinition
- UpdateAchievementDefinition
- ActivateAchievementDefinition
- DeactivateAchievementDefinition
- EvaluateMyAchievements
- GetMyAchievements
- GetMyEarnedAchievements
- GetMyAchievementProgress

Motivation Summary DTO:
- CurrentStreakDays
- LongestStreakDays
- TotalLessonsCompleted
- TotalCoursesCompleted
- TotalBooksCompleted
- TotalQuizAttempts
- TotalQuizPassed
- TotalPracticeSessionsCompleted
- TotalDailyPlansCompleted
- TotalGoalsCompleted
- EarnedAchievementCount
- RecentAchievements
- RecentActivity

Achievement Evaluation Rules:
- FirstLessonCompleted: earned after 1 completed lesson.
- FirstQuizCompleted: earned after 1 quiz attempt submitted.
- FirstQuizPassed: earned after 1 passed quiz attempt.
- FirstPracticeSession: earned after 1 completed practice session.
- ThreeDayStreak: earned at 3 current or longest streak days.
- SevenDayStreak: earned at 7 current or longest streak days.
- FourteenDayStreak: earned at 14 current or longest streak days.
- ThirtyDayStreak: earned at 30 current or longest streak days.
- TenLessonsCompleted: earned after 10 completed lessons.
- FirstCourseCompleted: earned after 1 completed course.
- FirstBookCompleted: earned after 1 completed book.
- DailyPlanCompleted: earned after 1 completed daily study plan.
- PracticeItemsCompleted: earned after target practice item count.

Integration:
Record activity and evaluate achievements when safe:
- Lesson completed
- Course completed
- Book completed
- Quiz attempt submitted
- Quiz passed
- Practice session completed
- Daily study plan completed
- Learning goal completed

If full integration is too large, implement manual service calls and integrate first with:
- Lesson completed
- Quiz attempt submitted
- Practice session completed
- Daily study plan completed

Then document TODO for remaining events.

Infrastructure Requirements:
- Create EF Core configuration for LearningActivityLog.
- Create EF Core configuration for StudentStreak.
- Create EF Core configuration for AchievementDefinition.
- Create EF Core configuration for StudentAchievement.
- Configure unique indexes:
  - StudentStreak StudentProfileId
  - AchievementDefinition Code
  - StudentAchievement StudentProfileId + AchievementDefinitionId
- Configure indexes:
  - LearningActivityLog StudentProfileId
  - LearningActivityLog ActivityType
  - LearningActivityLog OccurredAt
  - AchievementDefinition AchievementType
  - AchievementDefinition IsActive
  - StudentAchievement StudentProfileId
  - StudentAchievement Status
  - StudentAchievement EarnedAt
- Add migrations if EF Core tools are available.
- Do not break existing modules.

API Requirements:

Authenticated learner endpoints:
- GET /api/v1/me/motivation/summary
- GET /api/v1/me/motivation/activity
- GET /api/v1/me/motivation/activity/recent
- GET /api/v1/me/streak
- POST /api/v1/me/streak/update
- GET /api/v1/me/achievements
- GET /api/v1/me/achievements/earned
- GET /api/v1/me/achievements/progress
- POST /api/v1/me/achievements/evaluate

Admin achievement endpoints:
- GET /api/v1/admin/achievement-definitions
- POST /api/v1/admin/achievement-definitions
- PUT /api/v1/admin/achievement-definitions/{id}
- POST /api/v1/admin/achievement-definitions/{id}/activate
- POST /api/v1/admin/achievement-definitions/{id}/deactivate
- POST /api/v1/admin/achievement-definitions/seed-defaults

Security:
- All /me motivation endpoints require authentication.
- Users can only access their own motivation data.
- Admin achievement definition endpoints require admin permission.
- Do not expose another student's activity data.
- Do not expose sensitive auth fields.
- Do not expose internal file paths.
- MetadataJson must be safe.

Permissions:
Add:
- achievements.read
- achievements.manage
- motivation.read

Role mapping:
- SuperAdmin: all
- Admin: achievements.manage and achievements.read
- ContentEditor: no admin achievement management
- Reviewer: no admin achievement management
- Viewer: no admin achievement management
- Authenticated learners: own motivation and achievement data only

Blazor Requirements:

Create student pages:
- /learn/motivation
- /learn/achievements
- /learn/activity

Update Student Dashboard:
- Add streak card.
- Add achievement summary card.
- Add recent achievements.
- Add recent activity.
- Add motivational empty state for new learners.

Motivation page:
- Show current streak.
- Show longest streak.
- Show basic learning totals.
- Show recent activity.
- Show recent achievements.

Achievements page:
- Show earned achievements.
- Show in-progress achievements.
- Show locked achievements if simple.
- Show progress value.
- Show earned date.

Activity page:
- Show recent learning activity.
- Filter by activity type if simple.
- Pagination.

Admin pages:
- /admin/achievement-definitions
- /admin/achievement-definitions/create
- /admin/achievement-definitions/{id:guid}/edit

Update:
- AdminRoutes constants
- Public/student routes docs if applicable
- Admin navigation if admin pages are added

Testing Requirements:
Add tests for:
- Record learning activity.
- Same-day activity does not increase streak twice.
- Consecutive-day activity increases streak.
- Missed day resets current streak.
- Achievement definition Code is unique.
- FirstLessonCompleted achievement is earned.
- ThreeDayStreak achievement is earned.
- Student cannot access another student's motivation data.
- Admin permission required for achievement definition management.
- Existing modules still pass.

Quality:
- Rule-based only.
- Keep implementation simple.
- Use efficient EF Core queries.
- Use AsNoTracking where appropriate.
- Apply pagination/limit to activity history.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Domain entities created
3. Streak logic summary
4. Achievement evaluation summary
5. API endpoints created
6. Blazor pages created
7. Student dashboard updates
8. Security summary
9. Tests created
10. Build/test result
11. Remaining limitations