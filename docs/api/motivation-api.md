# Motivation API

## Learner Endpoints

```text
GET  /api/v1/me/motivation/summary
GET  /api/v1/me/motivation/activity
GET  /api/v1/me/motivation/activity/recent
POST /api/v1/me/motivation/activity
GET  /api/v1/me/streak
POST /api/v1/me/streak/update
```

All endpoints require authentication and operate only on the current learner.

## Record Activity Request

```json
{
  "activityType": "LessonCompleted",
  "contentType": "lesson",
  "contentId": null,
  "title": "Intro lesson",
  "occurredAt": "2026-07-15T00:00:00Z",
  "minutesSpent": 10,
  "metadataJson": "{\"safe\":true}"
}
```

Invalid activity types and negative minutes return bad request.
