# Content Quality Permissions

## Permissions

| Permission | Purpose |
| --- | --- |
| `content-quality.read` | View dashboard, checks, findings, and rules. |
| `content-quality.run` | Run checks and resolve findings. |
| `content-quality.manage` | Create, update, activate, and deactivate rules. |

## Role Mapping

- SuperAdmin: all content quality permissions.
- Admin: all content quality permissions.
- ContentEditor: read and run.
- Reviewer: read and run.
- Viewer: no content quality access in the current mapping.

## Security Rules

- Blazor pages use typed API clients and do not access EF Core directly.
- API endpoints enforce permission policies.
- Rule management is separate from check execution.
- Findings should not expose secrets or private infrastructure details.
