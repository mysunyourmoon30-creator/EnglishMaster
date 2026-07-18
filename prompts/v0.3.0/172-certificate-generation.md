# Prompt 172 - Certificate Generation

Status: Completed

## Scope

- Added issued certificate domain model and persistence configuration.
- Added certificate generation application flow for completed courses.
- Added learner API endpoints for generating and listing issued certificates.
- Added integration coverage for successful issuance, idempotency, incomplete course rejection, and current-user scoping.

## Verification

- `dotnet build`
- `dotnet test tests\EnglishMaster.IntegrationTests\EnglishMaster.IntegrationTests.csproj --filter "FullyQualifiedName~Certificates"`
