namespace EnglishMaster.Contracts.QuizQuestions;

public sealed record UpdateQuizQuestionRequest(
    string QuestionText,
    string QuestionType,
    string? ExplanationTh,
    string? ExplanationEn,
    int Points,
    int SortOrder,
    Guid? WordId,
    Guid? GrammarRuleId,
    Guid? PronunciationId,
    bool IsActive);
