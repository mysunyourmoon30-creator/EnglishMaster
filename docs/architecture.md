# Architecture

EnglishMaster uses Clean Architecture inside a modular monolith. The foundation is designed to keep business logic independent from frameworks while still allowing ASP.NET Core, Blazor, EF Core, and SQL Server to evolve cleanly.

## Layers

### Domain

`EnglishMaster.Domain` is the center of the system. It owns domain concepts and rules. It must not reference any other project or framework-specific package.

### Application

`EnglishMaster.Application` owns use cases, orchestration, interfaces, DTO coordination, and CQRS-ready request handling patterns. It references Domain, Contracts, and Shared.

### Infrastructure

`EnglishMaster.Infrastructure` owns external implementation details such as EF Core, SQL Server, file storage, email, clocks, and other adapters. It references Application, Domain, and Shared.

### Contracts

`EnglishMaster.Contracts` owns request and response contracts shared by API and frontend clients. Keep contracts stable, explicit, and version-friendly.

### Shared

`EnglishMaster.Shared` owns cross-cutting primitives that are safe to share across layers, such as simple result types or constants. Avoid turning Shared into a dumping ground.

### API

`EnglishMaster.Api` is the backend presentation host. It composes Application and Infrastructure and exposes server-side HTTP boundaries.

### Web

`EnglishMaster.Web` is the Blazor frontend. It references only Contracts and Shared to avoid coupling the UI directly to backend internals.

## Dependency Direction

Dependencies point inward toward Domain and Application. Infrastructure and presentation projects depend on abstractions and composition, not the other way around.

## Modular Monolith Guidance

New business areas should be introduced as modules only when the product needs them. Each module should keep its domain, application behavior, persistence configuration, and UI/API surfaces easy to identify.

Current implemented modules are Word, Category, and Tag. Category and Tag organize the Word Module and should remain simple supporting modules until authentication and authorization are added.
