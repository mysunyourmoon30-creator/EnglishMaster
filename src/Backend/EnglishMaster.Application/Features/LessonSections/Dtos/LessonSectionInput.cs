using EnglishMaster.Domain.Lessons;

namespace EnglishMaster.Application.Features.LessonSections.Dtos;

internal sealed record LessonSectionInput(
    string Title,
    string ContentMarkdown,
    SectionType SectionType,
    Guid? MediaId,
    int SortOrder,
    bool IsActive);
