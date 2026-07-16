# Weekly Learning Report Insight

## Purpose

`WeeklyLearningReportInsight` stores deterministic guidance derived from a weekly report.

Insights are rule based only. They do not use AI, external analytics, external scoring, or generated natural-language services.

## Fields

- `Id`: insight identifier.
- `WeeklyLearningReportId`: owning report.
- `InsightType`: rule category.
- `Severity`: display severity.
- `Message`: learner-facing message.
- `Recommendation`: learner-facing next step.
- `SortOrder`: display order.
- `CreatedAt`, `UpdatedAt`: audit timestamps.

## Insight Types

```text
StudyTime
Practice
Quiz
Goal
Streak
Achievement
Inactivity
```

## Severity Values

```text
Info
Warning
Positive
```

The current rules use `Warning` for low or missing activity and `Positive` for streak, achievement, and goal progress.

## Rule Behavior

- `Inactivity`: added when active study days are zero.
- `StudyTime`: added when weekly study minutes are below 60.
- `Practice`: added when practice items are due.
- `Quiz`: added when quiz attempts exist and the average score is below 70.
- `Streak`: added when current streak days are greater than zero.
- `Achievement`: added when achievements were earned during the week.
- `Goal`: added when goals were completed during the week.

## Limitations

Insight thresholds are simple constants. They should stay deterministic until product requirements define personalization, learner preferences, or reviewed analytics behavior.
