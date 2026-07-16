# Notification And Email API

## Purpose

The notification and email API provides the v0.2.0 foundation for in-app notifications and queued email messages. Notification event integration is intentionally incremental; domain workflows can call `INotificationService` and `IEmailSender` as follow-up work.

## Permissions

| Permission | Use |
| --- | --- |
| `notifications.read` | Read the current user's notifications. |
| `notifications.manage` | Search all notifications from admin tooling. |
| `email.read` | Search queued email messages from admin tooling. |
| `email.manage` | Queue email messages. |

SuperAdmin receives all permissions. Admin receives notification and email management permissions. ContentEditor, Reviewer, and Viewer receive `notifications.read`.

## Endpoints

```text
GET  /api/v1/me/notifications
GET  /api/v1/me/notifications/unread-count
POST /api/v1/me/notifications/{id}/read
GET  /api/v1/admin/notifications
GET  /api/v1/admin/email-messages
POST /api/v1/admin/email-messages
```

## Notification Events

Supported notification event values are:

```text
ContentSubmittedForReview
ContentApproved
ContentChangesRequested
ContentPublished
PublishJobCompleted
PublishJobFailed
UserCreated
RoleAssigned
QuizSubmitted
```

## Notes

- Email sending is represented by a development sender that logs messages.
- Production email delivery should replace `IEmailSender` with a provider-specific adapter.
- Notification and email searches are paged and filterable by status.
- API responses must not include secrets, provider credentials, or internal transport details.
