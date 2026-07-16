namespace EnglishMaster.Application.Features.GrammarRules.Commands;

public sealed record CreateGrammarRuleCommand(
    Guid GrammarTopicId,
    string Title,
    string RuleText,
    string? ExplanationTh,
    string? ExplanationEn,
    string? StructurePattern,
    string? CommonMistake,
    string? CorrectUsageNote,
    int SortOrder);
