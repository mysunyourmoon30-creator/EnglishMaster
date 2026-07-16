# Learning Report Pages

## Routes

```text
/learn/reports
/learn/reports/current-week
/learn/reports/{id}
```

These pages are learner-facing and call authenticated `/api/v1/me/learning-reports` endpoints.

## Reports History

`/learn/reports` shows recent weekly reports with status, week range, study minutes, active days, and quiz average. It also provides a generate-current-week action.

## Current Week

`/learn/reports/current-week` shows the current week report when available and provides a generate action when missing.

## Detail

`/learn/reports/{id}` shows a specific learner-owned report and includes a regenerate action.

## Shared Summary

The report summary displays:

- Study minutes.
- Active days.
- Completed plans.
- Current streak.
- Lessons completed.
- Practice sessions.
- Quiz attempts.
- Quiz average.
- Rule-based insights.

## States

The pages include loading, empty, and error states. Logged-out users cannot retrieve report data from the API and see a safe unavailable state instead of raw errors.
