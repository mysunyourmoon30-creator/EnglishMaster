# Admin Dashboard

## Purpose

The admin dashboard gives authorized EnglishMaster administrators a compact operational view of the platform. The reporting area should help administrators understand content readiness, learner progress, quiz activity, and recent activity without exposing unnecessary personal data.

## Blazor Admin Reports Page

The Blazor admin reports page should be available at:

```text
/admin/reports
```

Required permission:

- `reports.read`

The page should be read-only and should call the reporting API endpoints rather than duplicating reporting query logic in the UI.

## Dashboard Sections

The admin reports page should include:

- Summary metrics.
- Content status.
- Learning progress.
- Quiz analytics.
- Recent activity.

## Summary Metrics

Summary metrics should present the most important operational numbers in a compact layout.

Current metrics:

- Students and active students.
- Words, lessons, courses, books, and quizzes.
- Published and draft lessons.
- Published courses, books, and quizzes.
- Quiz attempts and recent activity.

## Content Status

The content status section should show:

- Published content count.
- Draft content count.
- Archived or inactive content count.
- Recently updated content count.

This section should help administrators identify content readiness and review needs.

## Learning Progress

The learning progress section should show:

- Active learner count.
- Lessons started.
- Lessons completed.
- Completion rate.
- Average progress percentage.

The UI should prefer aggregate values and avoid showing direct learner identifiers unless needed for an administrative workflow.

## Quiz Analytics

The quiz analytics section should show:

- Quiz attempt count.
- Average score.
- Passed attempts.
- Failed attempts.
- Pass rate.

This is basic quiz reporting only. It does not include item-level analysis, adaptive recommendations, or AI-generated insight.

## Recent Activity

Recent activity should show a bounded newest-first list of operational events.

Examples:

- Content updated.
- Lesson progress recorded.
- Quiz attempt submitted.
- Relevant administrative activity.

Activity descriptions should be concise and privacy-aware.

## UI States

The page should include:

- Loading state while metrics are requested.
- Empty state when no reporting data exists.
- Error state when reporting data cannot be loaded.
- Forbidden state when the current user lacks `reports.read`.

## Data Privacy Rules

The dashboard should:

- Display aggregate data by default.
- Avoid unnecessary learner personal data.
- Avoid showing quiz answers or detailed assessment response data.
- Avoid exposing internal errors or infrastructure details.

## Performance Limitations

The UI should avoid loading large datasets into the browser. Recent activity should be capped or paginated, and metric cards should be driven by aggregate API responses.

## Intentionally Not Included Yet

The admin dashboard does not yet include:

- Advanced analytics.
- Charts library integration.
- Data warehouse dashboards.
- AI insights.
- Export reporting.

## Known Limitations

- The first version should use simple aggregate cards and lists.
- No charting dependency is required.
- No report export action is included.
- No historical trend view is included yet.
- Learning progress and quiz attempt sections show zero or empty states until dedicated tracking entities exist.

## Related Documentation

- [Reporting Module](../modules/reporting-module.md)
- [Reporting API](../api/reporting-api.md)
- [Reporting Permissions](../security/reporting-permissions.md)
- [Admin Routes](../routes/admin-routes.md)
