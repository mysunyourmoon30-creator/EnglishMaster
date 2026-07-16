namespace EnglishMaster.Contracts.QuizChoices;

public sealed record UpdateQuizChoiceRequest(
    string ChoiceText,
    bool IsCorrect,
    string? ExplanationTh,
    string? ExplanationEn,
    int SortOrder,
    bool IsActive);
