# Practice Pages

## Routes

```text
/learn/practice
/learn/practice/session/{id:guid}
/learn/practice/history
```

These pages are learner-facing and expect the user to be authenticated.

## Practice Dashboard

The dashboard shows:

- Due today count.
- New practice item count.
- Reviewing count.
- Mastered count.
- Start practice action.
- Generate practice items action.
- Link to practice history.

The page includes loading, empty, and error states.

## Practice Session Page

The session page shows practice prompts and lets the learner submit a self-rated result:

```text
Again
Hard
Good
Easy
```

After submission, the page shows the item result and schedule state returned by the API.

## Practice History Page

The history page lists previous sessions with score and completion information.

## Student Dashboard Integration

The student dashboard links to `/learn/practice` and includes a practice entry point alongside continue-learning and recommendation content.

## Known Limitations

- The first UI is intentionally simple.
- It does not include audio pronunciation recording.
- It does not include AI-generated practice prompts.
- It does not include advanced per-skill analytics.
