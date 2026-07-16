# Role API

Base paths:

```text
/api/v1/roles
/api/v1/permissions
```

## Purpose

The Role API manages admin roles and role-permission assignments. The Permission API exposes the centralized permission catalog.

## Endpoint Protection

| Method | Route | Permission |
| --- | --- | --- |
| `GET` | `/api/v1/roles` | `roles.read` |
| `GET` | `/api/v1/roles/{id}` | `roles.read` |
| `POST` | `/api/v1/roles` | `roles.create` |
| `PUT` | `/api/v1/roles/{id}` | `roles.update` |
| `DELETE` | `/api/v1/roles/{id}` | `roles.delete` |
| `GET` | `/api/v1/roles/{id}/permissions` | `roles.read` |
| `POST` | `/api/v1/roles/{id}/permissions` | `permissions.update` |
| `DELETE` | `/api/v1/roles/{id}/permissions/{permissionKey}` | `permissions.update` |
| `GET` | `/api/v1/permissions` | `permissions.read` |

## Role Search Parameters

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Optional search term for role name or description. |
| `isActive` | bool | Optional active/inactive filter. |
| `pageNumber` | int | Optional page number. Defaults to `1`. |
| `pageSize` | int | Optional page size. Clamped to `1..100`. |

## Create Role Request

```json
{
  "name": "CurriculumManager",
  "description": "Manages content publishing and review."
}
```

## Update Role Request

```json
{
  "name": "CurriculumManager",
  "description": "Manages content publishing and review.",
  "isActive": true
}
```

## Role Response

```json
{
  "id": "e31bbfdb-c825-4ea0-91fb-111111111111",
  "name": "Viewer",
  "slug": "viewer",
  "description": "Viewer role",
  "isSystem": true,
  "isActive": true,
  "permissions": ["words.read", "publishing.read"],
  "createdAt": "2026-07-13T00:00:00+00:00",
  "updatedAt": "2026-07-13T00:00:00+00:00"
}
```

## Assign Permission Request

```json
{
  "permissionKey": "words.read"
}
```

## Permission Response

```json
{
  "key": "words.read",
  "name": "Words Read",
  "module": "Words",
  "description": "Allows words read operations."
}
```

## Default Roles

- `SuperAdmin`
- `Admin`
- `ContentEditor`
- `Reviewer`
- `Viewer`

See [Roles And Permissions](../security/roles-and-permissions.md) for the full permission list and default mapping.

## Security Notes

- System roles cannot be deleted.
- System roles cannot be deactivated through the current role update rule.
- Permission keys must exist in the centralized permission catalog.
- Role-permission changes may require affected users to log in again before claims refresh.
