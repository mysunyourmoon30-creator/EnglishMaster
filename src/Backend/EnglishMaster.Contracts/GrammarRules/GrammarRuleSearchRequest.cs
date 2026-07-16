namespace EnglishMaster.Contracts.GrammarRules;

public sealed record GrammarRuleSearchRequest(
    string? Search = null,
    Guid? GrammarTopicId = null,
    bool? IsActive = true,
    int PageNumber = 1,
    int PageSize = 20);
