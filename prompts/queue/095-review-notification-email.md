Review and harden Notification and Email Foundation.

Project:
EnglishMaster

Goal:
Make notification and email foundation secure, simple, buildable, and aligned with Clean Architecture.

Check and fix:

1. Clean Architecture
- Domain must not depend on Infrastructure, API, or Web.
- Notification logic must not be inside API controllers.
- Email sender implementation must stay in Infrastructure.
- Blazor pages must not access DbContext directly.

2. Notification Rules
Verify:
- Notification starts as Pending.
- Notification can be marked Sent.
- Notification can be marked Failed.
- Notification can be marked Read.
- Notification cannot expose another user's data.
- Unread count works correctly.

3. Email Rules
Verify:
- EmailMessage starts as Pending.
- EmailMessage can be marked Sent.
- EmailMessage can be marked Failed.
- Development email sender does not send real emails.
- No email provider secret is committed.

4. Security
Verify:
- /me notifications require authentication.
- User can only access own notifications.
- Admin notification pages require permission.
- Email message admin page requires permission.
- No sensitive user data is exposed unnecessarily.

5. API
Verify:
- GET /api/v1/me/notifications works.
- GET /api/v1/me/notifications/unread-count works.
- POST /api/v1/me/notifications/{id}/read works.
- Admin notification endpoints work.
- Email message endpoints work.
- Proper 401 / 403 behavior.

6. Blazor
Verify:
- /learn/notifications works.
- Notification indicator works if implemented.
- /admin/notifications works.
- /admin/email-messages works.
- Loading, empty, error states exist.

7. Tests
Verify:
- Notification tests pass.
- EmailMessage tests pass.
- Auth/security tests pass.
- Existing modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add SMS.
Do not add push notifications.
Do not add real email provider integration yet.
Do not add AI.

Output:
1. Problems found
2. Fixes applied
3. Notification security result
4. Email safety result
5. Build/test result
6. Remaining risks