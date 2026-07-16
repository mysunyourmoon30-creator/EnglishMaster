using EnglishMaster.Application.Features.PublicSearch.Queries;
using EnglishMaster.Contracts.PublicSearch;
using EnglishMaster.Shared.Results;
using AppPublicSearchFiltersResponse = EnglishMaster.Application.Features.PublicSearch.Dtos.PublicSearchFiltersResponse;
using AppPublicSearchResponse = EnglishMaster.Application.Features.PublicSearch.Dtos.PublicSearchResponse;
using AppPublicSearchResultDto = EnglishMaster.Application.Features.PublicSearch.Dtos.PublicSearchResultDto;

namespace EnglishMaster.Api.Endpoints;

public static class PublicSearchEndpoints
{
    public static IEndpointRouteBuilder MapPublicSearchEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/public/search")
            .WithTags("Public Search")
            .AllowAnonymous();

        group.MapGet("", SearchAsync);
        group.MapGet("/filters", GetFiltersAsync);
        group.MapGet("/suggestions", GetSuggestionsAsync);

        return endpoints;
    }

    private static async Task<IResult> SearchAsync(PublicSearchQueryHandler handler, string? q, string? contentType, string? type, string? cefrLevel, string? cefr, Guid? categoryId, Guid? tagId, int? pageNumber, int? pageSize, string? sortBy, string? sortDirection, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchPublicContentQuery(q, FirstNonEmpty(contentType, type), FirstNonEmpty(cefrLevel, cefr), categoryId, tagId, pageNumber, pageSize, sortBy, sortDirection), cancellationToken));

    private static async Task<IResult> GetFiltersAsync(PublicSearchQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetFiltersAsync(cancellationToken));

    private static async Task<IResult> GetSuggestionsAsync(PublicSearchQueryHandler handler, string? q, CancellationToken cancellationToken)
    {
        var result = await handler.GetSuggestionsAsync(new GetPublicSearchSuggestionsQuery(q), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Ok(new PublicSearchSuggestionsResponse(result.Value!))
            : Results.Problem();
    }

    private static IResult ToHttpResult(Result<AppPublicSearchResponse> result) =>
        result.Status == ResultStatus.Success
            ? Results.Ok(new PublicSearchResponse(result.Value!.Items.Select(ToContract).ToArray(), result.Value.PageNumber, result.Value.PageSize, result.Value.TotalCount, result.Value.TotalPages, result.Value.HasPreviousPage, result.Value.HasNextPage))
            : Results.Problem();

    private static IResult ToHttpResult(Result<AppPublicSearchFiltersResponse> result) =>
        result.Status == ResultStatus.Success
            ? Results.Ok(new PublicSearchFiltersResponse(
                result.Value!.ContentTypes.Select(filter => new PublicSearchFilterDto(filter.Key, filter.Label)).ToArray(),
                result.Value.CefrLevels.Select(filter => new PublicSearchFilterDto(filter.Key, filter.Label)).ToArray(),
                result.Value.Categories.Select(filter => new PublicSearchFilterDto(filter.Key, filter.Label)).ToArray(),
                result.Value.Tags.Select(filter => new PublicSearchFilterDto(filter.Key, filter.Label)).ToArray()))
            : Results.Problem();

    private static PublicSearchResultDto ToContract(AppPublicSearchResultDto result) =>
        new(result.ContentType, result.Title, result.Slug, result.Summary, result.CefrLevel, result.CategoryName, result.Tags, result.Url, result.HighlightText, result.UpdatedAt);

    private static string? FirstNonEmpty(string? first, string? second) =>
        !string.IsNullOrWhiteSpace(first) ? first : second;
}
