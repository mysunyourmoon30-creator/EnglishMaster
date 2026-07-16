# Import Job API

## Endpoints

| Method | Route | Purpose | Permission |
| --- | --- | --- | --- |
| `GET` | `/api/v1/import-jobs` | Search import jobs. | `import.read` |
| `GET` | `/api/v1/import-jobs/{id}` | Get job detail. | `import.read` |
| `GET` | `/api/v1/import-jobs/{id}/rows` | Get parsed rows. | `import.read` |
| `GET` | `/api/v1/import-jobs/{id}/errors` | Get validation errors. | `import.read` |
| `GET` | `/api/v1/import-jobs/{id}/errors/export` | Export validation errors as CSV. | `import.read` |
| `POST` | `/api/v1/import-jobs/upload` | Upload CSV/JSON content into an import job. | `import.upload` |
| `POST` | `/api/v1/import-jobs/{id}/validate` | Validate rows and prepare preview. | `import.validate` |
| `POST` | `/api/v1/import-jobs/{id}/confirm` | Confirm a preview-ready job. | `import.run` |
| `POST` | `/api/v1/import-jobs/{id}/run` | Run a confirmed import. | `import.run` |
| `POST` | `/api/v1/import-jobs/{id}/cancel` | Cancel a non-completed import. | `import.run` |
| `POST` | `/api/v1/import-jobs/{id}/rollback` | Roll back a completed import. | `import.rollback` |

## Upload Request

```json
{
  "importType": "Words",
  "format": "CSV",
  "originalFileName": "words.csv",
  "content": "Text,MeaningTh,CefrLevel\nhello,สวัสดี,A1"
}
```

The API normalizes import type names, requires `CSV` or `JSON`, requires the file extension to match the selected format, strips paths from the original file name, and rejects content larger than 1 MB.

## Search Filters

`GET /api/v1/import-jobs` supports:

- `importType`
- `format`
- `status`
- `pageNumber`
- `pageSize`

## Error Handling

Invalid lifecycle transitions return validation errors. Missing jobs return not found. Internal implementation details and storage paths should not be exposed to callers.

