namespace EnglishMaster.Application.Features.LessonSections.Commands;

public sealed record ReorderLessonSectionsCommand(
    Guid LessonId,
    IReadOnlyList<Guid> OrderedSectionIds);
