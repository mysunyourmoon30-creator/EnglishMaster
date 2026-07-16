# Learning Report Module

## Purpose

The learning report module gives authenticated learners a weekly summary of study activity, practice, quizzes, goals, streaks, and achievements.

Reports are intentionally rule based. They do not use AI-generated text, machine learning, external analytics services, payments, leaderboards, or social features.

## Scope

The module includes:

- `WeeklyLearningReport`.
- `WeeklyLearningReportInsight`.
- Authenticated `/api/v1/me/learning-reports` endpoints.
- Learner pages under `/learn/reports`.
- A Weekly Report card on the student dashboard.

## Data Sources

Weekly report generation uses existing learner-owned data:

- `LearningActivityLog` for study time and lesson/course/book activity.
- `DailyStudyPlan` for completed daily plans.
- `PracticeSession` for completed practice sessions and items.
- `QuizAttempt` for quiz attempts, passed quizzes, and average score.
- `LearningGoal` for completed goals.
- `StudentStreak` for current and longest streak.
- `StudentAchievement` for earned achievements.

## Behavior

One report exists per student profile and `WeekStartDate`. Generating the current week returns the existing report when one already exists. Regenerating a report recalculates metrics and replaces insights for the same student-owned report.

Weeks use Monday as the UTC week start. `WeekEndDate` is six days after `WeekStartDate`.

Past reports can be queried by date or history. They can be regenerated through the report id endpoint when the report belongs to the current learner.

## Limitations

- No EF migration has been generated yet for learning report tables.
- Reports are generated on demand, not by a background scheduler.
- Report text is deterministic and simple.
- No AI report generation is included by design.
- No external analytics service is included by design.

## Next Recommended Step

Generate and review EF migrations for the learning report, practice, learning goal, study plan, and motivation tables before database deployment.
