# Reporting API

Base path:

```text
/api/v1/reports
```

## Purpose

The Reporting API exposes read-only administrative reporting data for the EnglishMaster Blazor admin dashboard. It supports basic operational reporting for content status, learning progress, quiz analytics, and recent activity.

All reporting endpoints must require the `reports.read` permission.

## Authorization

Required permission:

- `reports.read`

The API must not return reporting data to anonymous users or authenticated users who do not have this permission.

## Endpoints

| Method | Route | Purpose | Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/reports/admin-dashboard` | Returns admin dashboard summary metrics. | `reports.read` |
| `GET` | `/api/v1/reports/content-status` | Returns content status metrics. | `reports.read` |
| `GET` | `/api/v1/reports/learning-progress` | Returns learning progress metrics. | `reports.read` |
| `GET` | `/api/v1/reports/quiz-analytics` | Returns quiz analytics metrics. | `reports.read` |
| `GET` | `/api/v1/reports/recent-activity` | Returns recent reporting activity. | `reports.read` |

## Dashboard Response

The dashboard endpoint should return a compact aggregate summary for the admin dashboard.

Example response shape:

```json
{
  "overview": {
    "totalStudents": 0,
    "totalActiveStudents": 0,
    "totalWords": 0,
    "totalLessons": 0,
    "totalCourses": 0,
    "totalBooks": 0,
    "totalQuizzes": 0,
    "totalQuizAttempts": 0,
    "averageQuizScore": 0
  },
  "contentStatus": {
    "publishedLessons": 0,
    "draftLessons": 0,
    "inReviewContent": 0,
    "publishedCourses": 0,
    "publishedBooks": 0,
    "publishedQuizzes": 0
  },
  "quizAnalytics": {
    "totalAttempts": 0,
    "averageScore": 0,
    "passedCount": 0,
    "failedCount": 0,
    "averageScoreByQuiz": [],
    "recentAttempts": [],
    "topQuizzesByAttempts": []
  },
  "recentActivity": []
}
```

## Content Status Metrics

Content status responses should include aggregate counts for:

- Published content.
- Draft content.
- Published lessons, courses, books, and quizzes.
- Draft lessons.
- In-review content when an explicit review status exists.

## Learning Progress Metrics

Learning progress responses should include aggregate counts and percentages for:

- Most accessed lessons when tracking data exists.
- Recently started lessons when tracking data exists.
- Recently completed lessons when tracking data exists.
- Course and book progress summaries when tracking data exists.

## Quiz Analytics Metrics

Quiz analytics responses should include aggregate values for:

- Quiz attempts.
- Average score.
- Passed attempts.
- Failed attempts.
- Recent quiz attempts.

## Recent Activity

Recent activity responses should be bounded and sorted newest first. Activity items should include only the data needed to identify the event in the admin dashboard.

Recommended fields:

- Activity type.
- Timestamp.
- Short description.
- Related entity type.
- Related entity identifier when safe to expose.

## Data Privacy Rules

Reporting APIs should:

- Return aggregate data by default.
- Avoid exposing learner names, email addresses, or other direct identifiers unless required.
- Avoid exposing quiz answers or detailed assessment responses.
- Avoid returning free-form user content unless reviewed for administrative display.
- Use permission checks consistently on every endpoint.

## Performance Limitations

The first version of reporting should avoid large unbounded result sets. Recent activity should be paginated or capped. Aggregate queries should select only required fields and should not load full related entities unless necessary.

If reporting data grows, consider indexes, summary tables, background aggregation, or a dedicated reporting store.

## Intentionally Not Included Yet

The reporting API does not yet include:

- Advanced analytics.
- Charts library-specific response contracts.
- Data warehouse integration.
- AI insights.
- Export reporting.

## Known Limitations

- Learning progress and quiz attempt values return zero or empty collections until dedicated progress and quiz attempt entities are introduced.
- No charting-specific payload shape is required.
- No export endpoint is included.
- No historical trend or time-series endpoint is included yet.
