# Practice Item

## Purpose

`PracticeItem` represents one learner-owned review target for one content record and one practice type.

## Fields

```text
Id
StudentProfileId
ContentType
ContentId
PracticeType
Status
DueAt
LastPracticedAt
NextReviewAt
ReviewCount
CorrectCount
IncorrectCount
CurrentIntervalDays
CreatedAt
UpdatedAt
```

## Rules

- `StudentProfileId` is required.
- `ContentType` is required.
- `ContentId` is required.
- `PracticeType` is required.
- `DueAt` and `NextReviewAt` are required.
- Review, correct, incorrect, and interval counters must not be negative.
- A unique database index prevents duplicates for the same student, content type, content id, and practice type.

## Lifecycle

New generated items are due immediately. When a learner submits a result, the item updates counters and schedules the next review. When suspended, the item is hidden from due practice until resumed.
