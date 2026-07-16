using EnglishMaster.Contracts.LearningReports;

namespace EnglishMaster.Web.Services.LearningReports;

public interface ILearningReportApiClient
{
    Task<WeeklyLearningReportDto?> GetCurrentWeekAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<WeeklyLearningReportDto>> GetHistoryAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate, CancellationToken cancellationToken);
    Task<WeeklyLearningReportDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<WeeklyLearningReportDto?> GetByDateAsync(DateTimeOffset date, CancellationToken cancellationToken);
    Task<WeeklyLearningReportDto> GenerateCurrentWeekAsync(CancellationToken cancellationToken);
    Task<WeeklyLearningReportDto> RegenerateAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<WeeklyLearningReportInsightDto>> GetInsightsAsync(Guid id, CancellationToken cancellationToken);
}
