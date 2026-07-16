namespace EnglishMaster.Contracts.GrammarExamples;

public sealed record CreateGrammarExampleRequest(
    string ExampleEn,
    string? TranslationTh,
    string? ExplanationTh,
    bool IsCorrectExample,
    int SortOrder);
