# ImportJob

## Purpose

`ImportJob` is the aggregate root for an advanced import run. It records the requested import type, file metadata, lifecycle status, row counts, requester, timestamps, and error summary.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Job identifier. |
| `ImportType` | Normalized import target such as `words` or `lessons`. |
| `Format` | `CSV` or `JSON`. |
| `Status` | Current lifecycle status. |
| `FileName` | Safe internal file name, not exposed as a storage path. |
| `OriginalFileName` | Original user-supplied file name after path stripping. |
| `FileSize` | Uploaded content size in bytes. |
| `RequestedBy` | User name recorded for audit context. |
| `RequestedAt` | Upload timestamp. |
| `ValidatedAt` | Validation completion timestamp. |
| `ConfirmedAt` | Confirmation timestamp. |
| `CompletedAt` | Successful completion timestamp. |
| `FailedAt` | Failed import timestamp. |
| `RolledBackAt` | Rollback timestamp. |
| `TotalRows` | Number of parsed rows. |
| `ValidRows` | Number of valid rows after validation. |
| `InvalidRows` | Number of invalid rows after validation. |
| `ImportedRows` | Number of imported rows after run. |
| `FailedRows` | Number of failed rows after run. |
| `ErrorMessage` | Safe summary message. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Rules

- Import type, format, file name, original file name, and status are required.
- File size must be greater than zero.
- New jobs start as `Uploaded`.
- Validation can only start from `Uploaded`.
- Confirmation requires `PreviewReady`.
- Run requires `Confirmed`.
- Rollback requires `Completed`.

