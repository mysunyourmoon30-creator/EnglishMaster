# Roles And Permissions

## Purpose

Roles group permission keys into manageable admin profiles. Permissions are the actual authorization units used by API policies.

## Role Model

Roles are represented by `AppRole` in the Domain layer and returned through `RoleDto` in API contracts.

Role fields:

- `Id`
- `Name`
- `Slug`
- `Description`
- `IsSystem`
- `IsActive`
- `Permissions`
- `CreatedAt`
- `UpdatedAt`

System roles are seeded by the platform and cannot be deleted. System roles cannot be deactivated through the current role update rules.

## Permission Model

Permissions are centralized as string constants in `Permissions`. They are seeded into `AppPermission` rows and attached to roles through `AppRolePermission`.

Permission fields:

- `Key`
- `Name`
- `Module`
- `Description`

Permission strings use `{module}.{action}` format.

## Default Roles

| Role | Purpose |
| --- | --- |
| `SuperAdmin` | Full platform administration. Has every permission. |
| `Admin` | Administer most platform areas, with limited user/role/permission power. |
| `ContentEditor` | Create and update content modules. |
| `Reviewer` | Read content and run publishing jobs. |
| `Viewer` | Read-only access to content and publishing records. |

## Permission List

| Module | Permissions |
| --- | --- |
| Words | `words.read`, `words.create`, `words.update`, `words.delete` |
| Categories | `categories.read`, `categories.create`, `categories.update`, `categories.delete` |
| Tags | `tags.read`, `tags.create`, `tags.update`, `tags.delete` |
| Media | `media.read`, `media.create`, `media.update`, `media.delete` |
| Pronunciation | `pronunciation.read`, `pronunciation.create`, `pronunciation.update`, `pronunciation.delete` |
| Grammar | `grammar.read`, `grammar.create`, `grammar.update`, `grammar.delete` |
| Lessons | `lessons.read`, `lessons.create`, `lessons.update`, `lessons.delete` |
| Courses | `courses.read`, `courses.create`, `courses.update`, `courses.delete` |
| Books | `books.read`, `books.create`, `books.update`, `books.delete` |
| Quizzes | `quizzes.read`, `quizzes.create`, `quizzes.update`, `quizzes.delete` |
| Publishing | `publishing.read`, `publishing.create`, `publishing.update`, `publishing.delete`, `publishing.run` |
| Users | `users.read`, `users.create`, `users.update`, `users.delete` |
| Roles | `roles.read`, `roles.create`, `roles.update`, `roles.delete` |
| Permissions | `permissions.read`, `permissions.update` |

## Role-Permission Mapping

| Role | Mapping |
| --- | --- |
| `SuperAdmin` | All permissions. |
| `Admin` | Most permissions except role administration, user create/delete, and permission update. Includes user read/update. |
| `ContentEditor` | Content read plus content create/update. |
| `Reviewer` | Content read plus `publishing.read` and `publishing.run`. |
| `Viewer` | Content read plus `publishing.read`. |

Content read currently includes Words, Categories, Tags, Media, Pronunciation, Grammar, Lessons, Courses, Books, and Quizzes.

Content create/update currently includes those same content modules, but not delete permissions.

## How To Add A New Permission

1. Add a constant to `Permissions`.
2. Add a `P(...)` entry to `Permissions.All`.
3. Add the permission to `ContentRead`, `ContentCreateUpdate`, or `ContentDelete` if it belongs to normal content workflows.
4. Update default role mapping in the security service seeding logic.
5. Add route-level `.RequireAuthorization(...)` to the API endpoint.
6. Add documentation in this file and in the relevant API/module docs.
7. Run `dotnet build` and `dotnet test`.

## Duplicate Permission Rule

Do not create a new permission string when an existing module/action permission already describes the operation. Use `update` for state changes such as activate, deactivate, publish, unpublish, reorder, and relationship add/remove unless the operation needs a distinct risk boundary, such as `publishing.run`.

## Security Limitations

- Permission assignments are claim-based after login and may not reflect changes until the next login.
- Role-permission changes are not audited yet.
- There is no tenant or ownership model yet.
- There are no public learner roles yet.

## Production Checklist

- Review default SuperAdmin credentials and rotate immediately.
- Audit every role-permission assignment before go-live.
- Add tests for critical forbidden cases.
- Add audit logging for role and permission changes.
- Add session revocation for permission changes.
