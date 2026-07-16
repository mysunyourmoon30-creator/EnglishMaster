namespace EnglishMaster.Application.Features.GrammarExamples.Commands;

public sealed record AddGrammarExampleCommand(
    Guid GrammarRuleId,
    string ExampleEn,
    string? TranslationTh,
    string? ExplanationTh,
    bool IsCorrectExample,
    int SortOrder);
