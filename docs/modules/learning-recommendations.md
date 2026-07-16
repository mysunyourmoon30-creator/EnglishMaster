# Learning Recommendations

## Purpose

Learning Recommendations provide simple rule-based suggestions for authenticated learners. The feature uses existing published content plus learner profile, progress, and quiz attempt data. It does not use AI, machine learning, or an external recommendation service.

## Recommendation Types

- `RecommendedCourse`
- `RecommendedLesson`
- `RecommendedWords`
- `RecommendedGrammar`
- `RecommendedQuiz`
- `ReviewWeakQuiz`
- `NextLessonInCourse`

## Reason Codes

- `SameCefrLevel`
- `NotStarted`
- `NewContentIfAvailable`
- `LowQuizScore`
- `NextInCourse`

## LearningRecommendationDto

Fields:

- `ContentType`
- `ContentId`
- `Slug`
- `Title`
- `Summary`
- `Url`
- `CefrLevel`
- `CategoryName`
- `Tags`
- `RecommendationType`
- `ReasonCode`
- `ReasonText`
- `Score`
- `SortOrder`

## Rules

- Recommended courses and lessons exclude completed progress.
- CEFR is taken from the query when provided, otherwise from `StudentProfile.CurrentCefrLevel`.
- Courses, lessons, books, and quizzes must be active and published.
- Words and grammar must be active.
- Quiz recommendations never include questions, choices, correct-answer flags, or answer keys.
- Review recommendations are created from failed quiz attempts.

## Next Lesson

The next lesson endpoint returns the first published active lesson in course order that the learner has not completed. If every course lesson is complete, it returns a safe not-found result.

## Known Limitations

- Recommendation scoring is intentionally simple.
- Category/tag inference from learning history is shallow.
- SQL migration for the new progress foundation still needs to be generated before database deployment.
- No AI/ML ranking, collaborative filtering, or personalization model exists.

