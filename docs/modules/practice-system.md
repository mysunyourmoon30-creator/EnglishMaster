# Practice System

## Purpose

The practice system gives authenticated learners a simple way to review words, grammar rules, minimal pairs, and weak quiz areas.

It is intentionally rule based. It does not use AI, machine learning, an external spaced repetition service, payments, marketplace behavior, or mobile-only flows.

## Scope

Practice supports:

- Word flashcards.
- Word meaning review.
- Grammar review.
- Minimal pair practice.
- Quiz review for weak or failed quiz attempts.

Practice is learner-owned. Every practice item and practice session belongs to one student profile.

## Practice Types

```text
WordFlashcard
WordMeaning
WordPronunciation
GrammarReview
MinimalPairPractice
QuizReview
```

The current implementation creates word flashcard, word meaning, grammar review, minimal pair practice, and quiz review items.

## Practice Results

```text
Again
Hard
Good
Easy
```

Invalid result values are rejected by the application layer and returned as a bad request by the API.

## Generation Rules

Practice generation:

- Creates items for active words.
- Creates items for active grammar rules.
- Creates items for active minimal pairs.
- Creates quiz review items from failed or weak quiz attempts when the quiz is active and published.
- Prevents duplicates for the same student, content type, content id, and practice type.
- Does not include inactive content.
- Does not include quiz answer keys.

## Due Behavior

Due practice includes items where `NextReviewAt` is now or earlier and the item is not suspended.

Due queries are bounded by a caller-provided limit with a safe maximum.

## Known Limitations

- No EF migration has been generated yet for the practice tables.
- Generation is simple and rule based.
- Quiz review targets whole quizzes rather than detailed weak skills.
- Word pronunciation practice type is defined but not fully generated yet.
- No AI-generated practice prompts are included yet by design.

## Next Recommended Step

Generate and review the EF migration for practice tables, then add richer quiz weakness tracking before any AI practice generation is considered.
