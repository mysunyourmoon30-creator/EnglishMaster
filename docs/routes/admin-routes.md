# Admin Routes

## Purpose

The admin routing layer is the Blazor `/admin/*` surface for managing EnglishMaster content and administration screens. It covers Words, Categories, Tags, Media, Pronunciations, Grammar, Lessons, Courses, Books, Quizzes, Publishing, Reporting, Users, Roles, and Permissions.

Admin pages are presentation-layer code. They call backend APIs through typed API clients and must not access EF Core, Identity stores, or Infrastructure services directly.

Unauthenticated users who open `/admin/*` are redirected to `/login?returnUrl=...`. API endpoints remain the real security boundary and enforce permission policies.

## Route Structure

Most modules follow this pattern:

```text
/admin/{module}
/admin/{module}/create
/admin/{module}/{id:guid}
/admin/{module}/{id:guid}/edit
```

Embedded child records may have fewer routes. Grammar Examples are mostly managed inside Grammar Rules. Minimal Pairs are managed inside Pronunciation detail pages.

## Existing Admin Routes

| Area | List | Create | Detail | Edit |
| --- | --- | --- | --- | --- |
| Dashboard | `/admin` | - | - | - |
| Reports | `/admin/reports` | - | - | - |
| Achievement Definitions | `/admin/achievement-definitions` | `/admin/achievement-definitions/create` | - | `/admin/achievement-definitions/{id:guid}/edit` |
| Notifications | `/admin/notifications` | - | - | - |
| Email Messages | `/admin/email-messages` | - | - | - |
| Content Quality | `/admin/content-quality`, `/admin/content-quality/checks`, `/admin/content-quality/rules` | `/admin/content-quality/rules/create` | `/admin/content-quality/checks/{id:guid}` | `/admin/content-quality/rules/{id:guid}/edit` |
| Content Revisions | `/admin/content-revisions`, `/admin/content-revision-restores` | - | `/admin/content-revisions/{id:guid}`, `/admin/content-revisions/{contentType}/{contentId:guid}`, `/admin/content-revision-restores/{id:guid}` | - |
| Bulk Operations | `/admin/bulk-operations` | `/admin/bulk-operations/create` | `/admin/bulk-operations/{id:guid}` | - |
| Import Jobs | `/admin/import-jobs` | `/admin/import-jobs/upload` | `/admin/import-jobs/{id:guid}`, `/admin/import-jobs/{id:guid}/rows`, `/admin/import-jobs/{id:guid}/errors` | - |
| Words | `/admin/words` | `/admin/words/create` | `/admin/words/{id:guid}` | `/admin/words/{id:guid}/edit` |
| Categories | `/admin/categories` | `/admin/categories/create` | `/admin/categories/{id:guid}` | `/admin/categories/{id:guid}/edit` |
| Tags | `/admin/tags` | `/admin/tags/create` | `/admin/tags/{id:guid}` | `/admin/tags/{id:guid}/edit` |
| Media | `/admin/media` | `/admin/media/create`, `/admin/media/upload` | `/admin/media/{id:guid}` | `/admin/media/{id:guid}/edit` |
| Pronunciations | `/admin/pronunciations` | `/admin/pronunciations/create` | `/admin/pronunciations/{id:guid}` | `/admin/pronunciations/{id:guid}/edit` |
| Grammar Topics | `/admin/grammar-topics` | `/admin/grammar-topics/create` | `/admin/grammar-topics/{id:guid}` | `/admin/grammar-topics/{id:guid}/edit` |
| Grammar Rules | `/admin/grammar-rules` | `/admin/grammar-rules/create` | `/admin/grammar-rules/{id:guid}` | `/admin/grammar-rules/{id:guid}/edit` |
| Grammar Examples | - | - | - | `/admin/grammar-examples/{id:guid}/edit` |
| Lessons | `/admin/lessons` | `/admin/lessons/create` | `/admin/lessons/{id:guid}` | `/admin/lessons/{id:guid}/edit` |
| Courses | `/admin/courses` | `/admin/courses/create` | `/admin/courses/{id:guid}` | `/admin/courses/{id:guid}/edit` |
| Books | `/admin/books` | `/admin/books/create` | `/admin/books/{id:guid}` | `/admin/books/{id:guid}/edit` |
| Quizzes | `/admin/quizzes` | `/admin/quizzes/create` | `/admin/quizzes/{id:guid}` | `/admin/quizzes/{id:guid}/edit` |
| Publish Jobs | `/admin/publishing/jobs` | `/admin/publishing/jobs/create` | `/admin/publishing/jobs/{id:guid}` | - |
| Publish Templates | `/admin/publishing/templates` | `/admin/publishing/templates/create` | - | `/admin/publishing/templates/{id:guid}/edit` |
| Published Artifacts | `/admin/publishing/artifacts` | - | - | - |
| Users | `/admin/users` | `/admin/users/create` | `/admin/users/{id:guid}` | `/admin/users/{id:guid}/edit` |
| Roles | `/admin/roles` | `/admin/roles/create` | `/admin/roles/{id:guid}` | `/admin/roles/{id:guid}/edit` |
| Permissions | `/admin/permissions` | - | - | - |

