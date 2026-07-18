# Advanced Analytics Operations

## Scope

This document describes v0.3.0 analytics operations for administrators and learners.

## Endpoints

| Endpoint | Access | Purpose |
| --- | --- | --- |
| `GET /api/v1/admin/analytics/overview` | `reports.read` | Aggregate platform learning analytics. |
| `GET /api/v1/me/analytics/overview` | Authenticated | Current learner analytics. |

## Dashboard Checks

- [ ] `/admin/analytics` loads for a user with `reports.read`.
- [ ] `/learn/analytics` loads for an authenticated learner.
- [ ] Admin analytics do not show learner names or emails.
- [ ] Student analytics show only current learner data.
- [ ] Empty learner data renders zero metrics without errors.

## Performance Notes

- Analytics use live read-only EF queries.
- Investigate rollups or caching only after measured dashboard latency becomes a problem.
- Keep date ranges bounded when adding future endpoints.

## Troubleshooting

- If admin analytics returns `403`, confirm the user has `reports.read`.
- If student analytics returns zeros, confirm the learner has progress/activity data in the selected date range.
- If dashboard latency increases, inspect database query plans before adding new visualizations.
