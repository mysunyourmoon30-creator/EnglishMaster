# Worker Prompts

Store one bounded prompt per task in this directory. The filename must match the
`promptFile` in `automation/ai/tasks.json`.

Each prompt should contain the objective, acceptance criteria, non-goals, and
any feature-specific verification. The orchestrator prepends the repository
rules and the JSON task contract automatically.
