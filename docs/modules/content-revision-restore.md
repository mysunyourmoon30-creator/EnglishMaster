# Content Revision Restore Request

## Purpose

`ContentRevisionRestoreRequest` records a controlled request to restore or recover from a prior content revision. It gives reviewers a simple approval trail before any restore action is completed.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Restore request identifier. |
| `ContentRevisionId` | Revision requested for restore. |
| `RequestedBy` | User requesting restore. |
| `RequestedAt` | Request time. |
| `Reason` | Required restore reason. |
| `Status` | Current request status. |
| `ReviewedBy` | Reviewer or admin actor. |
| `ReviewedAt` | Review time. |
| `ReviewNote` | Optional approval or rejection note. |
| `CreatedAt` | Record creation time. |
| `UpdatedAt` | Record update time. |

## Status Values

- `Pending`
- `Approved`
- `Rejected`
- `Completed`
- `Cancelled`

## Workflow

1. A restore request starts as `Pending`.
2. A pending request can be approved.
3. A pending request can be rejected.
4. An approved request can be completed.
5. Rejected or cancelled requests cannot be completed.

The current implementation tracks the restore workflow. Applying the snapshot back to the original content record is intentionally left for a later command-specific implementation.
