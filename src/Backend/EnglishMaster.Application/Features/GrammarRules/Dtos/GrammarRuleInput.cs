namespace EnglishMaster.Application.Features.GrammarRules.Dtos;

internal sealed record GrammarRuleInput(
    Guid GrammarTopicId,
    string Title,
    string Slug,
    string RuleText,
    string ExplanationTh,
    string ExplanationEn,
    string StructurePattern,
    string CommonMistake,
    string CorrectUsageNote,
    int SortOrder,
    bool IsActive);
