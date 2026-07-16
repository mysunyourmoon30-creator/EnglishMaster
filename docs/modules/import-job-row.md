# ImportJobRow

## Purpose

`ImportJobRow` stores one parsed row from an import job. It keeps the original row JSON, validated parsed JSON, row status, row-level error summary, and any entity IDs created or updated during import.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Row identifier. |
| `ImportJobId` | Owning job identifier. |
| `RowNumber` | 1-based source row number. |
| `RawDataJson` | Stored raw row data as JSON. |
| `ParsedDataJson` | Normalized row data after validation. |
| `Status` | `Pending`, `Valid`, `Invalid`, `Imported`, `Failed`, `RolledBack`, or `Skipped`. |
| `ErrorMessage` | Safe row error summary. |
| `CreatedEntityType` | Entity type created by this row, if any. |
| `CreatedEntityId` | Entity ID created by this row, if any. |
| `UpdatedEntityType` | Entity type updated by this row, if any. |
| `UpdatedEntityId` | Entity ID updated by this row, if any. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Rules

- A row belongs to exactly one `ImportJob`.
- `RowNumber` must be greater than zero.
- `RawDataJson` is required.
- Invalid rows must have an error message and one or more validation errors when possible.
- Failed rows should not crash the whole job unless the import action requires all-or-nothing behavior.

