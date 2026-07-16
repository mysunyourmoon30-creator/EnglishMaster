using EnglishMaster.Contracts.BookChapters;

namespace EnglishMaster.Contracts.Books;

public sealed record BookDto(
    Guid Id,
    string Title,
    string Slug,
    string Subtitle,
    string Summary,
    string Description,
    string? CefrLevel,
    Guid? CategoryId,
    BookCategoryDto? Category,
    Guid? CoverMediaId,
    BookMediaDto? CoverMedia,
    Guid? CourseId,
    BookCourseDto? Course,
    string AuthorName,
    string Edition,
    string Version,
    int EstimatedPages,
    int SortOrder,
    IReadOnlyCollection<BookChapterDto> Chapters,
    bool IsPublished,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record BookCategoryDto(
    Guid Id,
    string Name,
    string Slug);

public sealed record BookMediaDto(
    Guid Id,
    string FileName,
    string ContentType,
    string MediaType,
    string PublicUrl,
    string AltText);

public sealed record BookCourseDto(
    Guid Id,
    string Title,
    string Slug);
