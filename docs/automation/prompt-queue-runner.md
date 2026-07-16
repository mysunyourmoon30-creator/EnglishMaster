# Prompt Queue Runner

The prompt queue runner is infrastructure for running EnglishMaster prompts in order from Prompt 058 through Prompt 161. It does not run automatically.

## Folder Structure

```text
EnglishMaster/
├── prompts/
│   └── queue/
├── prompt-queue.json
├── scripts/
│   └── run-codex-queue.ps1
└── logs/
    └── codex-queue/
```

## Queue Status Values

- `pending`: Ready to run after its dependency is completed and its prompt content is filled in.
- `running`: The runner is currently executing the prompt.
- `completed`: The prompt finished and restore/build/test checks passed.
- `failed`: The prompt, build, or test step failed.
- `waiting_limit`: A usage limit, rate limit, depletion message, or temporary availability issue was detected.
- `skipped`: Reserved for prompts intentionally skipped by a human.

## Add Prompt Content

Each prompt file in `prompts/queue/` starts with front matter and a placeholder:

```text
PASTE FULL PROMPT CONTENT HERE
```

Replace only that placeholder with the full prompt text before running the queue. The runner stops before invoking Codex if the placeholder is still present.

## Dry Run

Use dry run mode to check which prompt would run without invoking Codex:

```powershell
.\scripts\run-codex-queue.ps1 -DryRun
```

To check a smaller range:

```powershell
.\scripts\run-codex-queue.ps1 -StartFrom 58 -EndAt 61 -DryRun
```

## Run Prompt 058 To 161

After Prompt 058 through Prompt 161 contain real prompt content, run:

```powershell
.\scripts\run-codex-queue.ps1 -StartFrom 58 -EndAt 161 -RetryOnLimit
```

The runner processes pending prompts in numeric order. Prompt 058 has no dependency. Every following prompt depends on the previous prompt being `completed`.

## Resume After Failure

If a prompt fails, fix the blocker first. Leave completed prompts as `completed`, leave the failed prompt as `failed` until reviewed, then manually set it back to `pending` in `prompt-queue.json` when it is safe to retry.

Run the queue again from the failed prompt:

```powershell
.\scripts\run-codex-queue.ps1 -StartFrom 58 -EndAt 161 -RetryOnLimit
```

The runner will skip completed prompts and pick the first pending item in the requested range.

## Usage Limit Handling

The runner detects these usage-limit signals:

- `usage limit`
- `rate limit`
- `You've hit your usage limit`
- `depleted`
- `temporarily unavailable`

With `-RetryOnLimit`, the runner marks the prompt as `waiting_limit`, waits 60 minutes, and retries the same prompt until `-MaxRetries` is exceeded.

Without `-RetryOnLimit`, it marks the prompt as `waiting_limit` and stops.

## Safety Rules

The runner must never:

- Deploy automatically.
- Push tags automatically.
- Merge pull requests automatically.
- Push to `main` automatically.
- Hide blockers or continue after failed build/test checks.

After a successful prompt run, the runner runs:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Use `-SkipBuild` or `-SkipTest` only when a human has decided those checks should be deferred.
