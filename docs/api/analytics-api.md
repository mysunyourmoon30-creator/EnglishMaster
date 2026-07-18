# Analytics API

## Purpose

The analytics API provides v0.3.0 read-only learning analytics aggregates for administrators and authenticated learners.

## Permissions

| Endpoint area | Requirement |
| --- | --- |
| `/api/v1/admin/analytics/*` | `reports.read` |
| `/api/v1/me/analytics/*` | Authenticated user |

There are no public analytics endpoints in v0.3.0.

## Endpoints

```text
GET /api/v1/admin/analytics/overview
GET /api/v1/me/analytics/overview
```

Optional query parameters:

```text
fromDate
toDate
```

If no date range is provided, the API uses the last 30 days. Date ranges are clamped to a maximum of 366 days.

## Admin Overview Response

```json
{
  "activeLearners": 12,
  "studyMinutes": 840,
  "learningActivities": 96,
  "courseCompletions": 8,
  "quizAttempts": 40,
  "averageQuizScore": 82.5,
  "practiceSessionsCompleted": 18,
  "certificatesIssued": 5
}
```

## Student Overview Response

```json
{
  "studyMinutes": 120,
  "learningActivities": 16,
  "lessonsCompleted": 5,
  "coursesCompleted": 1,
  "booksCompleted": 0,
  "quizAttempts": 4,
  "averageQuizScore": 85,
  "practiceSessionsCompleted": 3,
  "certificatesEarned": 1,
  "currentStreakDays": 4,
  "longestStreakDays": 9
}
```

## Data Safety

- Admin analytics are aggregate-only.
- Student analytics use the current authenticated user id from claims.
- The API does not accept a user id for student analytics.
- Quiz answer details are not exposed.
- Learner names and emails are not included in analytics DTOs.
