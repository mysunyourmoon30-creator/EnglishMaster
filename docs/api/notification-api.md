# Notification API

## Current User Endpoints

| Method | Route | Purpose | Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/me/notifications` | Search the authenticated user's notifications. | `notifications.read` |
| `GET` | `/api/v1/me/notifications/unread-count` | Return unread notification count. | `notifications.read` |
| `POST` | `/api/v1/me/notifications/{id}/read` | Mark one owned notification as read. | `notifications.read` |

The `/me` routes always use the authenticated user's id from claims. A user cannot pass another user id to read or update someone else's notifications.

## Admin Endpoints

| Method | Route | Purpose | Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/admin/notifications` | Search notifications across users. | `notifications.manage` |

Admin notification search supports status and event type filters.

## Response Rules

- Notification responses include notification state and bounded failure details.
- Responses do not include password data, role internals, provider credentials, or raw infrastructure details.
- Validation errors should be returned as validation problem responses.

## Known Limitations

- There is no public notification API.
- Admin create/update notification endpoints are not exposed yet.
