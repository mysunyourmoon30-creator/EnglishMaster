using EnglishMaster.Application.Features.DailyStudyPlans.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.DailyStudyPlans.Commands;

public sealed record GenerateDailyStudyPlanCommand(Guid UserId);
public sealed record DailyStudyPlanItemLifecycleCommand(Guid UserId, Guid ItemId);
public sealed record CompleteDailyStudyPlanCommand(Guid UserId, Guid PlanId);

public sealed class DailyStudyPlanCommandHandler
{
    private readonly IDailyStudyPlanRepository repository;

    public DailyStudyPlanCommandHandler(IDailyStudyPlanRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<DailyStudyPlanDto>> GenerateTodayAsync(GenerateDailyStudyPlanCommand command, CancellationToken cancellationToken) =>
        Result<DailyStudyPlanDto>.Success(await repository.GenerateTodayAsync(command.UserId, cancellationToken));

    public async Task<Result<DailyStudyPlanItemDto>> CompleteItemAsync(DailyStudyPlanItemLifecycleCommand command, CancellationToken cancellationToken)
    {
        var item = await repository.CompleteItemAsync(command.UserId, command.ItemId, cancellationToken);
        return item is null ? Result<DailyStudyPlanItemDto>.NotFound(nameof(command.ItemId), "Study plan item was not found.") : Result<DailyStudyPlanItemDto>.Success(item);
    }

    public async Task<Result<DailyStudyPlanItemDto>> SkipItemAsync(DailyStudyPlanItemLifecycleCommand command, CancellationToken cancellationToken)
    {
        var item = await repository.SkipItemAsync(command.UserId, command.ItemId, cancellationToken);
        return item is null ? Result<DailyStudyPlanItemDto>.NotFound(nameof(command.ItemId), "Study plan item was not found.") : Result<DailyStudyPlanItemDto>.Success(item);
    }

    public async Task<Result<DailyStudyPlanDto>> CompletePlanAsync(CompleteDailyStudyPlanCommand command, CancellationToken cancellationToken)
    {
        var plan = await repository.CompletePlanAsync(command.UserId, command.PlanId, cancellationToken);
        return plan is null ? Result<DailyStudyPlanDto>.NotFound(nameof(command.PlanId), "Study plan was not found.") : Result<DailyStudyPlanDto>.Success(plan);
    }
}
