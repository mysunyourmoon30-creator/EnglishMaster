using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Analytics;

public sealed record AdminAnalyticsOverviewDto(
    int ActiveLearners,
    int StudyMinutes,
    int LearningActivities,
    int CourseCompletions,
    int QuizAttempts,
    decimal AverageQuizScore,
    int PracticeSessionsCompleted,
    int CertificatesIssued);

public sealed record StudentAnalyticsOverviewDto(
    int StudyMinutes,
    int LearningActivities,
    int LessonsCompleted,
    int CoursesCompleted,
    int BooksCompleted,
    int QuizAttempts,
    decimal AverageQuizScore,
    int PracticeSessionsCompleted,
    int CertificatesEarned,
    int CurrentStreakDays,
    int LongestStreakDays);

public sealed record AnalyticsDateRange(DateTimeOffset FromDate, DateTimeOffset ToDate);

public sealed record GetAdminAnalyticsOverviewQuery(DateTimeOffset? FromDate, DateTimeOffset? ToDate);

public sealed record GetStudentAnalyticsOverviewQuery(Guid UserId, DateTimeOffset? FromDate, DateTimeOffset? ToDate);

public interface IAnalyticsRepository
{
    Task<AdminAnalyticsOverviewDto> GetAdminOverviewAsync(AnalyticsDateRange range, CancellationToken cancellationToken);
    Task<StudentAnalyticsOverviewDto> GetStudentOverviewAsync(Guid userId, AnalyticsDateRange range, CancellationToken cancellationToken);
}

public sealed class AnalyticsQueryHandler
{
    private readonly IAnalyticsRepository repository;
    private readonly TimeProvider timeProvider;

    public AnalyticsQueryHandler(IAnalyticsRepository repository, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<AdminAnalyticsOverviewDto>> GetAdminOverviewAsync(GetAdminAnalyticsOverviewQuery query, CancellationToken cancellationToken) =>
        Result<AdminAnalyticsOverviewDto>.Success(await repository.GetAdminOverviewAsync(BuildRange(query.FromDate, query.ToDate), cancellationToken));

    public async Task<Result<StudentAnalyticsOverviewDto>> GetStudentOverviewAsync(GetStudentAnalyticsOverviewQuery query, CancellationToken cancellationToken) =>
        Result<StudentAnalyticsOverviewDto>.Success(await repository.GetStudentOverviewAsync(query.UserId, BuildRange(query.FromDate, query.ToDate), cancellationToken));

    private AnalyticsDateRange BuildRange(DateTimeOffset? fromDate, DateTimeOffset? toDate)
    {
        var now = timeProvider.GetUtcNow();
        var to = toDate ?? now;
        var from = fromDate ?? to.AddDays(-30);
        if (from > to)
        {
            (from, to) = (to, from);
        }

        if ((to - from).TotalDays > 366)
        {
            from = to.AddDays(-366);
        }

        return new AnalyticsDateRange(from, to);
    }
}
