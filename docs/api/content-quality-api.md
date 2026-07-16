# Content Quality API

## Endpoints

| Method | Route | Purpose | Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/content-quality/dashboard` | Quality dashboard summary. | `content-quality.read` |
| `GET` | `/api/v1/content-quality/rules` | Search quality rules. | `content-quality.read` |
| `POST` | `/api/v1/content-quality/rules` | Create a quality rule. | `content-quality.manage` |
| `PUT` | `/api/v1/content-quality/rules/{id}` | Update a quality rule. | `content-quality.manage` |
| `POST` | `/api/v1/content-quality/rules/{id}/activate` | Activate a rule. | `content-quality.manage` |
| `POST` | `/api/v1/content-quality/rules/{id}/deactivate` | Deactivate a rule. | `content-quality.manage` |
| `POST` | `/api/v1/content-quality/checks/run` | Run a check from request body. | `content-quality.run` |
| `POST` | `/api/v1/content-quality/checks/{contentType}/{contentId}/run` | Run a check for one content item. | `content-quality.run` |
| `GET` | `/api/v1/content-quality/checks` | Search checks. | `content-quality.read` |
| `GET` | `/api/v1/content-quality/checks/{id}` | Get check detail. | `content-quality.read` |
| `GET` | `/api/v1/content-quality/{contentType}/{contentId}/latest` | Get latest check for content. | `content-quality.read` |
| `GET` | `/api/v1/content-quality/checks/{id}/findings` | Get check findings. | `content-quality.read` |
| `POST` | `/api/v1/content-quality/findings/{id}/resolve` | Mark a finding resolved. | `content-quality.run` |

## Notes

- All endpoints require authentication.
- Content type is normalized for check execution and latest-check lookup.
- The first implementation returns deterministic rule-based findings only.
