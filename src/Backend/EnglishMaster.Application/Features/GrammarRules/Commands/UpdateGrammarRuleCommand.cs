namespace EnglishMaster.Application.Features.GrammarRules.Commands;

public sealed record UpdateGrammarRuleCommand(
    Guid Id,
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
