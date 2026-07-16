# Learning Goal API

## Prefix

```text
/api/v1/me/learning-goals
```

All endpoints require authentication and operate on the current learner only.

## Endpoints

```text
GET  /api/v1/me/learning-goals
GET  /api/v1/me/learning-goals/active
GET  /api/v1/me/learning-goals/summary
GET  /api/v1/me/learning-goals/{id}
POST /api/v1/me/learning-goals
PUT  /api/v1/me/learning-goals/{id}
POST /api/v1/me/learning-goals/{id}/pause
POST /api/v1/me/learning-goals/{id}/resume
POST /api/v1/me/learning-goals/{id}/complete
POST /api/v1/me/learning-goals/{id}/cancel
```

## Create Request

```json
{
  "goalType": "DailyStudyMinutes",
  "title": "Study 30 minutes daily",
  "description": "Build a daily habit",
  "targetValue": 30,
  "unit": "minutes",
  "targetDate": null
}
```

Unsupported goal types return a bad request.

## Update Request

```json
{
  "title": "Study 45 minutes daily",
  "description": "Increase study time",
  "targetValue": 45,
  "currentValue": 10,
  "unit": "minutes",
  "targetDate": null
}
```

## Security Responses

- Unauthenticated requests return unauthorized.
- Another user's goal returns not found.
- Invalid goal data returns bad request.
