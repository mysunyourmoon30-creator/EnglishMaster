namespace EnglishMaster.Contracts.GrammarExamples;

public sealed record GrammarExampleDto(
    Guid Id,
    Guid GrammarRuleId,
    string ExampleEn,
    string TranslationTh,
    string ExplanationTh,
    bool IsCorrectExample,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
