# Content Revision

## Purpose

`ContentRevision` is the persisted revision record for one content item at one point in time.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Revision identifier. |
| `ContentType` | Normalized content type, such as `word` or `lesson`. |
| `ContentId` | Identifier of the content item. |
| `RevisionNumber` | Per-content-item revision number starting at 1. |
| `EventType` | Revision event such as `Created`, `Updated`, or `Published`. |
| `Title` | Optional display title. |
| `Summary` | Optional summary of the revision. |
| `ChangedBy` | User or system actor. |
| `ChangedAt` | Time the change occurred. |
| `ChangeReason` | Optional reason or note. |
| `SnapshotJson` | Sanitized JSON snapshot. |
| `DiffJson` | Optional sanitized JSON diff. |
| `CreatedAt` | Record creation time. |
| `UpdatedAt` | Record update time. |

## Rules

- `ContentType` is required.
- `ContentId` must not be empty.
- `RevisionNumber` must be greater than zero.
- `EventType` must be valid.
- `ChangedAt` is always set by the revision service.
- `SnapshotJson` is required and sanitized before persistence.
- `DiffJson` may be empty.

## Indexes

Revision storage indexes content type and content id, revision number, event type, changed by, and changed at for timeline and search workflows.
