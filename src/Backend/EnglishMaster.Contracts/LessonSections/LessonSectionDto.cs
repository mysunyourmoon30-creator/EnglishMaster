using EnglishMaster.Contracts.Lessons;

namespace EnglishMaster.Contracts.LessonSections;

public sealed record LessonSectionDto(
    Guid Id,
    Guid LessonId,
    string Title,
    string ContentMarkdown,
    string SectionType,
    Guid? MediaId,
    LessonMediaDto? Media,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
