# Bulk Operation Admin UI

## Pages

| Page | Purpose |
| --- | --- |
| `/admin/bulk-operations` | Search and filter operations. |
| `/admin/bulk-operations/create` | Create a bulk operation. |
| `/admin/bulk-operations/{id:guid}` | View status, item results, run, and cancel. |

## UI Behavior

The list page filters by operation type, content type, and status.

The create page accepts content IDs manually and supports optional note, category ID, tag IDs, and export format fields.

The detail page shows status, totals, item results, errors, and action buttons for pending operations.

## Known Limitations

- Content list checkbox selection is not implemented yet.
- Manual content ID entry is intentionally simple for the first implementation.
- Long-running operations should wait for a background execution design.
