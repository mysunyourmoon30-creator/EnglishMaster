# Prompt 163 - v0.3.0 Architecture Guardrails

## Goal

Define architecture, permission, persistence, endpoint, UI, and test guardrails for v0.3.0 before implementation prompts begin.

## Completed

- Reviewed the existing Clean Architecture layout.
- Confirmed current module conventions:
  - Domain entities under `EnglishMaster.Domain`.
  - Use cases and abstractions under `EnglishMaster.Application/Features`.
  - EF/provider adapters under `EnglishMaster.Infrastructure`.
  - Minimal APIs under `EnglishMaster.Api/Endpoints`.
  - Blazor API clients under `EnglishMaster.Web/Services`.
- Added `docs/architecture/v0.3.0-architecture-guardrails.md`.

## Key Decisions

- External provider SDKs stay in Infrastructure.
- Domain remains framework-free and persistence-free.
- Application owns abstractions and use-case orchestration.
- API endpoints delegate to Application handlers and enforce auth/permissions.
- Web does not access `DbContext` or Infrastructure services directly.
- Public certificate verification must expose only public-safe DTOs.
- Analytics must be bounded, aggregate-oriented, and privacy-safe.

## Next Prompt

Prompt 164 - External Email Provider Design.
