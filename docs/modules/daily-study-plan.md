# Daily Study Plan

## Purpose

The daily study plan gives authenticated learners a short rule-based list of tasks for today.

It combines due practice, continue learning, recommendation fallback, and weak quiz review without external calendar integration.

## DailyStudyPlan Fields

```text
Id
StudentProfileId
PlanDate
Status
TargetMinutes
CompletedMinutes
TotalItems
CompletedItems
CreatedAt
UpdatedAt
```

## DailyStudyPlanItem Fields

```text
Id
DailyStudyPlanId
ItemType
ContentType
ContentId
Title
Url
EstimatedMinutes
SortOrder
Status
CompletedAt
CreatedAt
UpdatedAt
```

## Plan Status Values

```text
Planned
InProgress
Completed
Skipped
```

## Item Status Values

```text
Pending
InProgress
Completed
Skipped
```

## Item Types

```text
ContinueLesson
ContinueCourse
PracticeDueItems
ReviewWeakQuiz
RecommendedLesson
RecommendedWords
RecommendedGrammar
TakeQuiz
```

The first implementation generates due practice, continue lesson, recommended lesson, and weak quiz review blocks.

## Rules

- One plan is created per student per UTC day.
- `TargetMinutes` defaults to 30 unless an active `DailyStudyMinutes` goal exists.
- Completed item counts and completed minutes are recalculated from item status.
- Skipped items do not count as completed.
- Existing same-day plans can be refreshed with a due-practice block without creating a duplicate plan.

## Known Limitations

- Plan generation is intentionally simple.
- Course/book continuation and richer recommendation blocks are deferred.
- External calendar sync is intentionally excluded.
