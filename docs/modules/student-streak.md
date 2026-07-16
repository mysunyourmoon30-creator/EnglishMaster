# Student Streak

## Purpose

`StudentStreak` tracks consecutive study days for one learner.

## Fields

```text
Id
StudentProfileId
CurrentStreakDays
LongestStreakDays
LastActivityDate
StreakStartDate
UpdatedAt
CreatedAt
```

## Calculation Rules

- One streak record belongs to one student profile.
- Same-day activity does not increase the streak more than once.
- Consecutive-day activity increases `CurrentStreakDays`.
- Missing one or more days resets `CurrentStreakDays` to 1 on the next qualifying activity.
- Older out-of-order activity does not reduce the current streak.
- Dates are normalized to UTC calendar days.
