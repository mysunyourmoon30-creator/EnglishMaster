# Weekly Learning Report

## Purpose

`WeeklyLearningReport` stores one learner-owned summary for one calendar week.

## Fields

- `Id`: report identifier.
- `StudentProfileId`: owning student profile.
- `WeekStartDate`: UTC Monday start of the report week.
- `WeekEndDate`: UTC Sunday end of the report week.
- `Status`: report lifecycle state.
- `GeneratedAt`: last generation time.
- `TotalStudyMinutes`: sum of weekly activity minutes.
- `ActiveStudyDays`: distinct UTC days with learning activity.
- `CompletedDailyPlans`: completed daily plans in the week.
- `LessonsStarted`, `LessonsCompleted`: lesson activity counts.
- `CoursesStarted`, `CoursesCompleted`: course activity counts.
- `BooksStarted`, `BooksCompleted`: book activity counts.
- `PracticeSessionsCompleted`: completed practice sessions.
- `PracticeItemsCompleted`: completed practice items inside completed sessions.
- `QuizAttempts`: quiz attempts in the week.
- `QuizzesPassed`: passed quiz attempts.
- `AverageQuizScore`: average score clamped from 0 to 100.
- `GoalsCompleted`: goals completed during the week.
- `AchievementsEarned`: achievements earned during the week.
- `CurrentStreakDays`: current streak at generation time.
- `LongestStreakDays`: longest streak at generation time.
- `SummaryText`: deterministic summary message.
- `CreatedAt`, `UpdatedAt`: audit timestamps.

## Status Values

```text
Generated
Stale
Archived
```

The first implementation generates reports as `Generated`. `Stale` is reserved for later background invalidation. `Archived` is supported by the domain and application use case.

## Calculation Rules

Study time is the sum of `LearningActivityLog.MinutesSpent` for the week. Negative minutes cannot be persisted by the activity log domain.

Active days are distinct UTC dates from weekly activity logs.

Completed daily plans count `DailyStudyPlan` records with `Completed` status in the week.

Practice counts use completed `PracticeSession` records where `CompletedAt` falls in the week.

Quiz counts use `QuizAttempt` rows for the authenticated user where `AttemptedAt` falls in the week. Quiz answer keys are never included in report payloads.

Goal counts use completed `LearningGoal` records updated during the week.

Achievement counts use earned `StudentAchievement` records with `EarnedAt` in the week.

Streak values come from the current `StudentStreak` record.

## Safety Rules

Counts are clamped to zero or greater. `AverageQuizScore` is clamped from 0 to 100. `WeekEndDate` must be after or equal to `WeekStartDate`.
