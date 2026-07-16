using EnglishMaster.Application.Features.LearningReports.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.LearningReports.Queries;

public sealed record GetCurrentWeekLearningReportQuery(Guid UserId);
public sealed record GetWeeklyLearningReportByIdQuery(Guid UserId, Guid ReportId);
public sealed record GetWeeklyLearningReportByDateQuery(Guid UserId, DateTimeOffset Date);
public sealed record GetMyLearningReportHistoryQuery(Guid UserId, int? PageNumber, int? PageSize, DateTimeOffset? FromDate, DateTimeOffset? ToDate);
public sealed record GetWeeklyLearningReportInsightsQuery(Guid UserId, Guid ReportId);

public sealed class LearningReportQueryHandler
{
    private readonly ILearningReportRepository repository;

    public LearningReportQueryHandler(ILearningReportRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<WeeklyLearningReportDto>> GetCurrentWeekAsync(GetCurrentWeekLearningReportQuery query, CancellationToken cancellationToken)
    {
        var report = await repository.GetCurrentWeekAsync(query.UserId, cancellationToken);
        return report is null ? Result<WeeklyLearningReportDto>.NotFound(nameof(query.UserId), "Learning report was not found.") : Result<WeeklyLearningReportDto>.Success(report);
    }

    public async Task<Result<WeeklyLearningReportDto>> GetByIdAsync(GetWeeklyLearningReportByIdQuery query, CancellationToken cancellationToken)
    {
        var report = await repository.GetByIdAsync(query.UserId, query.ReportId, cancellationToken);
        return report is null ? Result<WeeklyLearningReportDto>.NotFound(nameof(query.ReportId), "Learning report was not found.") : Result<WeeklyLearningReportDto>.Success(report);
    }

    public async Task<Result<WeeklyLearningReportDto>> GetByDateAsync(GetWeeklyLearningReportByDateQuery query, CancellationToken cancellationToken)
    {
        var report = await repository.GetByDateAsync(query.UserId, query.Date, cancellationToken);
        return report is null ? Result<WeeklyLearningReportDto>.NotFound(nameof(query.Date), "Learning report was not found.") : Result<WeeklyLearningReportDto>.Success(report);
    }

    public async Task<Result<IReadOnlyCollection<WeeklyLearningReportDto>>> GetHistoryAsync(GetMyLearningReportHistoryQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<WeeklyLearningReportDto>>.Success(await repository.GetHistoryAsync(query.UserId, Math.Max(query.PageNumber ?? 1, 1), Math.Clamp(query.PageSize ?? 20, 1, 50), query.FromDate, query.ToDate, cancellationToken));

    public async Task<Result<IReadOnlyCollection<WeeklyLearningReportInsightDto>>> GetInsightsAsync(GetWeeklyLearningReportInsightsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<WeeklyLearningReportInsightDto>>.Success(await repository.GetInsightsAsync(query.UserId, query.ReportId, cancellationToken));
}
