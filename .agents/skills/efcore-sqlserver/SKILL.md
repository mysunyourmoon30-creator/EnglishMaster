---
name: efcore-sqlserver
description: Use for EF Core and SQL Server work in EnglishMaster, including entity modeling, DbContext configuration, migrations, query performance, transactions, data integrity, and persistence-layer review in an ASP.NET Core 9 Clean Architecture project.
---

# EF Core SQL Server

## Project Context

EnglishMaster uses SQL Server with EF Core inside a Clean Architecture, modular monolith application. Treat persistence as infrastructure unless the existing codebase has a specific local convention.

## Workflow

1. Inspect existing DbContext, entity configurations, migrations, connection setup, naming conventions, and repository or unit-of-work patterns before editing.
2. Keep persistence concerns in infrastructure. Do not push `DbContext`, EF attributes, SQL Server details, or migrations into domain code unless the current project already chose that style.
3. Configure entities with the existing approach. Prefer Fluent API configuration when the project already uses it.
4. Model relationships and constraints deliberately: requiredness, delete behavior, indexes, uniqueness, max lengths, precision, and concurrency tokens where relevant.
5. Keep queries async and cancellation-aware when called from request or UI flows.
6. Use projection for read models when only a subset of data is needed. Avoid loading full aggregates for list pages or simple lookups.
7. Use transactions for multi-step writes that must succeed or fail together.
8. Add migrations only when the user asks for schema changes or the task clearly requires them.

## SQL Server Guardrails

- Choose SQL Server-friendly types and sizes intentionally.
- Avoid cascade delete surprises across module boundaries.
- Avoid client-side evaluation and unbounded result sets.
- Avoid raw SQL unless EF Core cannot express the query safely and clearly.
- Parameterize any raw SQL and keep it close to infrastructure code.

## Verification

- Build the project after persistence changes.
- Run focused tests that cover changed queries or writes.
- If a migration is created, inspect the generated operations before trusting it.
- Confirm the migration does not drop or rewrite data unexpectedly.
