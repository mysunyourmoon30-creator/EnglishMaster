using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.Application.Features.GrammarRules.Dtos;

public sealed record GrammarRuleSearchCriteria(
    string? SearchTerm,
    Guid? GrammarTopicId,
    bool? IsActive,
    int PageNumber,
    int PageSize);

public sealed record GrammarRuleSearchResult(
    IReadOnlyCollection<GrammarRule> Items,
    int TotalCount);
