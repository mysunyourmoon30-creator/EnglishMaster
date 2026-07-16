using EnglishMaster.Contracts.LearningGoals;

namespace EnglishMaster.Web.Services.LearningGoals;

public interface ILearningGoalApiClient
{
    Task<IReadOnlyCollection<LearningGoalDto>> GetGoalsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LearningGoalDto>> GetActiveGoalsAsync(CancellationToken cancellationToken);
    Task<LearningGoalSummaryDto> GetSummaryAsync(CancellationToken cancellationToken);
    Task<LearningGoalDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<LearningGoalDto> CreateAsync(CreateLearningGoalRequest request, CancellationToken cancellationToken);
    Task<LearningGoalDto> UpdateAsync(Guid id, UpdateLearningGoalRequest request, CancellationToken cancellationToken);
    Task<LearningGoalDto> PauseAsync(Guid id, CancellationToken cancellationToken);
    Task<LearningGoalDto> ResumeAsync(Guid id, CancellationToken cancellationToken);
    Task<LearningGoalDto> CompleteAsync(Guid id, CancellationToken cancellationToken);
    Task<LearningGoalDto> CancelAsync(Guid id, CancellationToken cancellationToken);
}
