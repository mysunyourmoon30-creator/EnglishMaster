namespace EnglishMaster.Contracts.LessonSections;

public sealed record ReorderLessonSectionsRequest(
    IReadOnlyList<Guid> OrderedSectionIds);
