namespace EnglishMaster.Contracts.QuizQuestions;

public sealed record ReorderQuizQuestionsRequest(
    IReadOnlyList<Guid> OrderedQuestionIds);
