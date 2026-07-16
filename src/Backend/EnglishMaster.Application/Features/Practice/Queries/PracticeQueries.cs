using EnglishMaster.Application.Features.Practice.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Practice.Queries;

public sealed record GetDuePracticeItemsQuery(Guid UserId, int? Limit);
public sealed record GetPracticeSessionByIdQuery(Guid UserId, Guid SessionId);
public sealed record GetPracticeHistoryQuery(Guid UserId, int? Limit);
public sealed record GetPracticeSummaryQuery(Guid UserId);

public sealed class PracticeQueryHandler
{
    private readonly IPracticeRepository repository;

    public PracticeQueryHandler(IPracticeRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<IReadOnlyCollection<PracticeItemDto>>> GetDueAsync(GetDuePracticeItemsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<PracticeItemDto>>.Success(await repository.GetDuePracticeItemsAsync(query.UserId, Math.Clamp(query.Limit ?? 20, 1, 50), cancellationToken));

    public async Task<Result<PracticeSessionDto>> GetSessionAsync(GetPracticeSessionByIdQuery query, CancellationToken cancellationToken)
    {
        var session = await repository.GetPracticeSessionAsync(query.UserId, query.SessionId, cancellationToken);
        return session is null ? Result<PracticeSessionDto>.NotFound(nameof(query.SessionId), "Practice session was not found.") : Result<PracticeSessionDto>.Success(session);
    }

    public async Task<Result<IReadOnlyCollection<PracticeSessionDto>>> GetHistoryAsync(GetPracticeHistoryQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<PracticeSessionDto>>.Success(await repository.GetPracticeHistoryAsync(query.UserId, Math.Clamp(query.Limit ?? 20, 1, 50), cancellationToken));

    public async Task<Result<PracticeSummaryDto>> GetSummaryAsync(GetPracticeSummaryQuery query, CancellationToken cancellationToken) =>
        Result<PracticeSummaryDto>.Success(await repository.GetPracticeSummaryAsync(query.UserId, cancellationToken));
}

