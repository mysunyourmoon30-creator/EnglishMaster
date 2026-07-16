# Bulk Operation Item

## Purpose

`BulkOperationItem` records the result for one content item inside a bulk operation.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Item identifier. |
| `BulkOperationId` | Parent operation. |
| `ContentId` | Target content item. |
| `Status` | Item result status. |
| `ErrorMessage` | Failure or skip reason. |
| `CreatedAt` | Record creation time. |
| `UpdatedAt` | Record update time. |

## Status Values

- `Pending`
- `Success`
- `Failed`
- `Skipped`

## Item Behavior

Items are processed independently. Missing content or unsupported action/content combinations are recorded as `Failed` with a safe error message.
