# User API

Base path:

```text
/api/v1/users
```

## Purpose

The User API manages admin users and their role assignments. It is not a public learner-account API.

## Endpoint Protection

| Method | Route | Permission |
| --- | --- | --- |
| `GET` | `/api/v1/users` | `users.read` |
| `GET` | `/api/v1/users/{id}` | `users.read` |
| `POST` | `/api/v1/users` | `users.create` |
| `PUT` | `/api/v1/users/{id}` | `users.update` |
| `POST` | `/api/v1/users/{id}/deactivate` | `users.delete` |
| `POST` | `/api/v1/users/{id}/roles/{roleId}` | `users.update` |
| `DELETE` | `/api/v1/users/{id}/roles/{roleId}` | `users.update` |

## Search Parameters

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Optional search term for email, user name, or display name. |
| `isActive` | bool | Optional active/inactive filter. |
| `pageNumber` | int | Optional page number. Defaults to `1`. |
| `pageSize` | int | Optional page size. Clamped to `1..100`. |

## Create Request

```json
{
  "email": "editor@example.com",
  "userName": "editor",
  "displayName": "Content Editor",
  "password": "ChangeMe123",
  "roleIds": ["87b77ad4-26d5-48d6-9ed1-111111111111"]
}
```

## Update Request

```json
{
  "email": "editor@example.com",
  "userName": "editor",
  "displayName": "Content Editor",
  "isActive": true
}
```

## User Response

```json
{
  "id": "c6e7f25e-bc16-4f24-8c4b-111111111111",
  "email": "editor@example.com",
  "userName": "editor",
  "displayName": "Content Editor",
  "isActive": true,
  "roles": [
    {
      "id": "87b77ad4-26d5-48d6-9ed1-111111111111",
      "name": "ContentEditor",
      "slug": "contenteditor"
    }
  ],
  "createdAt": "2026-07-13T00:00:00+00:00",
  "updatedAt": "2026-07-13T00:00:00+00:00"
}
```

## Search Response

```json
{
  "items": [],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 0,
  "totalPages": 0,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

## Validation Rules

- Email must be unique.
- User name must be unique.
- Password must be at least 8 characters.
- Password must include uppercase, lowercase, and a digit.
- Role ids must reference active roles.

## Security Notes

- Passwords are only accepted on create and are hashed before storage.
- Password hashes are never exposed.
- Deactivation prevents current-user lookup and permission checks from treating the user as active.
- User deletion is modeled as deactivate for this module.
