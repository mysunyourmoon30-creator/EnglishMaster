namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed record ReorderBookChapterLessonsCommand(
    Guid BookChapterId,
    IReadOnlyList<Guid> OrderedBookChapterLessonIds);
