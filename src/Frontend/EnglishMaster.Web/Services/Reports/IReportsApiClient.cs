using EnglishMaster.Contracts.Reports;

namespace EnglishMaster.Web.Services.Reports;

public interface IReportsApiClient
{
    Task<AdminDashboardSummaryDto> GetAdminDashboardSummaryAsync(CancellationToken cancellationToken);

    Task<ContentStatusSummaryDto> GetContentStatusSummaryAsync(CancellationToken cancellationToken);

    Task<LearningProgressSummaryDto> GetLearningProgressSummaryAsync(CancellationToken cancellationToken);

    Task<QuizAnalyticsSummaryDto> GetQuizAnalyticsSummaryAsync(CancellationToken cancellationToken);

    Task<RecentActivitySummaryDto> GetRecentActivitySummaryAsync(int pageSize, CancellationToken cancellationToken);
}
