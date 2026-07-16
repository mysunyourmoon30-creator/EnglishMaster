namespace EnglishMaster.Contracts.GrammarExamples;

public sealed record UpdateGrammarExampleRequest(
    string ExampleEn,
    string? TranslationTh,
    string? ExplanationTh,
    bool IsCorrectExample,
    int SortOrder,
    bool IsActive);
