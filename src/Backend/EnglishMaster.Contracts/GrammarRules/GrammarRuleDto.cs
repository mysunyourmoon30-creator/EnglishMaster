namespace EnglishMaster.Contracts.GrammarRules;

public sealed record GrammarRuleDto(
    Guid Id,
    Guid GrammarTopicId,
    GrammarRuleTopicDto? Topic,
    string Title,
    string Slug,
    string RuleText,
    string ExplanationTh,
    string ExplanationEn,
    string StructurePattern,
    string CommonMistake,
    string CorrectUsageNote,
    int SortOrder,
    IReadOnlyCollection<GrammarRuleRelatedWordDto> RelatedWords,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record GrammarRuleTopicDto(
    Guid Id,
    string Title,
    string Slug,
    string CefrLevel);

public sealed record GrammarRuleRelatedWordDto(
    Guid Id,
    string Text,
    string Slug,
    string MeaningTh);
