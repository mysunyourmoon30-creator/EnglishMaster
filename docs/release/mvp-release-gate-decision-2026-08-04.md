# MVP Release Gate Decision — 2026-08-04

## Decision

The release owner explicitly accepts the combined native no-Docker local UAT
and disposable CI release evidence as the MVP release gate.

This decision completes the gate that previously blocked V1-F06 and later
product planning. It does not claim that a persistent staging environment was
deployed or tested.

## Accepted Evidence

- Native .NET local browser UAT passed on 2026-08-04, including public grammar,
  authenticated administration routes, a representative Category workflow,
  media validation, import row errors, and logout.
- The authenticated release smoke suite passed all 12 checks locally.
- Disposable CI release staging built the release containers, applied the
  reviewed migration bundle to a fresh SQL Server database, and passed the
  automated release smoke gate.
- The repository Build workflow passed for the recorded UAT revision.

## Accepted Boundary

- No paid hosting or Docker Desktop is required for this MVP gate.
- No public staging URL, persistent host, deployment credential, GitHub
  Environment, or persistent deployment workflow is configured.
- The native local UAT used the in-memory development database. Database and
  migration confidence comes from the separate disposable CI release run.
- Persistent staging remains an operations follow-up and must be completed
  before making any claim that the application has been validated on a
  long-lived Internet-accessible environment.

## Next Ordered Work

The release gate is complete. The product roadmap identifies Student Progress
as the next business module, but `automation/ai/tasks.json` currently ends at
V1-F05 and contains no approved V1-F06 task contract. Define and approve that
contract before implementation so its scope, dependencies, allowed paths, and
verification checks are explicit.

