namespace EnglishMaster.Contracts.BookChapters;

public sealed record CreateBookChapterRequest(
    string Title,
    string? Summary,
    string? ContentMarkdown,
    int SortOrder);
