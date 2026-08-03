# AI Failover Orchestrator

## Purpose

This is development automation for EnglishMaster. It coordinates bounded coding
tasks across installed Codex and Claude Code CLIs. It is not an AI Tutor feature
and it does not add model inference to the EnglishMaster product.

The first version deliberately keeps the integrator human-controlled: workers
may edit and test a dedicated feature branch, but the orchestrator never commits,
merges, pushes, deploys, creates a tag, or marks a task `done`.

## Files

| File | Owner | Purpose |
| --- | --- | --- |
| `AI_RULES.md` | Integrator | Rules prepended to every worker prompt |
| `automation/ai/providers.json` | Integrator | Explicit provider allowlist and priority |
| `automation/ai/tasks.json` | Integrator | Central task contracts; initially empty |
| `automation/ai/tasks.example.json` | Integrator | Diagnostic Test task example |
| `automation/ai/prompts/` | Task planner | One bounded prompt per task |
| `.ai-orchestrator/` | Runtime only | Locks, provider state, checkpoints, attempt logs |

Runtime state is ignored by Git. All worktrees that should coordinate must point
`-RuntimeDirectory` to the same absolute directory. Launching from one integration
worktree with the default `.ai-orchestrator` directory is the simplest setup.

## Required Task Contract

Each task requires:

- unique `id`, feature, initial status, branch, and existing dedicated worktree;
- dependencies whose task IDs exist and do not form a cycle;
- allowed and forbidden repository-relative paths;
- a prompt file with objective, acceptance criteria, and non-goals;
- explicit build/test commands represented as command plus argument arrays.

The orchestrator rejects duplicate IDs, missing dependencies, cycles, unsupported
status/version values, path traversal, a missing/mismatched worktree, and an
initially dirty worktree for a new queued task.

## Provider Rules

Only enabled entries in `providers.json` are considered. The script checks that
the configured executable exists and its version command succeeds; it never
installs a CLI, starts login, creates an account, or discovers new providers.
Codex may use the project-local `.tools/codex/codex.exe` fallback when the
WindowsApps executable is present but blocked by package permissions.

Paid providers are disabled by default. To use one, an integrator must configure
its command, enable it, set a non-zero budget, and pass `-AllowPaidProviders`.
Budget enforcement for provider-specific API wrappers remains required before
enabling either example paid provider.

Failover occurs only when a provider exits unsuccessfully and its output contains
a configured rate/quota signal. Authentication failures, timeouts, code errors,
repository errors, forbidden-path edits, and failing checks block the task; they
do not trigger provider switching.

## Isolation And Concurrency

The runtime uses atomic lock files for coordination, each task, and each provider.
One provider may own only one task at a time. Multiple script processes may run
independent tasks concurrently when their dependencies are complete and their
normalized allowed paths do not overlap.

Locks are intentionally fail-closed. A crashed process can leave a lock behind.
Before removing it manually, verify the recorded PID/machine is no longer active,
inspect the immutable attempt log, Git diff, runtime state, and checkpoint, then
record the ownership transfer. The script never deletes a lock it did not acquire.
On resume, the task HEAD must still match the checkpoint and path validation uses
the original baseline HEAD, so committed partial work cannot evade the contract.

## Status Flow

```text
queued -> in_progress -> testing -> ready_to_merge
                 |
                 +-> checkpointed (recognized provider limit only)
                 |
                 +-> blocked (task, timeout, path, build, or test failure)

ready_to_merge -> done (integrator only, after sequential merge and full tests)
```

Provider output is written once per task/attempt/provider under the runtime log
directory. Common credential forms are redacted, but prompts and output must not
contain secrets in the first place.

Allowed-path validation covers both the uncommitted worktree status and commits
created after the recorded baseline HEAD. Paths traversing a Windows reparse
point are blocked for integrator review.

## Setup

Create branches and worktrees explicitly. The orchestrator will validate them but
will not create or switch them:

```powershell
git worktree add ../EnglishMaster-codex-diagnostic -b codex/v1-f04-diagnostic-test integration
git worktree add ../EnglishMaster-claude-vocabulary -b codex/v1-f05-vocabulary integration
```

Copy the relevant task from `tasks.example.json` into `tasks.json`, update its
contract, and create the referenced prompt. Shared files such as `Program.cs`,
DbContext, migrations, project files, authentication, appsettings, and the main
project-control document should remain forbidden unless the integrator owns that
specific task.

## Validate And Preview

Validation does not invoke a provider:

```powershell
./scripts/Invoke-AiFailoverOrchestrator.ps1 -ValidateOnly
```

Dry-run selects and displays a task/provider plan without creating runtime files,
probing providers, running builds, or running tests:

```powershell
./scripts/Invoke-AiFailoverOrchestrator.ps1 -TaskId V1-F04 -DryRun
```

Run the committed validation suite:

```powershell
./scripts/Test-AiFailoverOrchestrator.ps1
```

## Execute

After checking the task contract and worktree:

```powershell
./scripts/Invoke-AiFailoverOrchestrator.ps1 -TaskId V1-F04
```

To run two independent tasks, start two PowerShell processes with the same task
queue and runtime directory but different task IDs. Provider locks ensure Codex
and Claude are not each assigned twice, and allowed-path overlap blocks unsafe
concurrency.

## Integrator Gate

For each `ready_to_merge` task:

1. Read its checkpoint and immutable attempt log.
2. Review its Git diff against the task contract.
3. Commit only that task if the worker did not already produce a reviewed commit.
4. Merge one branch into `integration`.
5. Run the relevant build and tests immediately.
6. Rebase the next worker branch on the tested integration revision.
7. Merge and run the full suite again.
8. Only then update the central project control to `done` and merge toward main.

Do not auto-resolve conflicts and do not auto-merge into `main`.
