namespace EnglishMaster.Contracts.BookChapters;

public sealed record BookChapterDto(
    Guid Id,
    Guid BookId,
    string Title,
    string Slug,
    string Summary,
    string ContentMarkdown,
    int SortOrder,
    IReadOnlyCollection<BookChapterLessonDto> Lessons,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record BookChapterLessonDto(
    Guid Id,
    Guid BookChapterId,
    Guid LessonId,
    string LessonTitle,
    string LessonSlug,
    string LessonSummary,
    string? LessonCefrLevel,
    int SortOrder,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
