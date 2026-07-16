namespace EnglishMaster.Contracts.GrammarTopics;

public sealed record GrammarTopicSearchResponse(
    IReadOnlyCollection<GrammarTopicDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
