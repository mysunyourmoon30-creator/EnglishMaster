namespace EnglishMaster.Application.Features.GrammarExamples.Dtos;

internal sealed record GrammarExampleInput(
    string ExampleEn,
    string TranslationTh,
    string ExplanationTh,
    bool IsCorrectExample,
    int SortOrder,
    bool IsActive);
