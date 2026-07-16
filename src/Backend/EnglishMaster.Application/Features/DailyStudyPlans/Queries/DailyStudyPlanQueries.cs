using EnglishMaster.Application.Features.DailyStudyPlans.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.DailyStudyPlans.Queries;

public sealed record GetTodayStudyPlanQuery(Guid UserId);
public sealed record GetStudyPlanByDateQuery(Guid UserId, DateTimeOffset PlanDate);
public sealed record GetStudyPlanByIdQuery(Guid UserId, Guid PlanId);
public sealed record GetMyStudyPlanHistoryQuery(Guid UserId, int? Limit);
public sealed record GetStudyPlanSummaryQuery(Guid UserId);

public sealed class DailyStudyPlanQueryHandler
{
    private readonly IDailyStudyPlanRepository repository;

    public DailyStudyPlanQueryHandler(IDailyStudyPlanRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<DailyStudyPlanDto>> GetTodayAsync(GetTodayStudyPlanQuery query, CancellationToken cancellationToken)
    {
        var plan = await repository.GetTodayAsync(query.UserId, cancellationToken);
        return plan is null ? Result<DailyStudyPlanDto>.NotFound(nameof(query.UserId), "Study plan was not found.") : Result<DailyStudyPlanDto>.Success(plan);
    }

    public async Task<Result<DailyStudyPlanDto>> GetByDateAsync(GetStudyPlanByDateQuery query, CancellationToken cancellationToken)
    {
        var plan = await repository.GetByDateAsync(query.UserId, query.PlanDate, cancellationToken);
        return plan is null ? Result<DailyStudyPlanDto>.NotFound(nameof(query.PlanDate), "Study plan was not found.") : Result<DailyStudyPlanDto>.Success(plan);
    }

    public async Task<Result<DailyStudyPlanDto>> GetByIdAsync(GetStudyPlanByIdQuery query, CancellationToken cancellationToken)
    {
        var plan = await repository.GetByIdAsync(query.UserId, query.PlanId, cancellationToken);
        return plan is null ? Result<DailyStudyPlanDto>.NotFound(nameof(query.PlanId), "Study plan was not found.") : Result<DailyStudyPlanDto>.Success(plan);
    }

    public async Task<Result<IReadOnlyCollection<DailyStudyPlanDto>>> GetHistoryAsync(GetMyStudyPlanHistoryQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<DailyStudyPlanDto>>.Success(await repository.GetHistoryAsync(query.UserId, Math.Clamp(query.Limit ?? 30, 1, 90), cancellationToken));

    public async Task<Result<DailyStudyPlanSummaryDto>> GetSummaryAsync(GetStudyPlanSummaryQuery query, CancellationToken cancellationToken) =>
        Result<DailyStudyPlanSummaryDto>.Success(await repository.GetSummaryAsync(query.UserId, cancellationToken));
}
