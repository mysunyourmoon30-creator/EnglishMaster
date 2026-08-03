namespace EnglishMaster.Contracts.Diagnostics;

public sealed record DiagnosticChoiceDto(Guid Id, string Text, int SortOrder);

public sealed record DiagnosticQuestionDto(
    Guid Id,
    string Text,
    string QuestionType,
    int SortOrder,
    IReadOnlyCollection<DiagnosticChoiceDto> Choices);

public sealed record DiagnosticQuizDto(
    Guid Id,
    string Title,
    string Summary,
    int TimeLimitMinutes,
    int PassingScore,
    string? CefrLevel,
    IReadOnlyCollection<DiagnosticQuestionDto> Questions);

public sealed record DiagnosticAnswerRequest(Guid QuestionId, Guid ChoiceId);

public sealed record SubmitDiagnosticRequest(IReadOnlyCollection<DiagnosticAnswerRequest>? Answers);

public sealed record DiagnosticResultDto(
    Guid QuizId,
    string QuizTitle,
    int CorrectCount,
    int TotalQuestions,
    int PercentageScore,
    int PassingScore,
    bool Passed,
    string? AssessedCefrLevel,
    string Recommendation);
