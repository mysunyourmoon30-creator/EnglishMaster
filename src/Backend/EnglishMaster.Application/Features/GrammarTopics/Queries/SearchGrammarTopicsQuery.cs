namespace EnglishMaster.Application.Features.GrammarTopics.Queries;

public sealed record SearchGrammarTopicsQuery(
    string? Search,
    string? CefrLevel,
    bool? IsActive,
    int? PageNumber = null,
    int? PageSize = null);
