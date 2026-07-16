# Bulk Operation

## Purpose

`BulkOperation` is the parent record for one requested bulk action.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Operation identifier. |
| `OperationType` | Action to run. |
| `ContentType` | Target content type. |
| `Status` | Operation status. |
| `RequestedBy` | Requesting user. |
| `RequestedAt` | Request time. |
| `StartedAt` | Run start time. |
| `CompletedAt` | Finish or cancel time. |
| `TotalItems` | Number of distinct requested content items. |
| `SucceededItems` | Number of item successes. |
| `FailedItems` | Number of item failures. |
| `ErrorMessage` | Operation-level error summary. |
| `Note` | Optional user note. |
| `CategoryId` | Optional category target. |
| `TagIds` | Optional tag targets. |
| `ExportFormat` | Optional export format. |
| `CreatedAt` | Record creation time. |
| `UpdatedAt` | Record update time. |

## Status Values

- `Pending`
- `Running`
- `Completed`
- `Failed`
- `PartiallyCompleted`
- `Cancelled`

## Rules

- Operations start as `Pending`.
- Running persists before item execution begins.
- Duplicate content IDs are deduplicated.
- Completed, failed, partially completed, and cancelled operations set `CompletedAt`.
