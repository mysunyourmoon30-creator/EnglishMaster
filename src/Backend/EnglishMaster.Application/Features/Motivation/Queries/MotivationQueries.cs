using EnglishMaster.Application.Features.Motivation.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Motivation.Queries;

public sealed record GetMyLearningActivityQuery(Guid UserId, int? Limit);
public sealed record GetMyStreakQuery(Guid UserId);
public sealed record GetMyMotivationSummaryQuery(Guid UserId);

public sealed class MotivationQueryHandler
{
    private readonly IMotivationRepository repository;

    public MotivationQueryHandler(IMotivationRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<IReadOnlyCollection<LearningActivityDto>>> GetActivityAsync(GetMyLearningActivityQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<LearningActivityDto>>.Success(await repository.GetActivityAsync(query.UserId, Math.Clamp(query.Limit ?? 50, 1, 100), cancellationToken));

    public async Task<Result<StudentStreakDto>> GetStreakAsync(GetMyStreakQuery query, CancellationToken cancellationToken) =>
        Result<StudentStreakDto>.Success(await repository.GetStreakAsync(query.UserId, cancellationToken));

    public async Task<Result<MotivationSummaryDto>> GetSummaryAsync(GetMyMotivationSummaryQuery query, CancellationToken cancellationToken) =>
        Result<MotivationSummaryDto>.Success(await repository.GetSummaryAsync(query.UserId, cancellationToken));
}
