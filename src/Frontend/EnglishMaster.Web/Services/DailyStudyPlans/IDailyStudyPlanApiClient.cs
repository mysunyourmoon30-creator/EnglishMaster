using EnglishMaster.Contracts.DailyStudyPlans;

namespace EnglishMaster.Web.Services.DailyStudyPlans;

public interface IDailyStudyPlanApiClient
{
    Task<DailyStudyPlanDto?> GetTodayAsync(CancellationToken cancellationToken);
    Task<DailyStudyPlanDto> GenerateTodayAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DailyStudyPlanDto>> GetHistoryAsync(CancellationToken cancellationToken);
    Task<DailyStudyPlanSummaryDto> GetSummaryAsync(CancellationToken cancellationToken);
    Task<DailyStudyPlanItemDto> CompleteItemAsync(Guid id, CancellationToken cancellationToken);
    Task<DailyStudyPlanItemDto> SkipItemAsync(Guid id, CancellationToken cancellationToken);
    Task<DailyStudyPlanDto> CompletePlanAsync(Guid id, CancellationToken cancellationToken);
}
