# Study Plan Generation

## Purpose

Study plan generation creates a safe, bounded daily plan using existing EnglishMaster learning data.

It is deterministic and does not call AI or external services.

## Priority Order

Generation uses this priority:

```text
1. PracticeDueItems
2. ContinueLesson
3. RecommendedLesson
4. ReviewWeakQuiz
```

The intended broader order remains:

```text
Due practice
Continue learning
Next lesson in active course
Weak quiz review
Recommended lessons, words, grammar, quizzes
```

## Safety Rules

- Avoid duplicate content inside the same plan.
- Use active/published learner-facing content only.
- Exclude inactive content.
- Do not expose quiz correct answers.
- Return a safe empty plan when no content exists.

## Performance Rules

- History queries are bounded.
- Plan generation uses small targeted queries.
- Existing plans are reused instead of creating duplicate same-day rows.

## Why AI Is Not Included

AI study plan generation is intentionally deferred until content safety, explainability, review controls, and cost limits are defined.

## Why Calendar Integration Is Not Included

External calendar integration is intentionally deferred because it requires OAuth scope review, data deletion behavior, retry handling, and user consent flows.
