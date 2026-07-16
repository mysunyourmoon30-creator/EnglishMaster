using EnglishMaster.Application.Features.ContentQuality.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.ContentQuality.Queries;

public sealed record SearchContentQualityRulesQuery(string? ContentType, string? Severity, bool? IsActive, int? PageNumber, int? PageSize);
public sealed record SearchContentQualityChecksQuery(string? ContentType, string? Status, int? PageNumber, int? PageSize);
public sealed record GetContentQualityCheckByIdQuery(Guid Id);
public sealed record GetLatestContentQualityCheckQuery(string ContentType, Guid ContentId);
public sealed record GetContentQualityFindingsQuery(Guid CheckId, bool? IsResolved);

public sealed class ContentQualityQueryHandler
{
    private readonly IContentQualityRepository repository;

    public ContentQualityQueryHandler(IContentQualityRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<ContentQualityRuleSearchResponse>> SearchRulesAsync(SearchContentQualityRulesQuery query, CancellationToken cancellationToken) =>
        Result<ContentQualityRuleSearchResponse>.Success(await repository.SearchRulesAsync(query.ContentType, query.Severity, query.IsActive, Page(query.PageNumber), Size(query.PageSize), cancellationToken));

    public async Task<Result<ContentQualityCheckSearchResponse>> SearchChecksAsync(SearchContentQualityChecksQuery query, CancellationToken cancellationToken) =>
        Result<ContentQualityCheckSearchResponse>.Success(await repository.SearchChecksAsync(query.ContentType, query.Status, Page(query.PageNumber), Size(query.PageSize), cancellationToken));

    public async Task<Result<ContentQualityCheckDto>> GetCheckAsync(GetContentQualityCheckByIdQuery query, CancellationToken cancellationToken)
    {
        var check = await repository.GetCheckAsync(query.Id, cancellationToken);
        return check is null
            ? Result<ContentQualityCheckDto>.NotFound(nameof(query.Id), "Quality check was not found.")
            : Result<ContentQualityCheckDto>.Success(check);
    }

    public async Task<Result<ContentQualityCheckDto>> GetLatestCheckAsync(GetLatestContentQualityCheckQuery query, CancellationToken cancellationToken)
    {
        var check = await repository.GetLatestCheckAsync(query.ContentType, query.ContentId, cancellationToken);
        return check is null
            ? Result<ContentQualityCheckDto>.NotFound(nameof(query.ContentId), "Quality check was not found.")
            : Result<ContentQualityCheckDto>.Success(check);
    }

    public async Task<Result<IReadOnlyCollection<ContentQualityFindingDto>>> GetFindingsAsync(GetContentQualityFindingsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ContentQualityFindingDto>>.Success(await repository.GetFindingsAsync(query.CheckId, query.IsResolved, cancellationToken));

    public async Task<Result<ContentQualityDashboardDto>> GetDashboardAsync(CancellationToken cancellationToken) =>
        Result<ContentQualityDashboardDto>.Success(await repository.GetDashboardAsync(cancellationToken));

    private static int Page(int? value) => Math.Max(value ?? 1, 1);

    private static int Size(int? value) => Math.Clamp(value ?? 20, 1, 100);
}
