# Content Revision Security

## Permission Model

| Permission | Purpose |
| --- | --- |
| `content-revisions.read` | Read revision timelines and restore requests. |
| `content-revisions.restore.request` | Request a restore from an existing revision. |
| `content-revisions.restore.approve` | Approve or reject restore requests. |
| `content-revisions.manage` | Create revisions and complete approved restore requests. |

## Role Mapping

- SuperAdmin: all revision permissions.
- Admin: all revision permissions.
- Reviewer: read and approve restore requests.
- ContentEditor: read and request restore.
- Viewer: no restore permissions.

## Snapshot Safety

Snapshots must not expose authentication or infrastructure secrets. The sanitizer removes field names containing:

- password
- token
- secret
- API key
- connection string
- cookie
- security stamp

Do not serialize user credentials, session data, raw storage keys, private file paths, or infrastructure configuration into revision snapshots.

## Endpoint Protection

All revision and restore endpoints require authentication and permission policies. Blazor route protection is not the security boundary; API authorization remains required for every protected action.

## Testing Note

The test environment relaxes some non-security endpoint authentication behavior. Permission definitions, role mappings, and workflow rules are covered by automated tests; full 401 and 403 behavior should also be verified in a non-Testing environment.
