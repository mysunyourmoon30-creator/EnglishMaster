namespace EnglishMaster.Contracts.QuizChoices;

public sealed record QuizChoiceDto(
    Guid Id,
    Guid QuizQuestionId,
    string ChoiceText,
    bool IsCorrect,
    string ExplanationTh,
    string ExplanationEn,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
