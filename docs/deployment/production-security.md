# Production Security

## HTTPS

Production must run behind HTTPS. The apps enforce secure cookie policy outside development/testing. Terminate TLS at the hosting platform or reverse proxy and forward traffic to the app securely.

## Cookies

Admin cookies are HTTP-only. SameSite is `Lax`. In production, secure cookie policy is `Always`.

## CORS

The API currently does not enable a broad CORS policy. Keep API access same-origin or behind trusted infrastructure unless a specific browser-based cross-origin client is introduced. If CORS is added later, use an explicit allow-list.

## Authentication And Authorization

- `/api/v1/auth/login` is the login endpoint.
- Non-login `/api/v1/*` routes require authentication outside Testing.
- Endpoint policies enforce module permissions.
- Admin Blazor routes redirect unauthenticated users to login, but API authorization remains the real boundary.

## Health And Diagnostics

Health endpoints should be used by load balancers and monitors. Avoid exposing detailed exception output or infrastructure identifiers publicly.

## Secrets

Do not commit connection strings, bootstrap credentials, provider credentials, tokens, certificates, or production passwords. Use environment variables or deployment secret storage.
