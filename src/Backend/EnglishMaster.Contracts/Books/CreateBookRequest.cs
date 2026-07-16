namespace EnglishMaster.Contracts.Books;

public sealed record CreateBookRequest(
    string Title,
    string? Subtitle,
    string? Summary,
    string? Description,
    string? CefrLevel,
    Guid? CategoryId,
    Guid? CoverMediaId,
    Guid? CourseId,
    string? AuthorName,
    string? Edition,
    string? Version,
    int EstimatedPages,
    int SortOrder);
