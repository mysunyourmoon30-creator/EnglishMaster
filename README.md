# EnglishMaster

EnglishMaster is an English learning platform built with ASP.NET Core 9, Blazor, SQL Server, and EF Core.

The current v0.2.0 release candidate includes admin content management, security roles and permissions, publishing, reporting, notifications, content quality, content revision history, bulk operations, import validation, public search, learner recommendations, practice, goals, study plans, motivation, achievements, and weekly learning reports.

AI tutor features, mobile apps, payments, marketplace behavior, plugins, and microservices remain intentionally out of scope.

## Solution Layout

```text
src/
  Backend/
    EnglishMaster.Api/
    EnglishMaster.Application/
    EnglishMaster.Contracts/
    EnglishMaster.Domain/
    EnglishMaster.Infrastructure/
    EnglishMaster.Shared/
  Frontend/
    EnglishMaster.Web/
tests/
  EnglishMaster.ArchitectureTests/
  EnglishMaster.IntegrationTests/
  EnglishMaster.UnitTests/
```

## Build

```powershell
dotnet restore EnglishMaster.sln
dotnet build EnglishMaster.sln --configuration Release --no-restore
dotnet test EnglishMaster.sln --configuration Release --no-build
```

## v0.2.0 Release Docs

- [Release Notes](docs/release/v0.2.0-release-notes.md)
- [Release Candidate](docs/release/v0.2.0-release-candidate.md)
- [Known Limitations](docs/release/v0.2.0-known-limitations.md)
- [Smoke Test](docs/testing/v0.2.0-smoke-test.md)
- [UAT Plan](docs/testing/v0.2.0-uat-plan.md)
- [Go-Live Checklist](docs/release/v0.2.0-go-live-checklist.md)
- [Rollback Plan](docs/release/v0.2.0-rollback-plan.md)

## Architecture Rules

- `EnglishMaster.Domain` has no project references.
- `EnglishMaster.Application` references Domain, Contracts, and Shared.
- `EnglishMaster.Infrastructure` references Application, Domain, and Shared.
- `EnglishMaster.Api` references Application, Infrastructure, Contracts, and Shared.
- `EnglishMaster.Web` references Contracts and Shared.
- Test projects reference only the projects needed for their test scope.
