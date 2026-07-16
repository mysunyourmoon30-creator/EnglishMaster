Create Practice System and Spaced Repetition Foundation for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Student-facing pages completed.
- Student Progress completed.
- Public Search completed.
- Learning Recommendations completed.

Goal:
Add a simple rule-based practice and spaced repetition system for authenticated learners.

Important:
Do not add AI.
Do not add machine learning.
Do not add external SRS service.
Do not add mobile.
Do not add marketplace.
Do not add payment.
Do not add microservices.
Do not redesign architecture.

Scope:
Create simple practice sessions and review scheduling.

Practice Targets:
- Words
- Grammar Rules
- Minimal Pairs
- Quizzes / weak quiz areas

Practice Types:
- WordFlashcard
- WordMeaning
- WordPronunciation
- GrammarReview
- MinimalPairPractice
- QuizReview

Practice Result Values:
- Again
- Hard
- Good
- Easy

Spaced Repetition Rule:
Use simple rule-based intervals:
- Again: review tomorrow
- Hard: review in 2 days
- Good: review in 4 days
- Easy: review in 7 days

For repeated correct answers:
- Increase interval gradually: 1, 2, 4, 7, 14, 30 days
- Keep it simple.
- Do not implement complex SM-2 unless already very simple.

Domain Entities:

1. PracticeItem

Fields:
- Id
- StudentProfileId
- ContentType
- ContentId
- PracticeType
- Status
- DueAt
- LastPracticedAt
- NextReviewAt
- ReviewCount
- CorrectCount
- IncorrectCount
- CurrentIntervalDays
- CreatedAt
- UpdatedAt

Status values:
- New
- Due
- Learning
- Reviewing
- Mastered
- Suspended

Rules:
- StudentProfileId is required.
- ContentType is required.
- ContentId is required.
- PracticeType is required.
- DueAt is required.
- CurrentIntervalDays must not be negative.
- ReviewCount must not be negative.
- CorrectCount must not be negative.
- IncorrectCount must not be negative.

2. PracticeSession

Fields:
- Id
- StudentProfileId
- StartedAt
- CompletedAt
- Status
- TotalItems
- CompletedItems
- CorrectItems
- IncorrectItems
- CreatedAt
- UpdatedAt

Status values:
- Started
- Completed
- Abandoned

Rules:
- PracticeSession belongs to one student.
- CompletedAt is set when completed.
- Counts must not be negative.

3. PracticeSessionItem

Fields:
- Id
- PracticeSessionId
- PracticeItemId
- ContentType
- ContentId
- PracticeType
- PromptText
- AnswerText
- UserAnswer
- Result
- IsCorrect
- PracticedAt
- CreatedAt
- UpdatedAt

Rules:
- PracticeSessionItem belongs to one session.
- PracticeItemId is required.
- Result is optional until answered.
- PracticedAt is set when answered.

Application Requirements:

Create CQRS-style folders:
- Features/Practice/Commands
- Features/Practice/Queries
- Features/Practice/Dtos

Use cases:
- CreatePracticeItem
- GeneratePracticeItemsForStudent
- GetDuePracticeItems
- StartPracticeSession
- SubmitPracticeSessionItem
- CompletePracticeSession
- GetPracticeSessionById
- GetMyPracticeHistory
- GetPracticeSummary
- SuspendPracticeItem
- ResumePracticeItem

Practice generation rules:
- Generate word practice items from published active words.
- Generate grammar practice items from published active grammar rules.
- Generate minimal pair practice items where minimal pairs exist.
- Generate quiz review items from low quiz scores.
- Do not create duplicate PracticeItem for same student + content + practice type.
- Do not include draft, archived, or inactive content.

API Requirements:

Create authenticated learner endpoints:
- GET /api/v1/me/practice/summary
- GET /api/v1/me/practice/due
- POST /api/v1/me/practice/generate
- POST /api/v1/me/practice/sessions/start
- GET /api/v1/me/practice/sessions/{id}
- POST /api/v1/me/practice/session-items/{id}/submit
- POST /api/v1/me/practice/sessions/{id}/complete
- GET /api/v1/me/practice/history
- POST /api/v1/me/practice/items/{id}/suspend
- POST /api/v1/me/practice/items/{id}/resume

Security:
- All /me/practice endpoints require authentication.
- Users can only access their own practice data.
- Do not expose another student's practice data.
- Do not expose quiz correct answers before answer submission.
- Do not expose draft/in-review/archived content.
- Do not expose internal storage paths.

Blazor Requirements:

Create pages:
- /learn/practice
- /learn/practice/session/{id:guid}
- /learn/practice/history

Practice Dashboard:
- Due today count
- New practice items count
- Reviewing count
- Mastered count
- Start practice button
- Generate practice items button if needed

Practice Session Page:
- Show one item at a time or simple list if easier.
- Show prompt.
- Let user submit answer or select result.
- Show result after submission.
- Update item schedule.
- Show progress in session.

Practice History Page:
- Show previous sessions.
- Show score.
- Show completed date.

Update Student Dashboard:
- Add Practice Due card.
- Add Start Practice button.
- Add recent practice summary.

Testing Requirements:
Add tests for:
- Create practice item
- Generate practice items without duplicates
- Due practice item query
- Start practice session
- Submit practice item with Good result
- Submit practice item with Again result
- NextReviewAt calculation
- Complete practice session
- User cannot access another user's practice data
- Draft content is not used for practice
- Existing modules still pass

Quality:
- Rule-based only.
- Keep implementation simple.
- Use efficient EF Core queries.
- Use AsNoTracking where appropriate.
- Apply limit/max limit.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Domain entities created
3. Practice scheduling logic summary
4. API endpoints created
5. Blazor pages created
6. Student dashboard updates
7. Security summary
8. Tests created
9. Build/test result
10. Remaining limitations