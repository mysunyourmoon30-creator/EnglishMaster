Create Student Progress and Learning Tracking for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Content Review Workflow completed.
- Better Publishing completed.
- Student-facing Learning Pages completed.

Goal:
Add basic student progress tracking for lessons, courses, books, and quizzes.

Important:
Do not add AI.
Do not add Mobile.
Do not add Marketplace.
Do not add Payment.
Do not add Certificates yet.
Do not add advanced analytics yet.
Do not redesign the architecture.

Scope:
Create simple progress tracking for authenticated learners.

Target Areas:
- Lesson progress
- Course progress
- Book progress
- Quiz attempt summary

Domain Entities:

1. StudentProfile

Fields:
- Id
- UserId
- DisplayName
- CurrentCefrLevel
- LearningGoal
- IsActive
- CreatedAt
- UpdatedAt

Rules:
- StudentProfile belongs to one user.
- UserId is required.
- DisplayName is optional.
- StudentProfile supports Activate and Deactivate behavior.

2. LessonProgress

Fields:
- Id
- StudentProfileId
- LessonId
- Status
- StartedAt
- CompletedAt
- LastAccessedAt
- ProgressPercent
- CreatedAt
- UpdatedAt

Status values:
- NotStarted
- InProgress
- Completed

Rules:
- A student can have only one progress record per lesson.
- ProgressPercent must be between 0 and 100.
- CompletedAt must be set when completed.
- LastAccessedAt should update when lesson is opened.

3. CourseProgress

Fields:
- Id
- StudentProfileId
- CourseId
- Status
- StartedAt
- CompletedAt
- LastAccessedAt
- ProgressPercent
- CompletedLessonCount
- TotalLessonCount
- CreatedAt
- UpdatedAt

Rules:
- A student can have only one progress record per course.
- ProgressPercent must be between 0 and 100.
- Progress should be calculated from lesson completion if possible.

4. BookProgress

Fields:
- Id
- StudentProfileId
- BookId
- Status
- StartedAt
- CompletedAt
- LastAccessedAt
- ProgressPercent
- CompletedChapterCount
- TotalChapterCount
- CreatedAt
- UpdatedAt

Rules:
- A student can have only one progress record per book.
- ProgressPercent must be between 0 and 100.

5. QuizAttempt

Fields:
- Id
- StudentProfileId
- QuizId
- StartedAt
- SubmittedAt
- Score
- MaxScore
- PercentScore
- Passed
- CreatedAt
- UpdatedAt

Rules:
- QuizAttempt belongs to one student.
- QuizAttempt belongs to one quiz.
- PercentScore must be between 0 and 100.
- Passed is based on quiz PassingScore if available.
- Do not expose correct answers before submit.

6. QuizAttemptAnswer

Fields:
- Id
- QuizAttemptId
- QuizQuestionId
- SelectedChoiceId
- AnswerText
- IsCorrect
- PointsEarned
- CreatedAt
- UpdatedAt

Rules:
- QuizAttemptAnswer belongs to one QuizAttempt.
- SelectedChoiceId is optional for non-choice questions.
- AnswerText is optional.
- IsCorrect should be calculated after submission.

Application Requirements:

Create CQRS-style folders:

For StudentProfiles:
- Features/StudentProfiles/Commands
- Features/StudentProfiles/Queries
- Features/StudentProfiles/Dtos

Use cases:
- CreateStudentProfile
- UpdateStudentProfile
- GetMyStudentProfile
- GetStudentProfileById
- SearchStudentProfiles

For LessonProgress:
- StartLesson
- CompleteLesson
- GetMyLessonProgress
- GetLessonProgressByLessonId
- GetMyRecentLessons

For CourseProgress:
- StartCourse
- GetMyCourseProgress
- GetCourseProgressByCourseId
- RecalculateCourseProgress

For BookProgress:
- StartBook
- GetMyBookProgress
- GetBookProgressByBookId
- RecalculateBookProgress

For QuizAttempts:
- StartQuizAttempt
- SubmitQuizAttempt
- GetQuizAttemptById
- GetMyQuizAttempts
- GetMyQuizAttemptsByQuizId

Infrastructure Requirements:
- Create EF Core configurations for all new entities.
- Configure unique indexes:
  - StudentProfile UserId
  - LessonProgress StudentProfileId + LessonId
  - CourseProgress StudentProfileId + CourseId
  - BookProgress StudentProfileId + BookId
- Configure indexes:
  - LessonProgress LessonId
  - CourseProgress CourseId
  - BookProgress BookId
  - QuizAttempt StudentProfileId
  - QuizAttempt QuizId
  - QuizAttempt SubmittedAt
- Add migrations if EF Core tools are available.
- Do not break existing modules.

API Requirements:

Create Student endpoints:
- GET /api/v1/me/student-profile
- PUT /api/v1/me/student-profile

Create Progress endpoints:
- POST /api/v1/me/lessons/{lessonId}/start
- POST /api/v1/me/lessons/{lessonId}/complete
- GET /api/v1/me/lessons/{lessonId}/progress
- GET /api/v1/me/recent-lessons

- POST /api/v1/me/courses/{courseId}/start
- GET /api/v1/me/courses/{courseId}/progress
- GET /api/v1/me/courses/progress

- POST /api/v1/me/books/{bookId}/start
- GET /api/v1/me/books/{bookId}/progress
- GET /api/v1/me/books/progress

Create Quiz Attempt endpoints:
- POST /api/v1/me/quizzes/{quizId}/attempts/start
- POST /api/v1/me/quiz-attempts/{attemptId}/submit
- GET /api/v1/me/quiz-attempts
- GET /api/v1/me/quiz-attempts/{attemptId}

Security Rules:
- All /me endpoints require authentication.
- Users can only access their own progress.
- Admins should not be able to accidentally modify student progress through public endpoints.
- Do not expose other students' progress.

Blazor Requirements:

Update Student-facing pages:
- Lesson detail page:
  - Start lesson button if authenticated.
  - Mark as completed button.
  - Show progress status.

- Course detail page:
  - Start course button.
  - Show lesson completion progress.
  - Show progress percent.

- Book detail page:
  - Start book button.
  - Show book progress if available.

- Quiz page:
  - Start quiz attempt.
  - Submit answers.
  - Show result after submit.
  - Do not show correct answers before submit.
  - Show score and passed/failed after submit.

Create student dashboard:
- /learn/dashboard

Dashboard should show:
- Current student profile
- Recent lessons
- Course progress
- Book progress
- Recent quiz attempts

Navigation:
- Add Student Dashboard link to public navigation if authenticated.

Testing Requirements:
Add tests for:
- StudentProfile creation
- Start lesson
- Complete lesson
- Course progress recalculation
- Book progress creation
- Start quiz attempt
- Submit quiz attempt
- Score calculation
- User cannot access another user's progress
- Existing public/admin modules still pass

Quality:
- Keep implementation simple.
- Do not overengineer.
- Do not add analytics dashboards yet.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Domain entities created
3. API endpoints created
4. Student pages updated
5. Student dashboard created
6. Security summary
7. Tests created
8. Build/test result
9. Remaining limitations