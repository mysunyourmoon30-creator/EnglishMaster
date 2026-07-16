namespace EnglishMaster.Application.Features.BookChapters.Dtos;

internal sealed record BookChapterInput(
    string Title,
    string Slug,
    string Summary,
    string ContentMarkdown,
    int SortOrder,
    bool IsActive);
