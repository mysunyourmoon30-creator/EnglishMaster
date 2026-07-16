using EnglishMaster.Application.Features.LearningReports.Dtos;

namespace EnglishMaster.Application.Features.LearningReports;

public interface ILearningReportRepository
{
    Task<WeeklyLearningReportDto> GenerateWeeklyReportAsync(Guid userId, DateTimeOffset weekStartDate, bool regenerate, CancellationToken cancellationToken);
    Task<WeeklyLearningReportDto?> GetCurrentWeekAsync(Guid userId, CancellationToken cancellationToken);
    Task<WeeklyLearningReportDto?> GetByIdAsync(Guid userId, Guid reportId, CancellationToken cancellationToken);
    Task<WeeklyLearningReportDto?> GetByDateAsync(Guid userId, DateTimeOffset date, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<WeeklyLearningReportDto>> GetHistoryAsync(Guid userId, int pageNumber, int pageSize, DateTimeOffset? fromDate, DateTimeOffset? toDate, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<WeeklyLearningReportInsightDto>> GetInsightsAsync(Guid userId, Guid reportId, CancellationToken cancellationToken);
    Task<WeeklyLearningReportDto?> ArchiveAsync(Guid userId, Guid reportId, CancellationToken cancellationToken);
}
