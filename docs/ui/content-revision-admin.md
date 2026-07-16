# Content Revision Admin UI

## Pages

| Page | Purpose |
| --- | --- |
| `/admin/content-revisions` | Search revision history. |
| `/admin/content-revisions/{id:guid}` | View revision detail and snapshot. |
| `/admin/content-revisions/{contentType}/{contentId}` | View a content item's revision timeline. |
| `/admin/content-revision-restores` | Search restore requests. |
| `/admin/content-revision-restores/{id:guid}` | Review, approve, reject, or complete a restore request. |

## Content Detail Links

Revision History links are available on:

- Word detail
- Lesson detail
- Course detail
- Book detail
- Quiz detail

## UI Behavior

The admin pages use typed Web API clients. They include loading, empty, and error states and do not access EF Core or infrastructure services directly.

The revision detail page displays event type, changed by, changed at, reason, snapshot JSON, and diff JSON when present.

## Known Limitations

- Snapshot display is JSON-first.
- Advanced side-by-side visual diff is intentionally not included yet.
- Restore completion tracks workflow state; command-specific content mutation is deferred.
