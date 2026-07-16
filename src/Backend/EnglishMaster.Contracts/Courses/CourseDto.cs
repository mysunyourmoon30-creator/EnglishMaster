namespace EnglishMaster.Contracts.Courses;

public sealed record CourseDto(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string Description,
    string? CefrLevel,
    Guid? CategoryId,
    CourseCategoryDto? Category,
    Guid? ThumbnailMediaId,
    CourseMediaDto? ThumbnailMedia,
    int EstimatedMinutes,
    int SortOrder,
    IReadOnlyCollection<CourseLessonDto> Lessons,
    bool IsPublished,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CourseCategoryDto(
    Guid Id,
    string Name,
    string Slug);

public sealed record CourseMediaDto(
    Guid Id,
    string FileName,
    string ContentType,
    string MediaType,
    string PublicUrl,
    string AltText);

public sealed record CourseLessonDto(
    Guid Id,
    Guid CourseId,
    Guid LessonId,
    string LessonTitle,
    string LessonSlug,
    string LessonSummary,
    string? LessonCefrLevel,
    int SortOrder,
    bool IsRequired,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
