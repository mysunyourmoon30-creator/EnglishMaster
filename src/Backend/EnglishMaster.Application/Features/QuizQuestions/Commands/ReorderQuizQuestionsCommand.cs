namespace EnglishMaster.Application.Features.QuizQuestions.Commands;

public sealed record ReorderQuizQuestionsCommand(Guid QuizId, IReadOnlyList<Guid> OrderedQuestionIds);
