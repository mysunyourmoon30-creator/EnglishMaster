---
name: clean-architecture-dotnet
description: Use for designing, implementing, or reviewing .NET architecture changes in EnglishMaster, especially ASP.NET Core 9 Clean Architecture work, modular monolith boundaries, dependency direction, application services, domain rules, and infrastructure composition.
---

# Clean Architecture .NET

## Project Context

EnglishMaster is an ASP.NET Core 9 application using Blazor, SQL Server, EF Core, Clean Architecture, and a modular monolith style. Keep changes aligned with the existing solution layout and naming before introducing new structure.

## Workflow

1. Inspect the current project structure, `Program.cs`, dependency registration, folders, namespaces, and existing conventions before changing anything.
2. Identify the feature or module boundary first. Keep module ownership explicit and avoid cross-module shortcuts.
3. Keep dependencies pointing inward:
   - Domain holds business concepts and rules. Do not reference EF Core, ASP.NET Core, Blazor, HTTP, or database concerns from domain code.
   - Application holds use cases, interfaces, DTOs, validation, orchestration, and transaction intent.
   - Infrastructure implements persistence, integrations, identity adapters, and framework-specific services.
   - Presentation composes UI, endpoints, routing, request handling, and user-facing state.
4. Put shared behavior in shared abstractions only when at least two real callers need it. Prefer clear feature-local code over premature generalization.
5. Register services at the composition root or the existing module registration point. Avoid service locator patterns.
6. Keep public contracts stable and narrow. Do not expose EF entities directly to UI or API surfaces unless the project already intentionally does so.
7. Make architectural decisions visible in names, folders, and dependency direction rather than comments.

## Guardrails

- Do not create a new module unless the user explicitly requests that module or the existing architecture clearly requires one.
- Do not create Word Module work unless the user explicitly asks for it in the current task.
- Do not move unrelated files as part of an architecture cleanup.
- Do not add new libraries when the existing stack already supports the need.
- Prefer small, verifiable changes with focused tests when behavior changes.

## Review Checklist

- The changed code follows existing namespace and folder conventions.
- Domain code stays persistence- and framework-free.
- Application code depends on abstractions, not concrete infrastructure.
- Infrastructure code does not leak into UI or domain contracts.
- Module boundaries remain clear and enforceable.
- Tests cover the business behavior or architecture-sensitive path touched by the change.
