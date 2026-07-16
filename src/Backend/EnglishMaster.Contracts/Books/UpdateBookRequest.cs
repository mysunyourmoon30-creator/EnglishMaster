namespace EnglishMaster.Contracts.Books;

public sealed record UpdateBookRequest(
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
    int SortOrder,
    bool IsPublished,
    bool IsActive);
