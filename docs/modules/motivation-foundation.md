# Motivation Foundation

## Purpose

The motivation foundation adds simple rule-based learner motivation features:

- Learning activity log.
- Student streak.
- Achievement definitions.
- Student achievements.
- Motivation summary.

It intentionally excludes AI motivation, leaderboards, social features, payments, marketplace behavior, and mobile-only flows.

## Activity Types

```text
LessonStarted
LessonCompleted
CourseStarted
CourseCompleted
BookStarted
BookCompleted
QuizAttempted
QuizPassed
PracticeSessionCompleted
DailyStudyPlanCompleted
LearningGoalCompleted
```

## Achievement Types

```text
FirstLessonCompleted
FirstQuizCompleted
FirstQuizPassed
FirstPracticeSession
ThreeDayStreak
SevenDayStreak
FourteenDayStreak
ThirtyDayStreak
TenLessonsCompleted
FirstCourseCompleted
FirstBookCompleted
DailyPlanCompleted
WeeklyGoalCompleted
PracticeItemsCompleted
```

## Known Limitations

- Automatic activity recording is not wired into every workflow yet.
- No EF migration has been generated yet.
- No social or leaderboard features are included.
