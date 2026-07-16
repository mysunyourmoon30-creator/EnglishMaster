# Notification Security

## Permissions

| Permission | Purpose |
| --- | --- |
| `notifications.read` | Read the current user's notification list and unread count. |
| `notifications.manage` | Search all notifications from admin tooling. |
| `email.read` | Search queued email messages from admin tooling. |
| `email.manage` | Queue email messages from admin tooling. |

## Role Mapping

- SuperAdmin receives all notification and email permissions.
- Admin receives notification and email management permissions.
- ContentEditor, Reviewer, and Viewer receive `notifications.read` for their own notification page.

## User Isolation

The `/api/v1/me/notifications` endpoints derive the user id from authenticated claims. They do not accept a recipient id from the request. Mark-read also checks the notification belongs to the current recipient.

## Admin Access

Admin notification and email pages use typed Web API clients. Blazor pages do not access EF Core or Infrastructure services directly. API endpoints remain the security boundary.

## Sensitive Data Rules

- Do not expose password hashes, normalized identity fields, provider credentials, or connection strings.
- Do not include email provider secrets in application settings committed to source control.
- Keep failure messages bounded and avoid raw provider exception dumps.

## Known Limitations

The Testing environment auto-authenticates many non-security API routes for integration testing. Runtime authorization is enforced by permission policies and non-testing middleware.
