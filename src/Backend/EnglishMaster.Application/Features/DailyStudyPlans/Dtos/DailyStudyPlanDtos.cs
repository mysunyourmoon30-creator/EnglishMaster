namespace EnglishMaster.Application.Features.DailyStudyPlans.Dtos;

public sealed record DailyStudyPlanItemDto(
    Guid Id,
    Guid DailyStudyPlanId,
    string ItemType,
    string ContentType,
    Guid? ContentId,
    string Title,
    string Url,
    int EstimatedMinutes,
    int SortOrder,
    string Status,
    DateTimeOffset? CompletedAt);

public sealed record DailyStudyPlanDto(
    Guid Id,
    DateTimeOffset PlanDate,
    string Status,
    int TargetMinutes,
    int CompletedMinutes,
    int TotalItems,
    int CompletedItems,
    IReadOnlyCollection<DailyStudyPlanItemDto> Items);

public sealed record DailyStudyPlanSummaryDto(
    int PlansThisWeek,
    int CompletedPlans,
    int CompletedItems,
    int CompletedMinutes);
