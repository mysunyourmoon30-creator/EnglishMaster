namespace EnglishMaster.Application.Features.QuizChoices.Commands;

public sealed record ReorderQuizChoicesCommand(Guid QuizQuestionId, IReadOnlyList<Guid> OrderedChoiceIds);
