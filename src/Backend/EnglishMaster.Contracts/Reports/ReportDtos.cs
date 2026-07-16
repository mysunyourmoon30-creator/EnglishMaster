namespace EnglishMaster.Contracts.Reports;

public sealed record AdminDashboardSummaryDto(
    OverviewMetricsDto Overview,
    ContentStatusSummaryDto ContentStatus,
    QuizAnalyticsSummaryDto QuizAnalytics,
    IReadOnlyCollection<RecentActivityDto> RecentActivity);

public sealed record OverviewMetricsDto(
    int TotalStudents,
    int TotalActiveStudents,
    int TotalWords,
    int TotalLessons,
    int TotalCourses,
    int TotalBooks,
    int TotalQuizzes,
    int TotalQuizAttempts,
    decimal AverageQuizScore);

public sealed record ContentStatusSummaryDto(
    int PublishedLessons,
    int DraftLessons,
    int InReviewContent,
    int PublishedCourses,
    int PublishedBooks,
    int PublishedQuizzes);

public sealed record LearningProgressSummaryDto(
    IReadOnlyCollection<LessonActivityDto> MostAccessedLessons,
    IReadOnlyCollection<LessonActivityDto> RecentlyStartedLessons,
    IReadOnlyCollection<LessonActivityDto> RecentlyCompletedLessons,
    ProgressMetricDto CourseProgress,
    ProgressMetricDto BookProgress);

public sealed record QuizAnalyticsSummaryDto(
    int TotalAttempts,
    decimal AverageScore,
    int PassedCount,
    int FailedCount,
    IReadOnlyCollection<QuizAverageScoreDto> AverageScoreByQuiz,
    IReadOnlyCollection<QuizAttemptSummaryDto> RecentAttempts,
    IReadOnlyCollection<QuizAttemptCountDto> TopQuizzesByAttempts);

public sealed record RecentActivitySummaryDto(IReadOnlyCollection<RecentActivityDto> Items);

public sealed record LessonActivityDto(Guid LessonId, string LessonTitle, int Count, DateTimeOffset? LastActivityAt);

public sealed record ProgressMetricDto(int StartedCount, int CompletedCount, decimal CompletionRate);

public sealed record QuizAverageScoreDto(Guid QuizId, string QuizTitle, decimal AverageScore, int AttemptCount);

public sealed record QuizAttemptSummaryDto(Guid QuizId, string QuizTitle, decimal Score, bool Passed, DateTimeOffset AttemptedAt);

public sealed record QuizAttemptCountDto(Guid QuizId, string QuizTitle, int AttemptCount);

public sealed record RecentActivityDto(string Type, string Title, DateTimeOffset OccurredAt);
