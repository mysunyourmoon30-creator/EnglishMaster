# Publishing API

The Publishing API exposes admin-oriented endpoints for publish jobs, publish templates, and generated artifacts. Endpoints are thin presentation-layer adapters that call Application command/query handlers and return DTOs, never EF Core entities.

## Supported Values

Source types:

- `Lesson`
- `Course`
- `Book`
- `Quiz`

Formats:

- `Html`
- `Markdown`
- `Pdf`
- `Docx`

Job statuses:

- `Pending`
- `Running`
- `Completed`
- `Failed`
- `Cancelled`

## Publish Job Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/publish-jobs` | Search publish jobs. |
| `GET` | `/api/v1/publish-jobs/{id}` | Get one publish job. |
| `POST` | `/api/v1/publish-jobs` | Create a publish job. |
| `POST` | `/api/v1/publish-jobs/{id}/run` | Run a publish job synchronously. |
| `POST` | `/api/v1/publish-jobs/{id}/cancel` | Cancel a non-completed publish job. |
| `GET` | `/api/v1/publish-jobs/{publishJobId}/artifacts` | List artifacts for a job. |

Search parameters:

| Parameter | Description |
| --- | --- |
| `sourceType` | Optional source type filter. |
| `sourceId` | Optional source id filter. |
| `format` | Optional output format filter. |
| `status` | Optional job status filter. |
| `pageNumber` | Optional page number. Defaults to `1`. |
| `pageSize` | Optional page size. Defaults to `20`, maximum `100`. |
| `sortBy` | `CreatedAt`, `Title`, or `Status`. |
| `sortDirection` | `Asc` or `Desc`. |

Create request:

```json
{
  "sourceType": "Lesson",
  "sourceId": "00000000-0000-0000-0000-000000000001",
  "format": "Markdown",
  "title": "Daily Lesson Export",
  "requestedBy": "admin"
}
```

Create response:

```json
{
  "id": "11111111-1111-1111-1111-111111111111",
  "sourceType": "Lesson",
  "sourceId": "00000000-0000-0000-0000-000000000001",
  "format": "Markdown",
  "status": "Pending",
  "title": "Daily Lesson Export",
  "outputFileName": "",
  "outputPath": "",
  "errorMessage": "",
  "requestedBy": "admin",
  "startedAt": null,
  "completedAt": null,
  "createdAt": "2026-07-13T00:00:00+00:00",
  "updatedAt": "2026-07-13T00:00:00+00:00"
}
```

Run response returns the updated `PublishJobDto`. If output is generated, status becomes `Completed` and output fields are populated. If generation fails in a handled way, status becomes `Failed` and `errorMessage` is populated.

Validation failures return `400 ValidationProblem`. Unknown ids return `404 NotFound`.

## Publish Template Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/publish-templates` | Search publish templates. |
| `GET` | `/api/v1/publish-templates/{id}` | Get one template. |
| `POST` | `/api/v1/publish-templates` | Create a template. |
| `PUT` | `/api/v1/publish-templates/{id}` | Update a template. |
| `DELETE` | `/api/v1/publish-templates/{id}` | Delete a template. |

Search parameters:

- `format`
- `isDefault`
- `isActive`
- `pageNumber`
- `pageSize`

Create request:

```json
{
  "name": "Default Markdown",
  "description": "Simple markdown template",
  "format": "Markdown",
  "templateContent": "# {{title}}",
  "isDefault": true
}
```

Update request:

```json
{
  "name": "Default Markdown",
  "description": "Simple markdown template",
  "format": "Markdown",
  "templateContent": "# {{title}}",
  "isDefault": true,
  "isActive": true
}
```

Template response:

```json
{
  "id": "22222222-2222-2222-2222-222222222222",
  "name": "Default Markdown",
  "slug": "default-markdown",
  "description": "Simple markdown template",
  "format": "Markdown",
  "templateContent": "# {{title}}",
  "isDefault": true,
  "isActive": true,
  "createdAt": "2026-07-13T00:00:00+00:00",
  "updatedAt": "2026-07-13T00:00:00+00:00"
}
```

Only one default template per format is allowed by application validation.

## Published Artifact Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/published-artifacts` | Search artifacts. |
| `GET` | `/api/v1/published-artifacts/{id}` | Get one artifact. |

Search parameters:

- `publishJobId`
- `sourceType`
- `sourceId`
- `format`
- `pageNumber`
- `pageSize`

Artifact response:

```json
{
  "id": "33333333-3333-3333-3333-333333333333",
  "publishJobId": "11111111-1111-1111-1111-111111111111",
  "sourceType": "Lesson",
  "sourceId": "00000000-0000-0000-0000-000000000001",
  "format": "Markdown",
  "fileName": "daily-lesson-export.md",
  "filePath": "publishing/daily-lesson-export.md",
  "publicUrl": "/publishing/daily-lesson-export.md",
  "fileSize": 128,
  "contentType": "text/markdown",
  "createdAt": "2026-07-13T00:00:00+00:00",
  "updatedAt": "2026-07-13T00:00:00+00:00"
}
```

Artifact paths are relative paths and public URLs. Internal absolute server paths are not exposed.

## Rendering Limitations

`Html` and `Markdown` use simple generated content. `Pdf` and `Docx` currently create placeholder text files with `.pdf.txt` and `.docx.txt` names. Real rendering should be introduced only after choosing a rendering library deliberately.

Publish jobs and artifact searches are paginated. Running a publish job is synchronous in the MVP, so large or long-running rendering workloads should wait for a background worker design.
