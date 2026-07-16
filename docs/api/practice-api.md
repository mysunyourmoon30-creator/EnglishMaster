# Practice API

## Prefix

```text
/api/v1/me/practice
```

All practice endpoints require authentication and operate only on the current learner's practice data.

## Endpoints

```text
GET  /api/v1/me/practice/summary
GET  /api/v1/me/practice/due
POST /api/v1/me/practice/generate
POST /api/v1/me/practice/sessions/start
GET  /api/v1/me/practice/sessions/{id}
POST /api/v1/me/practice/session-items/{id}/submit
POST /api/v1/me/practice/sessions/{id}/complete
GET  /api/v1/me/practice/history
POST /api/v1/me/practice/items/{id}/suspend
POST /api/v1/me/practice/items/{id}/resume
```

## Summary

Returns counts for due, new, reviewing, and mastered items.

## Due Items

Returns bounded due practice items. Suspended items are excluded.

## Generate

Creates missing practice items from eligible content and returns the number created.

## Start Session

Starts a session from due items. If no due items exist, the repository attempts generation before starting.

## Submit Session Item

Request:

```json
{
  "userAnswer": "learner answer",
  "result": "Good"
}
```

Allowed result values:

```text
Again
Hard
Good
Easy
```

Invalid result values return a bad request. Repeated submission does not mutate practice counters.

## Complete Session

Completes a started session and records counts. Already completed sessions cannot be completed again.

## Security Responses

- Unauthenticated requests return unauthorized.
- Requests for another student's data return not found through ownership checks.
- Invalid result values return bad request.

## Limitations

- No public practice API is provided.
- No quiz answer keys are returned by quiz practice items.
- No AI-generated practice content is returned.
