# Import Admin UI

## Pages

| Page | Route |
| --- | --- |
| Import job list | `/admin/import-jobs` |
| Upload import job | `/admin/import-jobs/upload` |
| Import job detail | `/admin/import-jobs/{id:guid}` |
| Import job rows | `/admin/import-jobs/{id:guid}/rows` |
| Import validation errors | `/admin/import-jobs/{id:guid}/errors` |

## Job List

The list page supports filtering by import type, format, and status, with pagination. It links to job detail pages for inspection.

## Upload

The upload page allows admins to choose import type, choose format, provide an original filename, and paste CSV or JSON content. It submits to the import job API.

## Detail And Preview

The detail page shows status, row counts, timestamps, and available actions. Actions are shown according to the job state:

- Validate from `Uploaded`.
- Confirm from `PreviewReady`.
- Run from `Confirmed`.
- Cancel before completion.
- Rollback from `Completed`.
- Export errors when validation errors exist.

## Rows And Errors

The rows page shows raw and parsed row data. The errors page shows row-level validation errors and links to the CSV export endpoint.

## Known UI Limitations

- Upload uses pasted body content rather than a browser file picker.
- Permission-specific button hiding is basic; API permissions remain authoritative.
- There is no background progress view because import runs synchronously.

