using EnglishMaster.Application.Features.ContentRevisionRestores.Dtos;
using EnglishMaster.Application.Features.ContentRevisions;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.ContentRevisionRestores.Queries;

public sealed record SearchContentRevisionRestoreRequestsQuery(string? Status, int? PageNumber, int? PageSize);
public sealed record GetContentRevisionRestoreRequestByIdQuery(Guid Id);

public sealed class ContentRevisionRestoreQueryHandler
{
    private readonly IContentRevisionRepository repository;

    public ContentRevisionRestoreQueryHandler(IContentRevisionRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<ContentRevisionRestoreRequestSearchResponse>> SearchAsync(SearchContentRevisionRestoreRequestsQuery query, CancellationToken cancellationToken) =>
        Result<ContentRevisionRestoreRequestSearchResponse>.Success(await repository.SearchRestoreRequestsAsync(query.Status, Math.Max(query.PageNumber ?? 1, 1), Math.Clamp(query.PageSize ?? 20, 1, 100), cancellationToken));

    public async Task<Result<ContentRevisionRestoreRequestDto>> GetAsync(GetContentRevisionRestoreRequestByIdQuery query, CancellationToken cancellationToken)
    {
        var request = await repository.GetRestoreRequestAsync(query.Id, cancellationToken);
        return request is null
            ? Result<ContentRevisionRestoreRequestDto>.NotFound(nameof(query.Id), "Restore request was not found.")
            : Result<ContentRevisionRestoreRequestDto>.Success(request);
    }
}
