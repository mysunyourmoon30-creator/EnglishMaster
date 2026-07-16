namespace EnglishMaster.Contracts.BookChapters;

public sealed record UpdateBookChapterRequest(
    string Title,
    string? Summary,
    string? ContentMarkdown,
    int SortOrder,
    bool IsActive);
