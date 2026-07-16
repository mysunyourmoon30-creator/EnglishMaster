# Practice Session

## Purpose

`PracticeSession` groups due practice items into one learner review session.

## PracticeSession Fields

```text
Id
StudentProfileId
StartedAt
CompletedAt
Status
TotalItems
CompletedItems
CorrectItems
IncorrectItems
CreatedAt
UpdatedAt
```

Status values:

```text
Started
Completed
Abandoned
```

## PracticeSessionItem Fields

```text
Id
PracticeSessionId
PracticeItemId
ContentType
ContentId
PracticeType
PromptText
AnswerText
UserAnswer
Result
IsCorrect
PracticedAt
CreatedAt
UpdatedAt
```

## Rules

- A session belongs to one student profile.
- A session item belongs to one session.
- A session item references one practice item.
- `Result` and `PracticedAt` are empty until the learner submits an answer.
- A session item can only be submitted once.
- A started session can be completed once.

## Quiz Answer Safety

Quiz practice items do not include correct answers in `AnswerText`. Quiz answer checking should remain server-side when interactive quiz submission is added.
