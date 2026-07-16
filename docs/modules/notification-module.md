# Notification Module

## Purpose

The Notification module stores in-app and email-intent notifications for EnglishMaster users. It gives content, publishing, security, and quiz workflows a shared foundation for notifying users without embedding notification logic inside API endpoints or Blazor pages.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Notification identifier. |
| `RecipientUserId` | User who can see the notification. |
| `Type` | Delivery category. |
| `EventType` | Business event that created the notification. |
| `Title` | Short user-facing title. |
| `Message` | User-facing message body. |
| `Status` | Processing/read state. |
| `ReadAt` | When the recipient marked it read. |
| `SentAt` | When the notification was marked sent. |
| `FailedAt` | When delivery or processing failed. |
| `ErrorMessage` | Bounded failure detail. |
| `CreatedAt` / `UpdatedAt` | Audit timestamps. |

## Type Values

- `InApp`
- `Email`

## Status Values

- `Pending`
- `Sent`
- `Failed`
- `Read`
- `Cancelled`

## Event Type Values

- `ContentSubmittedForReview`
- `ContentApproved`
- `ContentChangesRequested`
- `ContentPublished`
- `PublishJobCompleted`
- `PublishJobFailed`
- `UserCreated`
- `RoleAssigned`
- `QuizSubmitted`

## Application Rules

- New notifications start as `Pending`.
- Current-user queries are scoped to the authenticated user's id.
- Mark-read only updates notifications owned by the current user.
- Admin search requires `notifications.manage`.
- Other modules should create notifications through `INotificationService`.

## UI

- `/learn/notifications` shows the authenticated user's notifications and unread count.
- `/admin/notifications` lets administrators search notification records.
- The main navigation shows a notification unread badge when available.

## Known Limitations

- Workflow events are not fully wired to create notifications yet.
- No SMS or push notification channel is included.
- Notification preferences are not included.
