# Daily Study Plan API

## Prefix

```text
/api/v1/me/study-plan
```

All endpoints require authentication and operate on the current learner only.

## Endpoints

```text
GET  /api/v1/me/study-plan/today
POST /api/v1/me/study-plan/today/generate
GET  /api/v1/me/study-plan/date/{date}
GET  /api/v1/me/study-plan/{id}
POST /api/v1/me/study-plan/items/{id}/complete
POST /api/v1/me/study-plan/items/{id}/skip
POST /api/v1/me/study-plan/{id}/complete
GET  /api/v1/me/study-plan/history
GET  /api/v1/me/study-plan/summary
```

## Today

Returns today's existing study plan. If no plan exists, the endpoint returns not found.

## Generate Today

Creates or refreshes today's plan. It does not create a duplicate plan for the same student and day.

## Date Lookup

The `{date}` value should be parseable as a date, for example:

```text
2026-07-15
```

Invalid dates return a bad request.

## Item Completion

Completing an item sets `CompletedAt`, marks the item completed, and updates plan counts.

Skipping an item marks it skipped and does not count it as completed.

## Security Responses

- Unauthenticated requests return unauthorized.
- Another user's study plan returns not found.
- Quiz answers are not returned in plan payloads.
