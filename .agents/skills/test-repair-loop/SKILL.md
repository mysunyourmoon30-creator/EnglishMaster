---
name: test-repair-loop
description: Use when EnglishMaster tests, builds, or CI checks fail and Codex needs to reproduce the failure, diagnose the root cause, make the smallest correct fix, and rerun the relevant .NET test or build commands until clean.
---

# Test Repair Loop

## Project Context

EnglishMaster is an ASP.NET Core 9, Blazor, EF Core, SQL Server, Clean Architecture project. Test repairs should preserve architecture boundaries and avoid hiding real product defects.

## Loop

1. Reproduce the failure with the smallest relevant command first.
2. Read the first meaningful error, stack trace, assertion diff, or build diagnostic. Ignore follow-on noise until the root failure is understood.
3. Locate the code and test involved. Compare expected behavior, current behavior, and recent local changes.
4. Fix the cause, not the symptom. Prefer production code fixes when the test exposes a real bug; update tests only when the expected behavior is obsolete or incorrect.
5. Rerun the same focused command.
6. Repeat until that command is clean.
7. Broaden to the next relevant test/build command before finishing.

## Repair Rules

- Do not delete or weaken assertions just to make tests pass.
- Do not skip tests unless the user explicitly requests it or the test is permanently invalid and the reason is documented in the change.
- Do not change public behavior silently to match a broken test.
- Do not introduce broad refactors while repairing a narrow failure.
- Preserve user changes in the working tree.

## Diagnosis Checklist

- Build errors before runtime errors.
- Dependency injection and configuration errors before business logic errors.
- Database provider, migration, and test data setup before query assertions.
- Async timing, disposal, and cancellation issues before adding waits.
- Culture, time zone, and clock assumptions before changing date logic.

## Finish Criteria

- The original failing command passes.
- A broader relevant command passes or the remaining failure is clearly unrelated.
- The final summary names what failed, what changed, and what was verified.
