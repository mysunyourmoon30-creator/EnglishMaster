# Import Security

## Permissions

| Permission | Purpose |
| --- | --- |
| `import.read` | Read import jobs, rows, and validation errors. |
| `import.upload` | Upload import content. |
| `import.validate` | Validate uploaded jobs. |
| `import.run` | Confirm, run, and cancel jobs. |
| `import.rollback` | Roll back completed imports. |

## Role Mapping

- SuperAdmin: all import permissions.
- Admin: all import permissions.
- ContentEditor: read, upload, validate, run.
- Reviewer: read, validate.
- Viewer: no import access unless a future read-only admin policy explicitly allows it.

## File Safety

- Only `CSV` and `JSON` are accepted.
- File extension must match the selected format.
- The original filename is passed through `Path.GetFileName` to prevent path traversal.
- Content is stored as text rows and is never executed.
- Request content is limited to 1 MB.
- Internal storage paths are not exposed by the API.

## Authorization Behavior

Unauthenticated users receive `401`. Authenticated users without the required policy receive `403`. API permissions are the security boundary; Blazor page visibility is only a convenience layer.

## Operational Caution

Rollback only deletes records created by the specific job and recorded on its rows. New import types must define rollback behavior before exposing run support.

