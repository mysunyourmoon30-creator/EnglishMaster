namespace EnglishMaster.Application.Features.GrammarRules.Queries;

public sealed record SearchGrammarRulesQuery(
    string? Search,
    Guid? GrammarTopicId,
    bool? IsActive,
    int? PageNumber = null,
    int? PageSize = null);
