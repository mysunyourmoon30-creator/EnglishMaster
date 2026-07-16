namespace EnglishMaster.Contracts.LessonSections;

public sealed record UpdateLessonSectionRequest(
    string Title,
    string? ContentMarkdown,
    string SectionType,
    Guid? MediaId,
    int SortOrder,
    bool IsActive);
