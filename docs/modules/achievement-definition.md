# Achievement Definition

## Purpose

`AchievementDefinition` defines a reusable achievement rule.

## Fields

```text
Id
Code
Name
Description
AchievementType
TargetValue
IconName
IsActive
SortOrder
CreatedAt
UpdatedAt
```

## Rules

- `Code` is required and unique.
- `Name` and `AchievementType` are required.
- `TargetValue` and `SortOrder` must not be negative.
- Inactive definitions are not newly awarded.
- Default seed is idempotent.
