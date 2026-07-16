# ImportValidationError

## Purpose

`ImportValidationError` records a field-level validation issue for an import row. It allows the admin UI and API to show exact row errors and export them as CSV.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Error identifier. |
| `ImportJobRowId` | Owning row identifier. |
| `FieldName` | Field that failed validation. |
| `ErrorCode` | Stable machine-readable error code. |
| `ErrorMessage` | Safe user-facing message. |
| `Severity` | `Info`, `Warning`, `Error`, or `Critical`. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Behavior

Errors with `Error` or `Critical` severity mark the row invalid. Exported errors use CSV with row ID, field name, error code, severity, and message.

