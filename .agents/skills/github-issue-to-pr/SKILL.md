---
name: github-issue-to-pr
description: Use when turning a GitHub issue for EnglishMaster into an implementation plan, branch, code change, verification pass, commit, and pull request, while respecting the ASP.NET Core 9, Blazor, EF Core, SQL Server, Clean Architecture, modular monolith context.
---

# GitHub Issue To PR

## Project Context

EnglishMaster is an ASP.NET Core 9, Blazor, EF Core, SQL Server application built with Clean Architecture and modular monolith boundaries. Treat each issue as a scoped product change unless the issue explicitly asks for broad refactoring.

## Workflow

1. Read the issue title, body, acceptance criteria, linked comments, labels, and related files. Use the GitHub CLI or available GitHub tools when configured; otherwise ask for the missing issue details.
2. Restate the goal in implementation terms: behavior, affected module, UI or API surface, persistence impact, and verification plan.
3. Check the working tree before editing. Preserve unrelated user changes.
4. Create or use a focused branch name based on the issue number and intent when branch work is requested.
5. Implement the smallest complete change that satisfies the issue. Follow the relevant repo skill for architecture, Blazor, EF Core, tests, or security when applicable.
6. Run focused verification first, then a broader build or test command when feasible.
7. Prepare a concise commit message and PR summary that connect the code change to the issue.

## PR Description Shape

Use this structure unless the repository already has a template:

```markdown
## Summary
- ...

## Verification
- ...

Closes #ISSUE_NUMBER
```

## Guardrails

- Do not implement extra features beyond the issue without calling them out.
- Do not create migrations, modules, or public API changes unless the issue requires them.
- Do not create Word Module work unless the issue explicitly asks for it.
- Do not force-push, rebase shared work, or overwrite user changes without explicit permission.
- If GitHub access is unavailable, complete the local work and provide the PR title, body, and verification notes for the user.
