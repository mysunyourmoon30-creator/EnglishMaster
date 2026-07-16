# Bulk Operation Permissions

## Permissions

| Permission | Purpose |
| --- | --- |
| `bulk-operations.read` | Search and view operations and item results. |
| `bulk-operations.run` | Create and run operations. |
| `bulk-operations.cancel` | Cancel allowed operations. |

## Underlying Permission Checks

Bulk actions also check the permission that matches the underlying action:

- `RunQualityCheck`: `content-quality.run`
- `Publish`: `publishing.run`
- `Approve`: revision approval or publishing run permission
- `Archive`: content update permission
- `AssignCategory`: category or word update permission
- `AddTags` / `RemoveTags`: tag or word update permission
- `Export`: content read permission

## Role Mapping

- SuperAdmin: all bulk permissions.
- Admin: all bulk permissions.
- ContentEditor: read and run.
- Reviewer: read and run.
- Viewer: no bulk operation permissions.

## Safety Rules

Bulk operations must not give users a shortcut around normal workflow permissions. The API checks permissions on create and run.
