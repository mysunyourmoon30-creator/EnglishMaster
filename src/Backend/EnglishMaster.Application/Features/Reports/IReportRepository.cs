using EnglishMaster.Application.Features.Reports.Dtos;

namespace EnglishMaster.Application.Features.Reports;

public interface IReportRepository
{
    Task<AdminDashboardSummaryDto> GetAdminDashboardSummaryAsync(CancellationToken cancellationToken);

    Task<ContentStatusSummaryDto> GetContentStatusSummaryAsync(CancellationToken cancellationToken);

    Task<LearningProgressSummaryDto> GetLearningProgressSummaryAsync(CancellationToken cancellationToken);

    Task<QuizAnalyticsSummaryDto> GetQuizAnalyticsSummaryAsync(CancellationToken cancellationToken);

    Task<RecentActivitySummaryDto> GetRecentActivitySummaryAsync(int pageSize, CancellationToken cancellationToken);
}
