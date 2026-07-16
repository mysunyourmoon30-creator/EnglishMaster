namespace EnglishMaster.Application.Features.LessonSections.Commands;

public sealed record AddLessonSectionCommand(
    Guid LessonId,
    string Title,
    string? ContentMarkdown,
    string SectionType,
    Guid? MediaId,
    int SortOrder);
