# Reporting Module

## Purpose

The Reporting Module provides basic administrative visibility into EnglishMaster platform activity. It gives authorized administrators a read-only summary of content status, learning progress, quiz activity, and recent operational activity.

The module is intended for lightweight operational monitoring and release readiness checks. It is not a full analytics platform.

## Scope

The reporting module covers:

- Admin dashboard metrics.
- Content status metrics.
- Learning progress metrics.
- Quiz analytics metrics.
- Recent activity.
- Read-only reporting APIs protected by `reports.read`.
- Blazor admin reports page.

## Admin Dashboard Metrics

The admin dashboard should summarize the current state of the system using aggregate metrics that are safe for administrators to view.

Current dashboard metrics:

- Total students and active students.
- Total words, lessons, courses, books, and quizzes.
- Published and draft lesson counts.
- Published course, book, and quiz counts.
- Quiz attempt count and average score when attempt data exists.
- Recent user, content, learning, and quiz activity.

## Content Status Metrics

Content status reporting tracks the editorial state of learning material.

Current metrics:

- Published lessons, courses, books, and quizzes.
- Draft lessons.
- In-review content count when a review status exists.

These metrics help administrators identify content that may need review before broader release.

## Learning Progress Metrics

Learning progress reporting summarizes learner engagement and completion behavior.

Expected metrics when tracking data exists:

- Learners with active progress.
- Lessons started.
- Lessons completed.
- Completion rate.
- Average progress percentage.
- Recent learning activity.

Learning progress metrics should be aggregate-first. Individual learner detail should only be exposed when there is a clear administrative need and the caller has the required permission.

## Quiz Analytics Metrics

Quiz analytics reporting provides basic aggregate insight into assessment activity.

Expected metrics when quiz attempt data exists:

- Total quiz attempts.
- Average quiz score.
- Passed attempts.
- Failed attempts.
- Pass rate.
- Recent quiz attempts.

The current scope is operational reporting, not item analysis, psychometric analysis, adaptive learning, or learner recommendations.

## Recent Activity

Recent activity should show a short read-only timeline of important administrative events.

Recommended activity types:

- Content created or updated.
- Learning progress recorded.
- Quiz attempt submitted.
- Administrative changes relevant to reporting.

Recent activity entries should avoid exposing sensitive personal data unless it is required for the administrative workflow.

## Data Privacy Rules

Reporting should follow aggregate-first privacy rules:

- Prefer counts, percentages, and summary values over individual records.
- Avoid learner names, email addresses, and direct identifiers unless required for an administrative workflow.
- Do not expose quiz answers, answer history, or sensitive assessment details in dashboard summaries.
- Bound recent activity responses to the minimum useful fields.
- Do not include secrets, tokens, internal exception details, or infrastructure identifiers.

## Performance Limitations

Reporting queries should be treated as read-only aggregate queries over application data. For the first version, avoid expensive unbounded scans and avoid loading full entity graphs when counts or summaries are sufficient.

If reporting latency becomes noticeable, recommended follow-up work includes query profiling, database indexes, pagination for activity feeds, optional summary tables, and background aggregation.

## Intentionally Not Included Yet

The reporting module intentionally does not include:

- Advanced analytics.
- Charts library integration.
- Data warehouse integration.
- AI insights.
- Export reporting.

These capabilities should be considered future enhancements after the basic reporting model, permissions, and API contracts are stable.

## Known Limitations

- Metrics are designed for basic operational visibility only.
- Reports may use live application data and may become slower as data volume grows.
- No dedicated reporting database, warehouse, or cache layer is assumed.
- No historical trend dashboard is included yet.
- No export workflow is included yet.
- No charting dependency is required for the first version.
- Learning progress and quiz attempt summaries currently return zero or empty collections until dedicated tracking entities are available.

## Related Documentation

- [Reporting API](../api/reporting-api.md)
- [Reporting Permissions](../security/reporting-permissions.md)
- [Admin Routes](../routes/admin-routes.md)
- [Admin Dashboard](../ui/admin-dashboard.md)
