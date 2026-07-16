# Publish Template

## Purpose

`PublishTemplate` stores reusable template metadata for a publishing format. The current foundation stores templates and enforces template invariants, but the basic HTML/Markdown content builder does not yet apply template content.

## Fields

| Field | Description |
| --- | --- |
| `Id` | Unique template identifier. |
| `Name` | Template name. Required. |
| `Slug` | Generated from `Name`. Unique. |
| `Description` | Optional description. |
| `Format` | Template format: `Html`, `Markdown`, `Pdf`, or `Docx`. |
| `TemplateContent` | Optional template body. |
| `IsDefault` | Indicates the default template for a format. |
| `IsActive` | Enables or disables the template. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Domain Rules

- `Name` is required.
- `Slug` is generated consistently from `Name`.
- `Format` is required.
- `TemplateContent` is optional.
- Templates support `Activate` and `Deactivate`.
- Application validation enforces only one default template per format.

## Application Use Cases

- `CreatePublishTemplate`
- `UpdatePublishTemplate`
- `DeletePublishTemplate`
- `GetPublishTemplateById`
- `SearchPublishTemplates`
- `ActivatePublishTemplate`
- `DeactivatePublishTemplate`

## Search Parameters

- `format`
- `isDefault`
- `isActive`
- `pageNumber`
- `pageSize`

## Admin Pages

- `/admin/publishing/templates`
- `/admin/publishing/templates/create`
- `/admin/publishing/templates/{id:guid}/edit`

## Known Limitation

Templates are not yet applied by the basic content builder. That should be addressed when real rendering is introduced.
