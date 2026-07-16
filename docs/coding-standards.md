# Coding Standards

## General

- Target .NET 9.
- Keep nullable reference types enabled.
- Prefer explicit, intention-revealing names.
- Keep methods small enough to read without scrolling through unrelated behavior.
- Avoid speculative abstractions.

## Clean Architecture

- Keep Domain independent from frameworks and persistence.
- Keep Application focused on use cases and orchestration.
- Keep Infrastructure behind Application-defined abstractions.
- Keep API and Web as presentation layers.
- Do not let Blazor components call EF Core directly.

## C#

- Prefer async APIs for I/O.
- Pass `CancellationToken` through application and infrastructure operations when practical.
- Use records for immutable contracts and simple value carriers.
- Use sealed classes when inheritance is not intended.
- Keep exceptions for exceptional cases; use explicit results for expected business outcomes when that pattern is introduced.

## Security

- Validate all external input.
- Do not log secrets or personal data.
- Keep authorization checks server-side.
- Prefer secure framework defaults over custom security code.

## Testing

- Unit tests focus on Domain and Application behavior.
- Integration tests cover infrastructure and host wiring.
- Architecture tests protect dependency direction and layer boundaries.
