namespace EnglishMaster.Application.Features.Achievements.Dtos;

public sealed record AchievementDefinitionDto(Guid Id, string Code, string Name, string Description, string AchievementType, int TargetValue, string IconName, bool IsActive, int SortOrder);
public sealed record StudentAchievementDto(Guid Id, Guid AchievementDefinitionId, string Code, string Name, string Description, string AchievementType, int TargetValue, string IconName, string Status, int ProgressValue, DateTimeOffset? EarnedAt);
