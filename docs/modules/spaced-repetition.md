# Spaced Repetition

## Purpose

Spaced repetition schedules practice items so learners see difficult content sooner and successful content later.

EnglishMaster v0.2.0 uses a simple deterministic schedule so the behavior is easy to test, explain, and adjust.

## Scheduling Rules

Initial result scheduling:

```text
Again -> 1 day
Hard  -> 2 days
Good  -> 4 days
Easy  -> 7 days
```

Repeated correct answers gradually increase the interval through a bounded progression:

```text
1, 2, 4, 7, 14, 30 days
```

The implementation does not use SM-2 or a machine learning model.

## Status Behavior

Practice items can have these statuses:

```text
New
Due
Learning
Reviewing
Mastered
Suspended
```

After a result is applied, the item records the review time, updates counters, sets `NextReviewAt`, and moves to `Reviewing` unless it has reached the mastered rule.

`Mastered` is applied only after repeated correct work and a long interval.

## Safety Rules

- Counters are never decremented by scheduling.
- Submitted session items cannot be submitted again.
- Suspended items are excluded from due practice.
- Completed sessions cannot be completed again.

## Performance Notes

Due practice is limited and ordered by review time. Practice summary uses database count queries rather than loading all practice items into memory.

## Known Limitations

- The schedule is intentionally simple.
- There is no per-skill weighting yet.
- There is no adaptive difficulty model yet.
