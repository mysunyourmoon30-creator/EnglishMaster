# Auth API

Base path:

```text
/api/v1/auth
```

## Purpose

The Auth API signs admin users in and out and returns the current authenticated admin user. It is cookie-based and intended for the admin platform.

## Endpoints

| Method | Route | Auth | Description |
| --- | --- | --- | --- |
| `POST` | `/api/v1/auth/login` | Anonymous | Validate credentials and issue the admin API cookie. |
| `POST` | `/api/v1/auth/logout` | Authenticated | Clear the admin API cookie. |
| `GET` | `/api/v1/auth/me` | Authenticated | Return the current user profile, roles, and permissions. |

## Login Request

```json
{
  "email": "admin@example.com",
  "password": "ChangeMe123"
}
```

## Login Response

```json
{
  "user": {
    "id": "6fdf9c8e-2d4a-4b2f-8c2c-111111111111",
    "email": "admin@example.com",
    "userName": "superadmin",
    "displayName": "Super Admin",
    "roles": ["SuperAdmin"],
    "permissions": ["words.read", "users.read", "roles.read"]
  }
}
```

The real response may include many more permissions depending on assigned roles.

## Current User Response

`GET /api/v1/auth/me` returns the same `CurrentUserDto` shape as `login.user`:

```json
{
  "id": "6fdf9c8e-2d4a-4b2f-8c2c-111111111111",
  "email": "admin@example.com",
  "userName": "superadmin",
  "displayName": "Super Admin",
  "roles": ["SuperAdmin"],
  "permissions": ["words.read", "users.read"]
}
```

## Status Codes

| Status | Meaning |
| --- | --- |
| `200 OK` | Login or current user lookup succeeded. |
| `204 No Content` | Logout succeeded. |
| `400 Bad Request` | Credentials or request values failed validation. |
| `401 Unauthorized` | No valid authenticated user. |
| `404 Not Found` | Current user no longer exists or is inactive. |

## Security Notes

- Password hashes are never returned.
- Invalid login responses do not reveal whether the email exists.
- The cookie is HTTP-only.
- Production should use HTTPS and secure cookies.
- Login throttling and account lockout are not implemented yet.
