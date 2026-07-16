namespace EnglishMaster.Application.Features.PublicSearch.Dtos;

public sealed record PublicSearchCriteria(
    string? Query,
    string? ContentType,
    string? CefrLevel,
    Guid? CategoryId,
    Guid? TagId,
    int PageNumber,
    int PageSize,
    string? SortBy,
    string? SortDirection);

public sealed record PublicSearchResultDto(
    string ContentType,
    string Title,
    string Slug,
    string Summary,
    string? CefrLevel,
    string? CategoryName,
    IReadOnlyCollection<string> Tags,
    string Url,
    string HighlightText,
    DateTimeOffset UpdatedAt);

public sealed record PublicSearchResponse(
    IReadOnlyCollection<PublicSearchResultDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record PublicSearchFilterDto(string Key, string Label);

public sealed record PublicSearchFiltersResponse(
    IReadOnlyCollection<PublicSearchFilterDto> ContentTypes,
    IReadOnlyCollection<PublicSearchFilterDto> CefrLevels,
    IReadOnlyCollection<PublicSearchFilterDto> Categories,
    IReadOnlyCollection<PublicSearchFilterDto> Tags);

