# Architecture Overview

EnglishMaster follows the architecture described in [Architecture](../architecture.md).

The v0.2.0 release candidate keeps the same boundaries:

- Domain contains business entities and rules.
- Application contains use cases, command handlers, query handlers, DTOs, and repository abstractions.
- Infrastructure contains EF Core persistence and external adapters.
- API exposes HTTP endpoints and delegates use cases to application handlers.
- Web contains Blazor pages and API clients, not direct DbContext access.