## Reporting Routes

The admin reporting page is:

```text
/admin/reports
```

The page should be backed by read-only reporting API endpoints under:

```text
/api/v1/reports
```

Reporting routes require `reports.read` and should expose only aggregate operational metrics unless a specific administrative workflow requires more detail.

Recommended reporting API route set:

| Method | Route | Purpose | Required Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/reports/admin-dashboard` | Admin dashboard summary metrics. | `reports.read` |
| `GET` | `/api/v1/reports/content-status` | Content status metrics. | `reports.read` |
| `GET` | `/api/v1/reports/learning-progress` | Learning progress metrics. | `reports.read` |
| `GET` | `/api/v1/reports/quiz-analytics` | Quiz analytics metrics. | `reports.read` |
| `GET` | `/api/v1/reports/recent-activity` | Recent reporting activity. | `reports.read` |

## Reporting Route Privacy And Performance

Reporting routes should return aggregate data by default and avoid unnecessary personal data. Recent activity should be bounded, privacy-aware, and limited to the fields needed by the admin dashboard.

Reporting API routes should avoid unbounded scans and should not load full related entities when counts or summaries are sufficient.

## Notification And Email Routes

Notification and email admin pages are:

```text
/admin/notifications
/admin/email-messages
```

The learner notification page is:

```text
/learn/notifications
```

Recommended backing API route set:

| Method | Route | Purpose | Required Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/me/notifications` | Current user's notifications. | `notifications.read` |
| `GET` | `/api/v1/me/notifications/unread-count` | Current user's unread count. | `notifications.read` |
| `POST` | `/api/v1/me/notifications/{id}/read` | Mark one owned notification read. | `notifications.read` |
| `GET` | `/api/v1/admin/notifications` | Admin notification search. | `notifications.manage` |
| `GET` | `/api/v1/admin/email-messages` | Admin email message search. | `email.read` |
| `POST` | `/api/v1/admin/email-messages` | Queue an email message. | `email.manage` |

## Reporting Routes Intentionally Not Included Yet

Admin reporting routes do not currently include:

- Advanced analytics routes.
- Charts library-specific routes.
- Data warehouse exploration routes.
- AI insights routes.
- Export reporting routes.

## Route Constants

Route constants live in:

```text
src/Frontend/EnglishMaster.Web/Routes/AdminRoutes.cs
```

Use `AdminRoutes` for links and redirects. Keep `@page` route literals in sync by hand because Blazor requires `@page` values to be compile-time literals.

## Admin Navigation

The main admin navigation lives in:

```text
src/Frontend/EnglishMaster.Web/Components/Layout/MainLayout.razor
```

