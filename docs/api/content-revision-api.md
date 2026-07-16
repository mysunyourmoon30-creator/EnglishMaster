# Content Revision API

## Revision Endpoints

| Method | Route | Purpose | Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/content-revisions` | Search revisions. | `content-revisions.read` |
| `POST` | `/api/v1/content-revisions` | Create a revision. | `content-revisions.manage` |
| `GET` | `/api/v1/content-revisions/{id}` | Get revision detail. | `content-revisions.read` |
| `GET` | `/api/v1/content-revisions/{contentType}/{contentId}` | Get revisions for content. | `content-revisions.read` |
| `GET` | `/api/v1/content-revisions/{contentType}/{contentId}/latest` | Get latest revision for content. | `content-revisions.read` |

## Restore Request Endpoints

| Method | Route | Purpose | Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/content-revision-restore-requests` | Search restore requests. | `content-revisions.read` |
| `GET` | `/api/v1/content-revision-restore-requests/{id}` | Get restore request detail. | `content-revisions.read` |
| `POST` | `/api/v1/content-revision-restore-requests` | Create restore request. | `content-revisions.restore.request` |
| `POST` | `/api/v1/content-revision-restore-requests/{id}/approve` | Approve restore request. | `content-revisions.restore.approve` |
| `POST` | `/api/v1/content-revision-restore-requests/{id}/reject` | Reject restore request. | `content-revisions.restore.approve` |
| `POST` | `/api/v1/content-revision-restore-requests/{id}/complete` | Complete approved restore request. | `content-revisions.manage` |

## Validation Notes

- Invalid event types return validation errors.
- Missing revisions return not found.
- Invalid restore state transitions return validation errors.
- Snapshot and diff JSON are sanitized before persistence.
- Internal exception details should not be exposed to clients.
