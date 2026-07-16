using EnglishMaster.Application.Features.Reports.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Reports.Queries;

public sealed record GetAdminDashboardSummaryQuery;

public sealed record GetContentStatusSummaryQuery;

public sealed record GetLearningProgressSummaryQuery;

public sealed record GetQuizAnalyticsSummaryQuery;

public sealed record GetRecentActivitySummaryQuery(int PageSize = 20);

public sealed class GetAdminDashboardSummaryQueryHandler
{
    private readonly IReportRepository repository;

    public GetAdminDashboardSummaryQueryHandler(IReportRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<AdminDashboardSummaryDto>> HandleAsync(
        GetAdminDashboardSummaryQuery query,
        CancellationToken cancellationToken) =>
        Result<AdminDashboardSummaryDto>.Success(await repository.GetAdminDashboardSummaryAsync(cancellationToken));
}

public sealed class GetContentStatusSummaryQueryHandler
{
    private readonly IReportRepository repository;

    public GetContentStatusSummaryQueryHandler(IReportRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<ContentStatusSummaryDto>> HandleAsync(
        GetContentStatusSummaryQuery query,
        CancellationToken cancellationToken) =>
        Result<ContentStatusSummaryDto>.Success(await repository.GetContentStatusSummaryAsync(cancellationToken));
}

public sealed class GetLearningProgressSummaryQueryHandler
{
    private readonly IReportRepository repository;

    public GetLearningProgressSummaryQueryHandler(IReportRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<LearningProgressSummaryDto>> HandleAsync(
        GetLearningProgressSummaryQuery query,
        CancellationToken cancellationToken) =>
        Result<LearningProgressSummaryDto>.Success(await repository.GetLearningProgressSummaryAsync(cancellationToken));
}

public sealed class GetQuizAnalyticsSummaryQueryHandler
{
    private readonly IReportRepository repository;

    public GetQuizAnalyticsSummaryQueryHandler(IReportRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<QuizAnalyticsSummaryDto>> HandleAsync(
        GetQuizAnalyticsSummaryQuery query,
        CancellationToken cancellationToken) =>
        Result<QuizAnalyticsSummaryDto>.Success(await repository.GetQuizAnalyticsSummaryAsync(cancellationToken));
}

public sealed class GetRecentActivitySummaryQueryHandler
{
    private readonly IReportRepository repository;

    public GetRecentActivitySummaryQueryHandler(IReportRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<RecentActivitySummaryDto>> HandleAsync(
        GetRecentActivitySummaryQuery query,
        CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        return Result<RecentActivitySummaryDto>.Success(
            await repository.GetRecentActivitySummaryAsync(pageSize, cancellationToken));
    }
}
