namespace EnglishMaster.Contracts.LessonSections;

public sealed record CreateLessonSectionRequest(
    string Title,
    string? ContentMarkdown,
    string SectionType,
    Guid? MediaId,
    int SortOrder);
