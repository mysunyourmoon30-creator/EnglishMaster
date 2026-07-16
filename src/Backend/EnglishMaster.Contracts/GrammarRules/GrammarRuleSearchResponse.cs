namespace EnglishMaster.Contracts.GrammarRules;

public sealed record GrammarRuleSearchResponse(
    IReadOnlyCollection<GrammarRuleDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
