using EnglishMaster.Contracts.Achievements;
using EnglishMaster.Contracts.Motivation;

namespace EnglishMaster.Web.Services.Motivation;

public interface IMotivationApiClient
{
    Task<MotivationSummaryDto> GetSummaryAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LearningActivityDto>> GetActivityAsync(CancellationToken cancellationToken);
    Task<StudentStreakDto> GetStreakAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudentAchievementDto>> GetAchievementsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudentAchievementDto>> GetEarnedAchievementsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudentAchievementDto>> EvaluateAchievementsAsync(CancellationToken cancellationToken);
}
