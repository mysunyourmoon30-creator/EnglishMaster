# Codex CLI Setup

This document records how to set up Codex CLI so the EnglishMaster prompt queue runner can call `codex` from PowerShell.

## Verify the Current Environment

Run these commands from a normal PowerShell terminal:

```powershell
where.exe codex
Get-Command codex -ErrorAction SilentlyContinue
node --version
npm --version
```

Then verify the CLI can execute:

```powershell
codex --version
```

The queue runner only needs `codex` to be discoverable and executable from PowerShell. Do not run `scripts/run-codex-queue.ps1` just to verify CLI setup.

## Official Windows Installer

Use the official Windows installer first:

```powershell
powershell -ExecutionPolicy Bypass -Command "irm https://chatgpt.com/codex/install.ps1 | iex"
```

After installation, close and reopen PowerShell so PATH changes are refreshed, then run:

```powershell
where.exe codex
codex --version
```

## npm Fallback

If the official installer fails and npm is available, install Codex CLI with:

```powershell
npm install -g @openai/codex
```

Then close and reopen PowerShell and verify:

```powershell
where.exe codex
codex --version
```

## Troubleshooting PATH

If `codex` is installed but PowerShell cannot find it:

1. Find the installed `codex.exe` location.
2. Add only that executable's directory to the User PATH.
3. Close and reopen PowerShell.
4. Verify with `where.exe codex` and `codex --version`.

Do not add broad or unrelated folders to PATH.

If `where.exe codex` points to `C:\Program Files\WindowsApps\...` and `codex --version` fails with `Access is denied`, the packaged app executable is being blocked by Windows package protections. A working fix is to install or copy the Codex CLI executable into a normal user-level bin directory that is already on PATH, such as the OpenAI Codex user bin directory when present.

If Windows marks the executable as blocked, run:

```powershell
Unblock-File <full path to codex.exe>
codex --version
```

## Queue Runner Verification

To verify that the queue runner can find Codex CLI without starting the queue, run:

```powershell
where.exe codex
codex --version
```

If both commands succeed in the same PowerShell environment that will run the queue, `scripts/run-codex-queue.ps1` should be able to launch Codex CLI.

Do not run the queue, Prompt 58, deployment commands, git tag commands, or any command that may include secrets as part of CLI setup verification.

## Current Machine Notes

On this machine, PowerShell resolved `codex` to the Codex desktop app package under `C:\Program Files\WindowsApps\OpenAI.Codex_26.623.19656.0_x64__2p2nqsd0c76g0\app\resources`, but executing that path returned `Access is denied`.

Copying that executable to a normal project-local test directory allowed it to run and report `codex-cli 0.142.5`, which confirms the executable itself is valid. The remaining setup step is to place a working `codex.exe` in a safe user-level directory that is already on PATH, or install Codex CLI with the official installer once the machine's download/TLS issue is resolved.
