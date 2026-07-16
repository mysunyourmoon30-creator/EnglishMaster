namespace EnglishMaster.Contracts.BookChapters;

public sealed record ReorderBookChapterLessonsRequest(
    IReadOnlyList<Guid> OrderedBookChapterLessonIds);
