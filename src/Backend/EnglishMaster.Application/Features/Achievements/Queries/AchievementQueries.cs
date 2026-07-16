using EnglishMaster.Application.Features.Achievements.Dtos;
using EnglishMaster.Application.Features.Motivation;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Achievements.Queries;

public sealed record SearchAchievementDefinitionsQuery(int? Limit);
public sealed record GetMyAchievementsQuery(Guid UserId, int? Limit);
public sealed record GetMyEarnedAchievementsQuery(Guid UserId, int? Limit);

public sealed class AchievementQueryHandler
{
    private readonly IMotivationRepository repository;

    public AchievementQueryHandler(IMotivationRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<IReadOnlyCollection<AchievementDefinitionDto>>> SearchDefinitionsAsync(SearchAchievementDefinitionsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<AchievementDefinitionDto>>.Success(await repository.SearchDefinitionsAsync(Math.Clamp(query.Limit ?? 50, 1, 100), cancellationToken));

    public async Task<Result<IReadOnlyCollection<StudentAchievementDto>>> GetMyAchievementsAsync(GetMyAchievementsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<StudentAchievementDto>>.Success(await repository.GetAchievementsAsync(query.UserId, earnedOnly: false, Math.Clamp(query.Limit ?? 100, 1, 200), cancellationToken));

    public async Task<Result<IReadOnlyCollection<StudentAchievementDto>>> GetMyEarnedAchievementsAsync(GetMyEarnedAchievementsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<StudentAchievementDto>>.Success(await repository.GetAchievementsAsync(query.UserId, earnedOnly: true, Math.Clamp(query.Limit ?? 50, 1, 100), cancellationToken));
}
