namespace EnglishMaster.Contracts.QuizChoices;

public sealed record CreateQuizChoiceRequest(
    string ChoiceText,
    bool IsCorrect,
    string? ExplanationTh,
    string? ExplanationEn,
    int SortOrder);
