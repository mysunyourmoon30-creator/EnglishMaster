namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed record AddBookChapterCommand(
    Guid BookId,
    string Title,
    string? Summary,
    string? ContentMarkdown,
    int SortOrder);
