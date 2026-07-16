using EnglishMaster.Application.Features.LearningGoals.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.LearningGoals.Queries;

public sealed record GetLearningGoalByIdQuery(Guid UserId, Guid GoalId);
public sealed record GetMyLearningGoalsQuery(Guid UserId, int? Limit);
public sealed record GetMyActiveLearningGoalsQuery(Guid UserId, int? Limit);
public sealed record GetLearningGoalSummaryQuery(Guid UserId);

public sealed class LearningGoalQueryHandler
{
    private readonly ILearningGoalRepository repository;

    public LearningGoalQueryHandler(ILearningGoalRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<LearningGoalDto>> GetByIdAsync(GetLearningGoalByIdQuery query, CancellationToken cancellationToken)
    {
        var goal = await repository.GetByIdAsync(query.UserId, query.GoalId, cancellationToken);
        return goal is null ? Result<LearningGoalDto>.NotFound(nameof(query.GoalId), "Learning goal was not found.") : Result<LearningGoalDto>.Success(goal);
    }

    public async Task<Result<IReadOnlyCollection<LearningGoalDto>>> GetGoalsAsync(GetMyLearningGoalsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<LearningGoalDto>>.Success(await repository.GetGoalsAsync(query.UserId, activeOnly: false, Math.Clamp(query.Limit ?? 50, 1, 100), cancellationToken));

    public async Task<Result<IReadOnlyCollection<LearningGoalDto>>> GetActiveGoalsAsync(GetMyActiveLearningGoalsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<LearningGoalDto>>.Success(await repository.GetGoalsAsync(query.UserId, activeOnly: true, Math.Clamp(query.Limit ?? 20, 1, 50), cancellationToken));

    public async Task<Result<LearningGoalSummaryDto>> GetSummaryAsync(GetLearningGoalSummaryQuery query, CancellationToken cancellationToken) =>
        Result<LearningGoalSummaryDto>.Success(await repository.GetSummaryAsync(query.UserId, cancellationToken));
}
