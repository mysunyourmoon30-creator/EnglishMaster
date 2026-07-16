# Bulk Operation API

## Endpoints

| Method | Route | Purpose | Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/bulk-operations` | Search bulk operations. | `bulk-operations.read` |
| `GET` | `/api/v1/bulk-operations/{id}` | Get operation detail. | `bulk-operations.read` |
| `GET` | `/api/v1/bulk-operations/{id}/items` | Get item results. | `bulk-operations.read` |
| `POST` | `/api/v1/bulk-operations` | Create operation. | `bulk-operations.run` plus underlying permission. |
| `POST` | `/api/v1/bulk-operations/{id}/run` | Run pending operation. | `bulk-operations.run` plus underlying permission. |
| `POST` | `/api/v1/bulk-operations/{id}/cancel` | Cancel pending/running operation. | `bulk-operations.cancel` |

## Create Request

The create request supports:

- `operationType`
- `contentType`
- `contentIds`
- `note`
- `categoryId`
- `tagIds`
- `exportFormat`

## Validation Notes

- At least one content ID is required.
- Invalid operation types return validation errors.
- Duplicate content IDs are deduplicated.
- Missing items are recorded as item failures during run.
- Internal exception details should not be exposed to clients.
