namespace EnglishMaster.Contracts.Quizzes;

public sealed record QuizSearchResponse(
    IReadOnlyCollection<QuizDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