Navigation currently includes Dashboard, content modules, Reports for users with `reports.read`, Notifications for authenticated users, admin notification/email pages for authorized users, Publishing, Users, Roles, and Permissions. API authorization remains required for every protected action.

## Achievement Definition Routes

Achievement definition API routes are:

```text
GET  /api/v1/admin/achievement-definitions
POST /api/v1/admin/achievement-definitions
PUT  /api/v1/admin/achievement-definitions/{id}
POST /api/v1/admin/achievement-definitions/{id}/activate
POST /api/v1/admin/achievement-definitions/{id}/deactivate
POST /api/v1/admin/achievement-definitions/seed-defaults
```

Read requires `achievements.read`; mutations require `achievements.manage`.

## Content Quality Routes

Content Quality admin pages are:

```text
/admin/content-quality
/admin/content-quality/checks
/admin/content-quality/checks/{id:guid}
/admin/content-quality/rules
/admin/content-quality/rules/create
/admin/content-quality/rules/{id:guid}/edit
```

Content Quality API routes are documented in [Content Quality API](../api/content-quality-api.md).

## Content Revision Routes

Content Revision admin pages are:

```text
/admin/content-revisions
/admin/content-revisions/{id:guid}
/admin/content-revisions/{contentType}/{contentId:guid}
/admin/content-revision-restores
/admin/content-revision-restores/{id:guid}
```

Content Revision API routes are documented in [Content Revision API](../api/content-revision-api.md).

Word, Lesson, Course, Book, and Quiz detail pages include a Revision History link to the content timeline route.

## Bulk Operation Routes

Bulk Operation admin pages are:

```text
/admin/bulk-operations
/admin/bulk-operations/create
/admin/bulk-operations/{id:guid}
```

Bulk Operation API routes are documented in [Bulk Operation API](../api/bulk-operation-api.md).

## Import Job Routes

Import Job admin pages are:

```text
/admin/import-jobs
/admin/import-jobs/upload
/admin/import-jobs/{id:guid}
/admin/import-jobs/{id:guid}/rows
/admin/import-jobs/{id:guid}/errors
```

Import Job API routes are documented in [Import Job API](../api/import-job-api.md).

## Admin Route Protection

The Web app middleware blocks unauthenticated `/admin/*` access and redirects to login. After login, the user returns to the requested admin URL when a `returnUrl` is present.

Do not rely on route protection alone. Each admin page must call protected API endpoints, and each endpoint must use the correct permission policy.

## How To Add A New Admin Page

1. Add the `@page "/admin/..."` literal to the Razor page.
2. Add the matching route constant and helper methods to `AdminRoutes.cs`.
3. Add navigation only if the page is a top-level area.
4. Use typed API clients from the Web project.
5. Do not access DbContext or Identity stores from Blazor.
6. Protect the backing API endpoint with `.RequireAuthorization(...)`.
7. Run `dotnet build` and `dotnet test`.

## Known Limitations

- Reporting navigation is permission-aware for `reports.read`.
- Notification/email navigation is permission-aware for notification and email permissions.
- No breadcrumb navigation exists yet.
- Minimal Pair has no standalone admin route.
- Grammar Example has only a standalone edit route.
- Publish jobs and published artifacts do not have edit routes.
- Import upload currently uses pasted CSV/JSON content rather than a browser file picker.

## Related Documentation

- [Authentication](../security/authentication.md)
- [Authorization](../security/authorization.md)
- [Roles And Permissions](../security/roles-and-permissions.md)
- [Reporting Permissions](../security/reporting-permissions.md)
- [Notification Security](../security/notification-security.md)
- [Content Revision Security](../security/content-revision-security.md)
- [Bulk Operation Permissions](../security/bulk-operation-permissions.md)
- [Import Security](../security/import-security.md)
- [Admin Dashboard](../ui/admin-dashboard.md)

## Next Recommended Module

Automatic revision capture for Word, Lesson, Course, Book, Quiz, and PublishTemplate commands is the next recommended admin feature after the documentation baseline.
