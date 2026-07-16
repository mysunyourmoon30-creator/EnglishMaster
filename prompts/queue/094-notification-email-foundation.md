Create Notification and Email Foundation for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Content Review Workflow completed.
- Better Publishing completed.
- Student-facing pages completed.
- Student Progress completed.
- Basic Reporting completed.

Goal:
Create a simple notification and email foundation for admin/system events.

Important:
Do not add real paid email provider integration yet.
Do not add SMS.
Do not add push notifications.
Do not add AI.
Do not add marketplace.
Do not redesign architecture.

Scope:
Create notification foundation only.

Notification Types:
- InApp
- Email

Notification Status:
- Pending
- Sent
- Failed
- Read
- Cancelled

Notification Events:
- ContentSubmittedForReview
- ContentApproved
- ContentChangesRequested
- ContentPublished
- PublishJobCompleted
- PublishJobFailed
- UserCreated
- RoleAssigned
- QuizSubmitted

Domain Entities:

1. Notification

Fields:
- Id
- RecipientUserId
- Type
- EventType
- Title
- Message
- Status
- ReadAt
- SentAt
- FailedAt
- ErrorMessage
- CreatedAt
- UpdatedAt

Rules:
- RecipientUserId is required.
- Type is required.
- EventType is required.
- Title is required.
- Message is required.
- Notification starts as Pending.
- Notification can be marked Sent.
- Notification can be marked Failed with ErrorMessage.
- Notification can be marked Read.
- Notification can be Cancelled if not Sent.

2. EmailMessage

Fields:
- Id
- ToEmail
- ToName
- Subject
- Body
- IsHtml
- Status
- SentAt
- FailedAt
- ErrorMessage
- CreatedAt
- UpdatedAt

Rules:
- ToEmail is required.
- Subject is required.
- Body is required.
- EmailMessage starts as Pending.
- EmailMessage can be marked Sent.
- EmailMessage can be marked Failed.

Application Requirements:

Create CQRS-style folders:
- Features/Notifications/Commands
- Features/Notifications/Queries
- Features/Notifications/Dtos
- Features/EmailMessages/Commands
- Features/EmailMessages/Queries
- Features/EmailMessages/Dtos

Use cases:
- CreateNotification
- MarkNotificationAsRead
- MarkNotificationAsSent
- MarkNotificationAsFailed
- GetMyNotifications
- GetUnreadNotificationCount
- SearchNotifications

Email use cases:
- QueueEmailMessage
- MarkEmailAsSent
- MarkEmailAsFailed
- SearchEmailMessages

Service Abstractions:
Create:
- INotificationService
- IEmailSender

Infrastructure Requirements:
- Create EF Core configuration for Notification.
- Create EF Core configuration for EmailMessage.
- Configure indexes:
  - Notification RecipientUserId
  - Notification Status
  - Notification EventType
  - Notification CreatedAt
  - EmailMessage ToEmail
  - EmailMessage Status
  - EmailMessage CreatedAt
- Add migrations if EF Core tools are available.
- Create a development email sender that logs email instead of sending real email.
- Do not store email provider secrets.

API Requirements:

Notification endpoints:
- GET /api/v1/me/notifications
- GET /api/v1/me/notifications/unread-count
- POST /api/v1/me/notifications/{id}/read

Admin notification endpoints:
- GET /api/v1/admin/notifications

Email endpoints:
- GET /api/v1/admin/email-messages

Security:
- /me notifications require authentication.
- Users can only see their own notifications.
- Admin notification/email pages require admin permission.
- Do not expose other users' private notification data to normal users.

Permissions:
Add:
- notifications.read
- notifications.manage
- email.read
- email.manage

Role mapping:
- SuperAdmin: all
- Admin: manage/read
- ContentEditor: read own notifications
- Reviewer: read own notifications
- Viewer: read own notifications

Blazor Requirements:

Student/Public side:
- Add notification indicator if authenticated.
- Create /learn/notifications page.

Admin side:
- Create /admin/notifications
- Create /admin/email-messages

Notification UI:
- List notifications
- Mark as read
- Show unread count
- Loading state
- Empty state
- Error state

Admin UI:
- View notification list
- View email message list
- Filter by status
- Filter by event type

Integration:
Add notification creation for simple existing events if safe:
- Content submitted for review
- Content approved
- Content changes requested
- Publish job failed

If event integration is too large, create service methods and document TODO.

Quality:
- Keep implementation simple.
- Do not overengineer.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Domain entities created
3. Service abstractions created
4. API endpoints created
5. Blazor pages created
6. Permissions added
7. Tests created
8. Build/test result
9. Remaining limitations