namespace EnglishMaster.Application.Features.GrammarRules.Queries;

public sealed record GetGrammarRulesByTopicIdQuery(
    Guid GrammarTopicId,
    bool? IsActive = true,
    int? PageNumber = null,
    int? PageSize = null);
