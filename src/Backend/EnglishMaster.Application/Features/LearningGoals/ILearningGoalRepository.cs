using EnglishMaster.Application.Features.LearningGoals.Dtos;

namespace EnglishMaster.Application.Features.LearningGoals;

public interface ILearningGoalRepository
{
    Task<LearningGoalDto> CreateAsync(Guid userId, string goalType, string title, string? description, int targetValue, string? unit, DateTimeOffset? targetDate, CancellationToken cancellationToken);
    Task<LearningGoalDto?> UpdateAsync(Guid userId, Guid goalId, string title, string? description, int targetValue, int currentValue, string? unit, DateTimeOffset? targetDate, CancellationToken cancellationToken);
    Task<LearningGoalDto?> ChangeStatusAsync(Guid userId, Guid goalId, string action, CancellationToken cancellationToken);
    Task<LearningGoalDto?> GetByIdAsync(Guid userId, Guid goalId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LearningGoalDto>> GetGoalsAsync(Guid userId, bool activeOnly, int limit, CancellationToken cancellationToken);
    Task<LearningGoalSummaryDto> GetSummaryAsync(Guid userId, CancellationToken cancellationToken);
}
