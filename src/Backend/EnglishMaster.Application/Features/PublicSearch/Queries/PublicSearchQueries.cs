using EnglishMaster.Application.Features.PublicSearch.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.PublicSearch.Queries;

public sealed record SearchPublicContentQuery(
    string? Query,
    string? ContentType,
    string? CefrLevel,
    Guid? CategoryId,
    Guid? TagId,
    int? PageNumber,
    int? PageSize,
    string? SortBy,
    string? SortDirection);

public sealed record GetPublicSearchSuggestionsQuery(string? Query);

public sealed class PublicSearchQueryHandler
{
    private const int MaximumPageSize = 50;
    private readonly IPublicSearchRepository repository;

    public PublicSearchQueryHandler(IPublicSearchRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<PublicSearchResponse>> SearchAsync(SearchPublicContentQuery query, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(query.PageNumber ?? 1, 1);
        var pageSize = Math.Clamp(query.PageSize ?? 10, 1, MaximumPageSize);
        var criteria = new PublicSearchCriteria(query.Query, Normalize(query.ContentType), query.CefrLevel, query.CategoryId, query.TagId, pageNumber, pageSize, query.SortBy, query.SortDirection);
        return Result<PublicSearchResponse>.Success(await repository.SearchAsync(criteria, cancellationToken));
    }

    public async Task<Result<PublicSearchFiltersResponse>> GetFiltersAsync(CancellationToken cancellationToken) =>
        Result<PublicSearchFiltersResponse>.Success(await repository.GetFiltersAsync(cancellationToken));

    public async Task<Result<IReadOnlyCollection<string>>> GetSuggestionsAsync(GetPublicSearchSuggestionsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<string>>.Success(await repository.GetSuggestionsAsync(query.Query, 8, cancellationToken));

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToLowerInvariant() switch
            {
                "words" => "word",
                "grammar-topic" => "grammar",
                "grammar-topics" => "grammar",
                "grammar-rule" => "grammar",
                "grammar-rules" => "grammar",
                "lessons" => "lesson",
                "courses" => "course",
                "books" => "book",
                "quizzes" => "quiz",
                var normalized => normalized
            };
}
