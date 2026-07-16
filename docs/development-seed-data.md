# Development Seed Data

## Purpose

EnglishMaster includes small, development-only seed data so the MVP can be tested immediately after database setup. The seed runs during API startup after migrations and after the security seed.

The seed is intentionally small. It is for local development, smoke testing, and demos, not production setup.

## How It Works

Startup calls `SecuritySeeder.SeedSecurityAsync`. That flow:

1. Applies EF Core migrations when the configured database provider is relational.
2. Seeds the built-in permissions.
3. Seeds default roles.
4. Seeds role-permission mappings.
5. Creates the initial SuperAdmin only when safe credentials are supplied through configuration.
6. Seeds demo MVP content only when `DevelopmentSeed:Enabled` is `true`.

The content seed uses the domain factories and EF Core through Infrastructure. It does not bypass domain invariants.

## Configuration

Development content seeding is enabled in `src/Backend/EnglishMaster.Api/appsettings.Development.json`:

```json
{
  "DevelopmentSeed": {
    "Enabled": true
  }
}
```

To disable demo content locally, set:

```json
{
  "DevelopmentSeed": {
    "Enabled": false
  }
}
```

Production configuration should leave `DevelopmentSeed:Enabled` unset or set to `false`.

## Development SuperAdmin

The seed can create one initial SuperAdmin user, but only when both values are supplied through configuration:

- `Auth:InitialSuperAdmin:Email`
- `Auth:InitialSuperAdmin:Password`

Do not commit a real password. Use environment variables, local user secrets, or a developer-only launch profile that is not committed.

PowerShell example:

```powershell
$env:Auth__InitialSuperAdmin__Email = "dev.admin@englishmaster.local"
$env:Auth__InitialSuperAdmin__Password = "replace-with-a-local-development-password"
```

If either value is missing, no SuperAdmin user is created.

## Roles

The security seed creates these roles:

- SuperAdmin
- Admin
- ContentEditor
- Reviewer
- Viewer

## Permissions

All permission constants defined by the application are seeded. Duplicate permission strings are ignored by the seeding process.

## Role Permission Mapping

- SuperAdmin receives all permissions.
- Admin receives broad admin and content permissions, excluding high-risk role and permission management operations.
- ContentEditor receives read, create, and update permissions for content modules, plus publishing read access.
- Reviewer receives read permissions and publish-related review access.
- Viewer receives read-only content permissions.

## Sample Content

The content seed creates:

- Categories: Vocabulary, Grammar, Pronunciation
- Tags: Beginner, Daily English, A1
- Words: hello, book, learn, speak, daily
- Pronunciations for hello, book, and speak
- Grammar Topics: Present Simple, Articles
- Grammar Rules:
  - Present simple for habits
  - A and an with singular nouns
- Lessons:
  - Daily Greetings
  - Using A and An
- Course: A1 Starter English
- Book: EnglishMaster MVP Starter Book
- Quiz: A1 Starter Quiz with questions and choices
- Publishing Templates:
  - Basic HTML Template
  - Basic Markdown Template

The seeded text uses simple English and romanized Thai helper text so the data remains portable in source control.

## Idempotency

The seed checks stable slugs and relationship keys before creating records. Running the API repeatedly should not create duplicate seed records.

## Security Warning

Never use development seed data as a production bootstrap process. Before production deployment:

- Disable `DevelopmentSeed:Enabled`.
- Configure real admin setup through a controlled operational process.
- Do not commit passwords, tokens, or production connection strings.
- Rotate any temporary local credentials that were shared during development.

## Verification

After configuring the database and optional development SuperAdmin, run:

```powershell
dotnet build
dotnet test
```

Then start the API in the Development environment and verify the seeded content appears in the admin pages after login.
