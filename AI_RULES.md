# EnglishMaster AI Worker Rules

These rules apply to every automated Codex or Claude Code worker launched for
this repository.

## Ownership

1. One task has one owner, one branch, and one worktree.
2. Work only inside the task's `allowed_paths`.
3. Never change a path listed in `forbidden_paths`.
4. Shared composition files, migrations, project files, authentication,
   configuration, and project-control documents belong to the integrator unless
   the task contract explicitly assigns them.
5. Do not start a second owner on a task until the first owner has written a
   checkpoint and released ownership.

## Scope And Architecture

1. Follow `docs/architecture.md`, `docs/coding-standards.md`, and the existing
   feature conventions.
2. Keep Domain independent of EF Core, ASP.NET Core, Blazor, HTTP, and database
   concerns.
3. Keep use cases and validation in Application, implementations in
   Infrastructure, and endpoints/pages thin.
4. Do not broaden product scope, add packages, or introduce shared abstractions
   without an explicit task requirement.
5. Preserve unrelated user changes. Never reset, discard, or rewrite them.

## Verification And Handoff

1. A worker may edit code, but it may not mark its own work `done`.
2. Run the task's focused checks before reporting completion.
3. A failed build or test is a task failure, not a provider-availability failure.
4. Never auto-merge, auto-push, deploy, or create a release/tag.
5. A checkpoint must record completed work, remaining work, changed files, last
   checks, the current/previous provider, and the next action.
6. Only the integrator may merge one worker branch at a time and set `done`
   after the full test suite passes.

## Provider Failover

1. Fail over only for a recognized rate limit, usage limit, quota depletion, or
   temporary provider outage.
2. Do not fail over for compilation errors, failing tests, invalid task input,
   permission failures, or repository problems.
3. Use only providers explicitly enabled in `automation/ai/providers.json`.
4. Paid API providers require an explicit invocation flag and a configured
   budget. Never discover, install, register, or subscribe to providers.
