namespace EnglishMaster.Contracts.LearningReports;

public sealed record WeeklyLearningReportInsightDto(Guid Id, Guid WeeklyLearningReportId, string InsightType, string Severity, string Message, string Recommendation, int SortOrder);

public sealed record WeeklyLearningReportDto(
    Guid Id,
    DateTimeOffset WeekStartDate,
    DateTimeOffset WeekEndDate,
    string Status,
    DateTimeOffset GeneratedAt,
    int TotalStudyMinutes,
    int ActiveStudyDays,
    int CompletedDailyPlans,
    int LessonsStarted,
    int LessonsCompleted,
    int CoursesStarted,
    int CoursesCompleted,
    int BooksStarted,
    int BooksCompleted,
    int PracticeSessionsCompleted,
    int PracticeItemsCompleted,
    int QuizAttempts,
    int QuizzesPassed,
    decimal AverageQuizScore,
    int GoalsCompleted,
    int AchievementsEarned,
    int CurrentStreakDays,
    int LongestStreakDays,
    string SummaryText,
    IReadOnlyCollection<WeeklyLearningReportInsightDto> Insights);
