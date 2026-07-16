# Learning Goals

## Purpose

Learning goals let authenticated learners define simple targets for study habits and learning outcomes.

The implementation is rule based and intentionally avoids AI, machine learning, external calendars, payments, marketplace behavior, mobile-only behavior, and microservices.

## Goal Types

Supported `GoalType` values:

```text
DailyStudyMinutes
WeeklyStudyMinutes
DailyPracticeItems
WeeklyLessonsCompleted
WeeklyQuizAttempts
TargetCefrLevel
CompleteCourse
CompleteBook
```

Unsupported goal types are rejected.

## Fields

```text
Id
StudentProfileId
GoalType
Title
Description
TargetValue
CurrentValue
Unit
TargetDate
Status
CreatedAt
UpdatedAt
```

## Status Values

```text
Active
Completed
Paused
Cancelled
```

## Rules

- A learning goal belongs to one student profile.
- `StudentProfileId`, `GoalType`, and `Title` are required.
- `TargetValue` and `CurrentValue` must not be negative.
- `CurrentValue` is kept within `TargetValue`.
- Goals start as `Active`.
- Active goals can be paused or completed.
- Paused goals can be resumed.
- Active or paused goals can be cancelled.
- Completed and cancelled goals cannot be modified incorrectly.

## Known Limitations

- Goal progress is not automatically synchronized from every learning event yet.
- No EF migration has been generated yet.
- No AI goal suggestion is included.
