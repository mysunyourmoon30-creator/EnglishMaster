# Learning Report API

## Prefix

```text
/api/v1/me/learning-reports
```

All endpoints require authentication and operate only on the current learner's reports.

## Endpoints

```text
GET  /api/v1/me/learning-reports/current-week
GET  /api/v1/me/learning-reports?pageNumber=1&pageSize=20&fromDate=2026-07-01&toDate=2026-07-31
GET  /api/v1/me/learning-reports/{id}
GET  /api/v1/me/learning-reports/by-date/{date}
POST /api/v1/me/learning-reports/current-week/generate
POST /api/v1/me/learning-reports/{id}/regenerate
GET  /api/v1/me/learning-reports/{id}/insights
```

`by-date/{date}` uses `yyyy-MM-dd`.

## Current Week

Returns the current learner's generated report for the current UTC week. If no report has been generated yet, the endpoint returns not found.

## History

Returns a paginated list of reports for the current learner. `pageNumber` is clamped to 1 or greater. `pageSize` is clamped from 1 to 50.

## Generate

Creates the current week report when missing, or returns the existing report for the same learner and week.

## Regenerate

Recalculates an existing current or past report by id. The report id must belong to the current learner.

## Insights

Returns the stored insights for a learner-owned report.

## Security Responses

- Unauthenticated requests return unauthorized.
- Requests for another student's report return not found through ownership checks.
- Invalid by-date route values return bad request.

## Privacy

Report DTOs do not include quiz answer keys, user secrets, admin-only fields, raw metadata, or internal exception details.
