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
7. When the full development seed is disabled, seeds only the grammar curriculum when `SeedGrammarCurriculum:Enabled` is `true`.

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

To install only the standard grammar curriculum without the rest of the demo data, use:

```json
{
  "DevelopmentSeed": {
    "Enabled": false
  },
  "SeedGrammarCurriculum": {
    "Enabled": true
  }
}
```

`SeedGrammarCurriculum:Enabled` is a one-time operational switch. It activates and refreshes the managed curriculum during startup, so set it back to `false` after a successful seed.

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
- Grammar curriculum: 13 topics, 13 rules, and 39 active examples from A1 through B1
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

The grammar curriculum uses deterministic IDs for new topics, rules, and example slots, while retaining slug and sort-order fallbacks for databases seeded by older versions. Existing rule-word links are synchronized to the configured seed set when their vocabulary exists. The curriculum write is transactional on relational databases, and running it repeatedly should not create duplicate seed records.

## Security Warning

Never use development seed data as a production bootstrap process. Before production deployment:

- Disable `DevelopmentSeed:Enabled`.
- Disable `SeedGrammarCurriculum:Enabled` after any intentional one-time curriculum seed.
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
