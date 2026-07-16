namespace EnglishMaster.Application.Features.LessonSections.Commands;

public sealed record UpdateLessonSectionCommand(
    Guid Id,
    string Title,
    string? ContentMarkdown,
    string SectionType,
    Guid? MediaId,
    int SortOrder,
    bool IsActive);
