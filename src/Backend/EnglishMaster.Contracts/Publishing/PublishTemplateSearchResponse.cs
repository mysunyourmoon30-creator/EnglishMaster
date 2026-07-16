namespace EnglishMaster.Contracts.Publishing;

public sealed record PublishTemplateSearchResponse(
    IReadOnlyCollection<PublishTemplateDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
