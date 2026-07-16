# Authorization

## Purpose

Authorization protects the EnglishMaster admin surface and backend API operations with permission-based policies. It is intentionally enforced at the API boundary and not only by hiding Blazor navigation links.

## Policy Model

Each permission key in `Permissions.All` is registered as an ASP.NET Core authorization policy. Policies require a matching `permission` claim on the authenticated principal.

Examples:

- `words.read`
- `words.create`
- `users.update`
- `publishing.run`

The claim type is centralized as `SecurityPermissionClaimTypes.Permission`.

## Admin Route Protection

The Blazor Web app protects `/admin/*` with middleware. If an unauthenticated user opens an admin route, the app redirects to:

```text
/login?returnUrl={original-admin-url}
```

Admin pages still call protected API endpoints. UI route protection is helpful for user experience, but API authorization remains the real security boundary.

## API Endpoint Protection

API routes under `/api/v1` require authentication except login. Module endpoints also use route-level policies for read/create/update/delete operations.

Security endpoints use these policies:

| Area | Endpoint Pattern | Policy |
| --- | --- | --- |
| Auth login | `POST /api/v1/auth/login` | Anonymous |
| Auth logout | `POST /api/v1/auth/logout` | Authenticated |
| Current user | `GET /api/v1/auth/me` | Authenticated |
| Users read | `GET /api/v1/users`, `GET /api/v1/users/{id}` | `users.read` |
| Users create | `POST /api/v1/users` | `users.create` |
| Users update | `PUT /api/v1/users/{id}`, role assignment | `users.update` |
| Users delete/deactivate | `POST /api/v1/users/{id}/deactivate` | `users.delete` |
| Roles read | `GET /api/v1/roles`, `GET /api/v1/roles/{id}` | `roles.read` |
| Roles create | `POST /api/v1/roles` | `roles.create` |
| Roles update | `PUT /api/v1/roles/{id}` | `roles.update` |
| Roles delete | `DELETE /api/v1/roles/{id}` | `roles.delete` |
| Role permissions update | assign/remove permission | `permissions.update` |
| Permission list | `GET /api/v1/permissions` | `permissions.read` |

Content endpoints follow their module permission set. For example, `GET /api/v1/words` requires `words.read`, `POST /api/v1/words` requires `words.create`, and `DELETE /api/v1/words/{id}` requires `words.delete`.

## HTTP Status Behavior

- Unauthenticated requests return `401 Unauthorized`.
- Authenticated users without the required permission return `403 Forbidden`.
- Validation failures return validation problem details.
- Internal exceptions are handled by the API exception handler outside Development and Testing.

## How To Protect A New API Endpoint

1. Add a permission key to `Permissions`.
2. Add the permission to `Permissions.All`.
3. Assign it to the correct default roles in the security service seeding logic.
4. Add `.RequireAuthorization(Permissions.YourPermission)` to the endpoint mapping.
5. Keep the endpoint thin: bind request data, call the Application handler, map the result.
6. Add or update integration tests for unauthorized, forbidden, and allowed flows where practical.

Example:

```csharp
group.MapPost("", CreateAsync)
    .RequireAuthorization(Permissions.ExampleCreate);
```

## How To Protect A New Admin Page

1. Put the page under `/admin/...` so the Web middleware redirects unauthenticated users.
2. Add the route constant to `AdminRoutes`.
3. Use typed API clients only; do not access EF Core or Identity stores from Blazor.
4. Hide or disable UI commands based on current user permissions if needed, but never rely on UI visibility as the only protection.
5. Ensure the backing API endpoint has the correct permission policy.

## Security Limitations

- Fine-grained ownership rules are not implemented yet because current modules are admin-owned content modules.
- Permission claims are refreshed on login, not continuously.
- The Blazor navigation is not yet dynamically filtered by permission.
- There is no dedicated audit log yet.

## Production Checklist

- Add tests for selected `401` and `403` cases on high-risk endpoints.
- Add audit logging for security-sensitive mutations.
- Add session revocation or security stamp checks.
- Confirm all new `/api/v1` routes have endpoint-level policy metadata.
- Confirm all new admin pages call the API and do not access persistence directly.
