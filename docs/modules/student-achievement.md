# Student Achievement

## Purpose

`StudentAchievement` stores one learner's progress toward one achievement definition.

## Fields

```text
Id
StudentProfileId
AchievementDefinitionId
Status
ProgressValue
EarnedAt
CreatedAt
UpdatedAt
```

## Status Values

```text
Locked
InProgress
Earned
```

## Rules

- One student achievement exists per student and achievement definition.
- `ProgressValue` must not be negative.
- `EarnedAt` is set when status becomes `Earned`.
- Earned achievements are not awarded repeatedly.
