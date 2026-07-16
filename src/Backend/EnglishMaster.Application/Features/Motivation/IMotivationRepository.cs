using EnglishMaster.Application.Features.Achievements.Dtos;
using EnglishMaster.Application.Features.Motivation.Dtos;

namespace EnglishMaster.Application.Features.Motivation;

public interface IMotivationRepository
{
    Task<LearningActivityDto> RecordActivityAsync(Guid userId, string activityType, string? contentType, Guid? contentId, string? title, DateTimeOffset occurredAt, int minutesSpent, string? metadataJson, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LearningActivityDto>> GetActivityAsync(Guid userId, int limit, CancellationToken cancellationToken);
    Task<StudentStreakDto> UpdateStreakAsync(Guid userId, DateTimeOffset occurredAt, CancellationToken cancellationToken);
    Task<StudentStreakDto> GetStreakAsync(Guid userId, CancellationToken cancellationToken);
    Task<MotivationSummaryDto> GetSummaryAsync(Guid userId, CancellationToken cancellationToken);
    Task<int> SeedDefaultAchievementDefinitionsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<AchievementDefinitionDto>> SearchDefinitionsAsync(int limit, CancellationToken cancellationToken);
    Task<AchievementDefinitionDto?> CreateDefinitionAsync(string code, string name, string? description, string achievementType, int targetValue, string? iconName, int sortOrder, CancellationToken cancellationToken);
    Task<AchievementDefinitionDto?> UpdateDefinitionAsync(Guid id, string name, string? description, string achievementType, int targetValue, string? iconName, int sortOrder, CancellationToken cancellationToken);
    Task<AchievementDefinitionDto?> SetDefinitionActiveAsync(Guid id, bool active, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudentAchievementDto>> EvaluateAchievementsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudentAchievementDto>> GetAchievementsAsync(Guid userId, bool earnedOnly, int limit, CancellationToken cancellationToken);
}
