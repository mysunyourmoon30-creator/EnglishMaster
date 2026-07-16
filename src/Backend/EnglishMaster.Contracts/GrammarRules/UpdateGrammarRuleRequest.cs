namespace EnglishMaster.Contracts.GrammarRules;

public sealed record UpdateGrammarRuleRequest(
    Guid GrammarTopicId,
    string Title,
    string RuleText,
    string? ExplanationTh,
    string? ExplanationEn,
    string? StructurePattern,
    string? CommonMistake,
    string? CorrectUsageNote,
    int SortOrder,
    bool IsActive);
