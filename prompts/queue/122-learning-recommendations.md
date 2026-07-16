Create Learning Recommendations and Continue Learning for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Student-facing pages completed.
- Student Progress completed.
- Public Search and Discovery completed.

Goal:
Add rule-based Continue Learning and Learning Recommendations for authenticated learners.

Important:
Do not add AI.
Do not add machine learning.
Do not add external recommendation service.
Do not add marketplace.
Do not add mobile.
Do not add payment.
Do not add microservices.
Do not redesign architecture.

Scope:
Create simple rule-based recommendations using existing data:
- StudentProfile
- LessonProgress
- CourseProgress
- BookProgress
- QuizAttempt
- Published content
- CEFR level
- Categories
- Tags

Recommendation Types:
- ContinueLesson
- ContinueCourse
- ContinueBook
- NextLessonInCourse
- RecommendedCourse
- RecommendedLesson
- RecommendedGrammar
- RecommendedWords
- RecommendedQuiz
- ReviewWeakQuiz

Recommendation Reason Codes:
- RecentlyStarted
- InProgress
- NextInCourse
- SameCefrLevel
- SameCategory
- SameTag
- LowQuizScore
- NotStarted
- PopularContentIfAvailable
- NewContentIfAvailable

Application Requirements:

Create:
- Features/LearningRecommendations/Queries
- Features/LearningRecommendations/Dtos
- Features/LearningRecommendations/Services if needed

Use cases:
- GetMyContinueLearning
- GetMyLearningRecommendations
- GetRecommendedCourses
- GetRecommendedLessons
- GetRecommendedWords
- GetRecommendedGrammar
- GetRecommendedQuizzes
- GetNextLessonForCourse
- GetReviewRecommendations

DTOs:

1. ContinueLearningItemDto
Fields:
- ContentType
- ContentId
- Slug
- Title
- Summary
- Url
- ProgressPercent
- Status
- LastAccessedAt
- RecommendationReason
- SortOrder

2. LearningRecommendationDto
Fields:
- ContentType
- ContentId
- Slug
- Title
- Summary
- Url
- CefrLevel
- CategoryName
- Tags
- RecommendationType
- ReasonCode
- ReasonText
- Score
- SortOrder

3. LearningRecommendationSummaryDto
Fields:
- ContinueLearning
- RecommendedCourses
- RecommendedLessons
- RecommendedWords
- RecommendedGrammar
- RecommendedQuizzes
- ReviewRecommendations

Rules:

Continue Learning:
- Show lessons with InProgress first.
- Show courses with InProgress.
- Show books with InProgress.
- Sort by LastAccessedAt descending.
- Limit default to 5 or 10.
- Only show content that is still Published and Active.
- Do not show archived or draft content.

Next Lesson:
- For a course, find first incomplete lesson by course order.
- If all lessons completed, do not recommend next lesson.
- If no progress exists, recommend first published active lesson in course.

Recommended Courses:
- Match student's CurrentCefrLevel if available.
- Prefer Published and Active courses.
- Exclude completed courses.
- Exclude archived/draft courses.
- Sort by CEFR match, recency, title.

Recommended Lessons:
- Match CEFR level if available.
- Prefer same categories/tags from recently completed lessons.
- Exclude completed lessons.
- Show only Published and Active lessons.

Recommended Words:
- Match CEFR level.
- Match category/tag if available.
- Exclude words already associated with completed lessons if simple.
- Show only public-safe words.

Recommended Grammar:
- Match CEFR level if available.
- Recommend grammar topics/rules from incomplete lessons or current CEFR.
- Show only public-safe grammar content.

Recommended Quizzes:
- Recommend quizzes related to lessons/courses in progress.
- Recommend quizzes not attempted yet.
- Recommend quiz retake if latest score is below passing score.
- Do not expose correct answers before submit.

Review Recommendations:
- If quiz score is low, recommend related lesson/grammar/words if relation exists.
- If no relation exists, recommend the quiz for retry.
- Keep it simple.

API Requirements:

Create authenticated learner endpoints:
- GET /api/v1/me/learning/continue
- GET /api/v1/me/learning/recommendations
- GET /api/v1/me/learning/recommended-courses
- GET /api/v1/me/learning/recommended-lessons
- GET /api/v1/me/learning/recommended-words
- GET /api/v1/me/learning/recommended-grammar
- GET /api/v1/me/learning/recommended-quizzes
- GET /api/v1/me/learning/courses/{courseId}/next-lesson

Query parameters:
- limit
- cefrLevel optional
- contentType optional

Security:
- All /me/learning endpoints require authentication.
- User can only get their own recommendations.
- Do not expose another student's progress.
- Do not expose draft/in-review/archived content.
- Do not expose quiz correct answers.
- Do not expose internal file paths or admin fields.

Blazor Requirements:

Update Student Dashboard:
Route:
- /learn/dashboard

Add sections:
- Continue Learning
- Recommended Courses
- Recommended Lessons
- Recommended Words
- Recommended Grammar
- Recommended Quizzes
- Review Weak Areas

Create page:
- /learn/recommendations

Page should show:
- Continue learning cards
- Recommendations grouped by type
- Reason text such as:
  - Continue where you left off
  - Next lesson in your course
  - Matches your CEFR level
  - Recommended after low quiz score
- Loading state
- Empty state
- Error state

Update public/student navigation:
- Add Recommendations link if authenticated.
- Keep logged-out behavior safe.

Testing Requirements:
Add tests for:
- Continue learning returns in-progress lessons.
- Completed lessons are not returned as continue learning.
- Draft content does not appear in recommendations.
- Archived content does not appear in recommendations.
- Recommended course matches CEFR when available.
- Next lesson returns first incomplete lesson in course.
- Low quiz score creates review recommendation if possible.
- Quiz recommendations do not expose correct answers.
- User cannot access another user's recommendation data.
- Existing modules still pass.

Quality:
- Keep implementation simple.
- Rule-based only.
- No AI.
- No external search/recommendation engine.
- Use efficient EF Core queries.
- Use AsNoTracking where appropriate.
- Apply limit/max limit.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Recommendation logic summary
3. API endpoints created
4. Student dashboard updates
5. Recommendation page created
6. Security summary
7. Tests created
8. Build/test result
9. Remaining limitations