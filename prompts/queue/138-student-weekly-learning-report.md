Create Student Weekly Summary and Learning Report for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Student-facing pages completed.
- Student Progress completed.
- Practice System completed.
- Learning Goals completed.
- Daily Study Plan completed.
- Streaks and Achievements completed.
- Learning Activity Log completed.

Goal:
Add simple rule-based weekly learning reports for authenticated learners.

Important:
Do not add AI.
Do not add machine learning.
Do not add external analytics service.
Do not add payment.
Do not add marketplace.
Do not add mobile.
Do not add microservices.
Do not redesign architecture.

Scope:
Create weekly learning report using existing learner data:
- LearningActivityLog
- StudentStreak
- StudentAchievement
- LearningGoal
- DailyStudyPlan
- PracticeSession
- QuizAttempt
- LessonProgress
- CourseProgress
- BookProgress

Report Period:
- Weekly
- Start date and end date should be explicit.
- Default report should be current week.
- Support previous weeks.

Report Sections:
1. Study Time Summary
- Total minutes studied
- Average minutes per active day
- Active study days
- Completed daily study plans

2. Learning Progress Summary
- Lessons started
- Lessons completed
- Courses started
- Courses completed
- Books started
- Books completed

3. Practice Summary
- Practice sessions completed
- Practice items completed
- Correct practice items
- Incorrect practice items
- Due items remaining

4. Quiz Summary
- Quiz attempts
- Quizzes passed
- Average quiz score
- Lowest quiz score
- Highest quiz score
- Weak quizzes if score below passing score

5. Goal Summary
- Active goals
- Completed goals this week
- Paused goals
- Goal progress snapshot

6. Streak Summary
- Current streak
- Longest streak
- Active days this week
- Missed days this week

7. Achievement Summary
- Achievements earned this week
- In-progress achievements

8. Rule-based Insight Summary
Use simple rule-based insight text only:
- If study minutes are low: "Study time was lower than your weekly goal."
- If quiz average is low: "Review quizzes with low scores."
- If practice due items are high: "You have practice items due."
- If streak improved: "Great job keeping your streak."
- If no activity: "Start with today's study plan."
- Do not use AI-generated text.

Domain Entities:

1. WeeklyLearningReport

Fields:
- Id
- StudentProfileId
- WeekStartDate
- WeekEndDate
- Status
- GeneratedAt
- TotalStudyMinutes
- ActiveStudyDays
- CompletedDailyPlans
- LessonsStarted
- LessonsCompleted
- CoursesStarted
- CoursesCompleted
- BooksStarted
- BooksCompleted
- PracticeSessionsCompleted
- PracticeItemsCompleted
- QuizAttempts
- QuizzesPassed
- AverageQuizScore
- GoalsCompleted
- AchievementsEarned
- CurrentStreakDays
- LongestStreakDays
- SummaryText
- CreatedAt
- UpdatedAt

Status values:
- Generated
- Stale
- Archived

Rules:
- StudentProfileId is required.
- WeekStartDate is required.
- WeekEndDate is required.
- WeekEndDate must be after or equal WeekStartDate.
- One report per student per WeekStartDate.
- Numeric counts must not be negative.
- AverageQuizScore must be between 0 and 100 if available.
- SummaryText must be rule-based only.

2. WeeklyLearningReportInsight

Fields:
- Id
- WeeklyLearningReportId
- InsightType
- Severity
- Message
- Recommendation
- SortOrder
- CreatedAt
- UpdatedAt

InsightType values:
- StudyTime
- Practice
- Quiz
- Goal
- Streak
- Achievement
- Inactivity

Severity values:
- Info
- Warning
- Positive

Rules:
- WeeklyLearningReportInsight belongs to one WeeklyLearningReport.
- InsightType is required.
- Severity is required.
- Message is required.
- SortOrder must not be negative.

Application Requirements:

Create CQRS-style folders:
- Features/LearningReports/Commands
- Features/LearningReports/Queries
- Features/LearningReports/Dtos

Use cases:
- GenerateWeeklyLearningReport
- RegenerateWeeklyLearningReport
- GetCurrentWeekLearningReport
- GetWeeklyLearningReportById
- GetWeeklyLearningReportByDate
- GetMyLearningReportHistory
- GetWeeklyLearningReportInsights
- ArchiveWeeklyLearningReport

Report Generation Rules:
- Use existing activity/progress/practice/quiz/goal/achievement data.
- Generate current week report on demand.
- If report already exists for the week, return existing report unless regenerate is requested.
- Current week report can be regenerated.
- Past reports should remain stable unless explicitly regenerated.
- Do not include other students' data.
- Do not expose draft/private/admin content.
- Do not expose quiz correct answers.
- Do not expose internal file paths.

Insight Rules:
- LowStudyTime:
  - If TotalStudyMinutes is less than weekly goal or less than 60 minutes when no goal exists.
- NoActivity:
  - If ActiveStudyDays is 0.
- PracticeDue:
  - If due practice items remain.
- LowQuizScore:
  - If AverageQuizScore is below 70 or below quiz passing score where available.
- StreakPositive:
  - If CurrentStreakDays is greater than zero.
- AchievementPositive:
  - If AchievementsEarned is greater than zero.
- GoalCompleted:
  - If GoalsCompleted is greater than zero.

API Requirements:

Authenticated learner endpoints:
- GET /api/v1/me/learning-reports/current-week
- GET /api/v1/me/learning-reports
- GET /api/v1/me/learning-reports/{id}
- GET /api/v1/me/learning-reports/by-date/{date}
- POST /api/v1/me/learning-reports/current-week/generate
- POST /api/v1/me/learning-reports/{id}/regenerate
- GET /api/v1/me/learning-reports/{id}/insights

Query parameters:
- pageNumber
- pageSize
- fromDate
- toDate

Security:
- All /me/learning-reports endpoints require authentication.
- Users can only access their own learning reports.
- Do not expose another student's reports.
- Do not expose quiz correct answers.
- Do not expose internal paths.
- Do not expose admin-only data.

Blazor Requirements:

Create pages:
- /learn/reports
- /learn/reports/current-week
- /learn/reports/{id:guid}

Update Student Dashboard:
- Add Weekly Summary card.
- Show study minutes this week.
- Show lessons completed this week.
- Show practice sessions this week.
- Show average quiz score.
- Show current streak.
- Link to full report.

Reports page:
- Show report history.
- Filter by date if simple.
- Pagination.
- Empty state.
- Loading state.
- Error state.

Current Week Report page:
- Generate report button.
- Regenerate button if report exists.
- Show sections:
  - Study Time
  - Learning Progress
  - Practice
  - Quiz
  - Goals
  - Streak
  - Achievements
  - Insights

Report Detail page:
- Show report period.
- Show all metrics.
- Show insights.
- Show recommendations.
- Safe empty states.

Testing Requirements:
Add tests for:
- Generate weekly report.
- One report per student per week.
- Current week report returns correct student data.
- User cannot access another student's report.
- Study minutes summary calculation.
- Quiz average calculation.
- Practice session count.
- Achievement earned count.
- Low study time insight.
- Low quiz score insight.
- No activity insight.
- Report does not expose quiz correct answers.
- Existing modules still pass.

Quality:
- Rule-based only.
- Keep implementation simple.
- Use efficient EF Core queries.
- Use AsNoTracking where appropriate.
- Apply pagination/limit to report history.
- Do not add charts library unless already available.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Domain entities created
3. Weekly report generation logic summary
4. Insight rule summary
5. API endpoints created
6. Blazor pages created
7. Student dashboard updates
8. Security summary
9. Tests created
10. Build/test result
11. Remaining limitations