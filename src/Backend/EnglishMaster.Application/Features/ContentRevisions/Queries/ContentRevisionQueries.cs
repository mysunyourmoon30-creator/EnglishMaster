using EnglishMaster.Application.Features.ContentRevisions.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.ContentRevisions.Queries;

public sealed record GetContentRevisionByIdQuery(Guid Id);
public sealed record SearchContentRevisionsQuery(string? ContentType, Guid? ContentId, string? EventType, int? PageNumber, int? PageSize);
public sealed record GetLatestContentRevisionQuery(string ContentType, Guid ContentId);

public sealed class ContentRevisionQueryHandler
{
    private readonly IContentRevisionRepository repository;

    public ContentRevisionQueryHandler(IContentRevisionRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<ContentRevisionDto>> GetAsync(GetContentRevisionByIdQuery query, CancellationToken cancellationToken)
    {
        var revision = await repository.GetRevisionAsync(query.Id, cancellationToken);
        return revision is null
            ? Result<ContentRevisionDto>.NotFound(nameof(query.Id), "Content revision was not found.")
            : Result<ContentRevisionDto>.Success(revision);
    }

    public async Task<Result<ContentRevisionSearchResponse>> SearchAsync(SearchContentRevisionsQuery query, CancellationToken cancellationToken) =>
        Result<ContentRevisionSearchResponse>.Success(await repository.SearchRevisionsAsync(query.ContentType, query.ContentId, query.EventType, Page(query.PageNumber), Size(query.PageSize), cancellationToken));

    public async Task<Result<ContentRevisionDto>> GetLatestAsync(GetLatestContentRevisionQuery query, CancellationToken cancellationToken)
    {
        var revision = await repository.GetLatestRevisionAsync(query.ContentType, query.ContentId, cancellationToken);
        return revision is null
            ? Result<ContentRevisionDto>.NotFound(nameof(query.ContentId), "Content revision was not found.")
            : Result<ContentRevisionDto>.Success(revision);
    }

    private static int Page(int? value) => Math.Max(value ?? 1, 1);
    private static int Size(int? value) => Math.Clamp(value ?? 20, 1, 100);
}
