namespace EnglishMaster.Application.Features.Books.Commands;

public sealed record CreateBookCommand(
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
