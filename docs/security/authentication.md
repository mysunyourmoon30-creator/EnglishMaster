# Authentication

## Purpose

EnglishMaster uses cookie-based authentication for the admin surface. The API issues the admin cookie after a successful login, and the Blazor admin app keeps its own web cookie plus a forwarded API cookie so admin pages can call protected backend endpoints through typed API clients.

Authentication is for the existing admin and content-management experience only. It does not yet model public learner accounts.

## Approach

- API cookie scheme: `EnglishMaster.Admin`
- API cookie name: `.EnglishMaster.Admin`
- Web cookie scheme: `EnglishMaster.Web`
- Web cookie name: `.EnglishMaster.Web`
- Cookie lifetime: 8 hours with sliding expiration
- Password storage: hashed through `PasswordHasher<AppUser>` in Infrastructure
- Authentication state: claims include user id, name, email, roles, and permission keys
- Initial seeding: default roles, permissions, and optional initial SuperAdmin are seeded outside the Testing environment

The Domain layer owns the security entities. The Application layer exposes security use cases through abstractions. The Infrastructure layer owns password hashing, EF Core persistence, and seeding. API endpoints stay thin and call Application handlers.

## Login Flow

1. The admin user opens `/login`.
2. Blazor posts the email and password to the Web `/login` endpoint.
3. The Web app calls `POST /api/v1/auth/login`.
4. The API validates credentials through the Application layer and Infrastructure security service.
5. The API signs in with the `EnglishMaster.Admin` cookie and returns the current user DTO.
6. The Web app signs in with the `EnglishMaster.Web` cookie and stores the API cookie in a claim used by the API HTTP client handler.
7. The user is redirected to `/admin` or the requested return URL.

Invalid credentials return safe validation errors. Password hashes are never returned by API contracts.

## Logout Flow

1. The user posts to Web `/logout`.
2. The Web app calls `POST /api/v1/auth/logout` using the stored API cookie.
3. The API clears the `EnglishMaster.Admin` cookie.
4. The Web app clears the `EnglishMaster.Web` cookie.
5. The user is redirected to `/login`.

## Current User Flow

`GET /api/v1/auth/me` reads the authenticated user id claim and reloads the user through the Application layer. If the user no longer exists or is inactive, the endpoint does not return a current user.

The current user response contains:

- `id`
- `email`
- `userName`
- `displayName`
- `roles`
- `permissions`

It does not contain password hashes, normalized identity fields, or internal EF Core navigation properties.

## Cookie Security

Development and Testing allow `SameAsRequest` secure-cookie behavior so local HTTP-based test hosts can work. Non-development environments require secure cookies.

Production deployments should run behind HTTPS and should keep:

- `HttpOnly = true`
- `SameSite = Lax` unless cross-site admin hosting requires a deliberate change
- `SecurePolicy = Always`
- short enough expiration for admin sessions

## Security Limitations

- Role and permission claims are created at login time. A user may need to log out and log back in to pick up changed permissions.
- There is no full security-stamp/session-revocation mechanism yet.
- There is no rate limiting or account lockout yet.
- There is no multi-factor authentication yet.
- Public learner authentication is not part of this module.

## Production Checklist

- Configure `Auth:InitialSuperAdmin:Email` and `Auth:InitialSuperAdmin:Password` through secrets, not committed settings.
- Rotate the initial SuperAdmin password after first deployment.
- Enforce HTTPS end to end.
- Add rate limiting to login endpoints.
- Add account lockout or throttling for repeated failed login attempts.
- Add structured audit logging for login, logout, user changes, role changes, and permission changes.
- Add a security-stamp or session-version check for immediate permission revocation.
- Review cookie domain and SameSite settings for the final hosting topology.

## Next Recommended Module

Student Progress is the next recommended module after admin security, because the platform now has enough protected content-management structure to track learner activity safely.
