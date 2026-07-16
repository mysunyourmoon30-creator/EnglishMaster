namespace EnglishMaster.Application.Features.GrammarExamples.Commands;

public sealed record UpdateGrammarExampleCommand(
    Guid Id,
    string ExampleEn,
    string? TranslationTh,
    string? ExplanationTh,
    bool IsCorrectExample,
    int SortOrder,
    bool IsActive);
