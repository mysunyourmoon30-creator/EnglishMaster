namespace EnglishMaster.Contracts.QuizChoices;

public sealed record ReorderQuizChoicesRequest(
    IReadOnlyList<Guid> OrderedChoiceIds);
