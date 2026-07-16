using EnglishMaster.Application.Features.DailyStudyPlans.Dtos;

namespace EnglishMaster.Application.Features.DailyStudyPlans;

public interface IDailyStudyPlanRepository
{
    Task<DailyStudyPlanDto> GenerateTodayAsync(Guid userId, CancellationToken cancellationToken);
    Task<DailyStudyPlanDto?> GetTodayAsync(Guid userId, CancellationToken cancellationToken);
    Task<DailyStudyPlanDto?> GetByDateAsync(Guid userId, DateTimeOffset planDate, CancellationToken cancellationToken);
    Task<DailyStudyPlanDto?> GetByIdAsync(Guid userId, Guid planId, CancellationToken cancellationToken);
    Task<DailyStudyPlanItemDto?> CompleteItemAsync(Guid userId, Guid itemId, CancellationToken cancellationToken);
    Task<DailyStudyPlanItemDto?> SkipItemAsync(Guid userId, Guid itemId, CancellationToken cancellationToken);
    Task<DailyStudyPlanDto?> CompletePlanAsync(Guid userId, Guid planId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DailyStudyPlanDto>> GetHistoryAsync(Guid userId, int limit, CancellationToken cancellationToken);
    Task<DailyStudyPlanSummaryDto> GetSummaryAsync(Guid userId, CancellationToken cancellationToken);
}
