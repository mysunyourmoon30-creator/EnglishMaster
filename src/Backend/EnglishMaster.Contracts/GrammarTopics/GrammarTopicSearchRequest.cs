namespace EnglishMaster.Contracts.GrammarTopics;

public sealed record GrammarTopicSearchRequest(
    string? Search = null,
    string? CefrLevel = null,
    bool? IsActive = true,
    int PageNumber = 1,
    int PageSize = 20);
