namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed record UpdateBookChapterCommand(
    Guid Id,
    string Title,
    string? Summary,
    string? ContentMarkdown,
    int SortOrder,
    bool IsActive);
