# Publish Job

## Purpose

`PublishJob` represents a request to export an existing Lesson, Course, Book, or Quiz into a supported output format.

## Fields

| Field | Description |
| --- | --- |
| `Id` | Unique job identifier. |
| `SourceType` | Source content type: `Lesson`, `Course`, `Book`, or `Quiz`. |
| `SourceId` | Identifier of the source content record. |
| `Format` | Output format: `Html`, `Markdown`, `Pdf`, or `Docx`. |
| `Status` | Current job status. |
| `Title` | Human-readable job title. Required. |
| `OutputFileName` | Generated output file name. Set on completion. |
| `OutputPath` | Relative generated output path. Set on completion. |
| `ErrorMessage` | Failure reason. Set when failed. |
| `RequestedBy` | Optional user/operator label. |
| `StartedAt` | First time the job moved to running. |
| `CompletedAt` | Time the job completed, failed, or was cancelled. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Status Values

- `Pending`
- `Running`
- `Completed`
- `Failed`
- `Cancelled`

## Domain Rules

- New jobs start as `Pending`.
- `SourceType`, `SourceId`, `Format`, `Status`, and `Title` are required.
- `MarkRunning` is blocked for completed or cancelled jobs.
- `MarkCompleted` is blocked for cancelled jobs and requires output file information.
- `MarkFailed` is blocked for completed jobs and requires `ErrorMessage`.
- `Cancel` is blocked for completed jobs.
- `CompletedAt` is set when a job is completed, failed, or cancelled.

## Application Use Cases

- `CreatePublishJob`
- `StartPublishJob`
- `CompletePublishJob`
- `FailPublishJob`
- `CancelPublishJob`
- `RunPublishJob`
- `GetPublishJobById`
- `SearchPublishJobs`

`RunPublishJob` calls `IPublishingService`, which coordinates content building, file storage, artifact creation, and lifecycle updates.

## Search Parameters

- `sourceType`
- `sourceId`
- `format`
- `status`
- `pageNumber`
- `pageSize`
- `sortBy`: `CreatedAt`, `Title`, `Status`
- `sortDirection`: `Asc`, `Desc`

## Admin Pages

- `/admin/publishing/jobs`
- `/admin/publishing/jobs/create`
- `/admin/publishing/jobs/{id:guid}`
