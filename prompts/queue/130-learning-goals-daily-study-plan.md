Create Learning Goals and Daily Study Plan for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Student-facing pages completed.
- Student Progress completed.
- Public Search completed.
- Learning Recommendations completed.
- Practice System and Spaced Repetition completed.

Goal:
Add simple learning goals and daily study plan for authenticated learners.

Important:
Do not add AI.
Do not add machine learning.
Do not add external calendar integration.
Do not add payment.
Do not add marketplace.
Do not add mobile.
Do not add microservices.
Do not redesign architecture.

Scope:
Create rule-based learning goals and daily study plan using existing data:
- StudentProfile
- LessonProgress
- CourseProgress
- BookProgress
- QuizAttempt
- PracticeItem
- PracticeSession
- Learning Recommendations
- Continue Learning

Goal Types:
- DailyStudyMinutes
- WeeklyStudyMinutes
- DailyPracticeItems
- WeeklyLessonsCompleted
- WeeklyQuizAttempts
- TargetCefrLevel
- CompleteCourse
- CompleteBook

Daily Plan Item Types:
- ContinueLesson
- ContinueCourse
- PracticeDueItems
- ReviewWeakQuiz
- RecommendedLesson
- RecommendedWords
- RecommendedGrammar
- TakeQuiz

Domain Entities:

1. LearningGoal

Fields:
- Id
- StudentProfileId
- GoalType
- Title
- Description
- TargetValue
- CurrentValue
- Unit
- TargetDate
- Status
- CreatedAt
- UpdatedAt

Status values:
- Active
- Completed
- Paused
- Cancelled

Rules:
- StudentProfileId is required.
- GoalType is required.
- Title is required.
- TargetValue must not be negative.
- CurrentValue must not be negative.
- CurrentValue cannot exceed TargetValue unless intentionally allowed.
- Goal starts as Active.
- Goal can be Completed, Paused, or Cancelled.

2. DailyStudyPlan

Fields:
- Id
- StudentProfileId
- PlanDate
- Status
- TargetMinutes
- CompletedMinutes
- TotalItems
- CompletedItems
- CreatedAt
- UpdatedAt

Status values:
- Planned
- InProgress
- Completed
- Skipped

Rules:
- StudentProfileId is required.
- PlanDate is required.
- One DailyStudyPlan per student per day.
- TargetMinutes must not be negative.
- CompletedMinutes must not be negative.

3. DailyStudyPlanItem

Fields:
- Id
- DailyStudyPlanId
- ItemType
- ContentType
- ContentId
- Title
- Url
- EstimatedMinutes
- SortOrder
- Status
- CompletedAt
- CreatedAt
- UpdatedAt

Status values:
- Pending
- InProgress
- Completed
- Skipped

Rules:
- DailyStudyPlanItem belongs to one DailyStudyPlan.
- ItemType is required.
- Title is required.
- EstimatedMinutes must not be negative.
- SortOrder must not be negative.
- CompletedAt is set when completed.

Application Requirements:

Create CQRS-style folders:
- Features/LearningGoals/Commands
- Features/LearningGoals/Queries
- Features/LearningGoals/Dtos
- Features/DailyStudyPlans/Commands
- Features/DailyStudyPlans/Queries
- Features/DailyStudyPlans/Dtos

Use cases for Learning Goals:
- CreateLearningGoal
- UpdateLearningGoal
- PauseLearningGoal
- ResumeLearningGoal
- CompleteLearningGoal
- CancelLearningGoal
- GetLearningGoalById
- GetMyLearningGoals
- GetMyActiveLearningGoals
- GetLearningGoalSummary

Use cases for Daily Study Plan:
- GenerateDailyStudyPlan
- GetTodayStudyPlan
- GetStudyPlanByDate
- GetStudyPlanById
- CompleteStudyPlanItem
- SkipStudyPlanItem
- CompleteDailyStudyPlan
- GetMyStudyPlanHistory
- GetStudyPlanSummary

Daily Plan Generation Rules:
- Prefer due practice items first.
- Then continue in-progress lessons/courses/books.
- Then next lesson in active course.
- Then weak quiz review if any.
- Then recommended lessons/words/grammar/quizzes.
- Do not include draft, archived, inactive, or private content.
- Do not expose quiz correct answers.
- Keep default plan simple:
  - 1 practice block
  - 1 continue learning item
  - 1 recommended item
  - 1 optional quiz/review item
- Respect target minutes if available.
- Avoid duplicate content in same daily plan.
- If no content exists, return safe empty plan.

API Requirements:

Learning Goal endpoints:
- GET /api/v1/me/learning-goals
- GET /api/v1/me/learning-goals/active
- GET /api/v1/me/learning-goals/summary
- GET /api/v1/me/learning-goals/{id}
- POST /api/v1/me/learning-goals
- PUT /api/v1/me/learning-goals/{id}
- POST /api/v1/me/learning-goals/{id}/pause
- POST /api/v1/me/learning-goals/{id}/resume
- POST /api/v1/me/learning-goals/{id}/complete
- POST /api/v1/me/learning-goals/{id}/cancel

Daily Study Plan endpoints:
- GET /api/v1/me/study-plan/today
- POST /api/v1/me/study-plan/today/generate
- GET /api/v1/me/study-plan/date/{date}
- GET /api/v1/me/study-plan/{id}
- POST /api/v1/me/study-plan/items/{id}/complete
- POST /api/v1/me/study-plan/items/{id}/skip
- POST /api/v1/me/study-plan/{id}/complete
- GET /api/v1/me/study-plan/history
- GET /api/v1/me/study-plan/summary

Security:
- All /me learning goal and study plan endpoints require authentication.
- Users can only access their own goals and plans.
- Do not expose another student's study data.
- Do not expose draft/in-review/archived content.
- Do not expose quiz correct answers.
- Do not expose internal file paths.

Blazor Requirements:

Create pages:
- /learn/goals
- /learn/goals/create
- /learn/goals/{id:guid}
- /learn/study-plan
- /learn/study-plan/history

Update Student Dashboard:
- Add Today's Study Plan section.
- Add Active Goals section.
- Add Study Summary card.
- Add quick button: Generate Today Plan.
- Add quick button: Start Practice.

Learning Goals page:
- Show active goals.
- Show completed goals.
- Create goal.
- Pause/resume/cancel goal.
- Show progress toward target.

Daily Study Plan page:
- Show today's plan.
- Show target minutes.
- Show completed minutes.
- Show plan items.
- Complete item button.
- Skip item button.
- Generate plan button.
- Empty state.
- Loading state.
- Error state.

Testing Requirements:
Add tests for:
- Create learning goal.
- Pause/resume learning goal.
- Complete learning goal.
- Prevent user accessing another user's goal.
- Generate daily study plan.
- One plan per student per day.
- Due practice items appear first.
- Draft content does not appear in plan.
- Complete study plan item.
- Complete daily study plan.
- Existing modules still pass.

Quality:
- Rule-based only.
- Keep implementation simple.
- Use efficient EF Core queries.
- Use AsNoTracking where appropriate.
- Do not add external calendar.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Domain entities created
3. Study plan generation logic summary
4. API endpoints created
5. Blazor pages created
6. Student dashboard updates
7. Security summary
8. Tests created
9. Build/test result
10. Remaining limitations