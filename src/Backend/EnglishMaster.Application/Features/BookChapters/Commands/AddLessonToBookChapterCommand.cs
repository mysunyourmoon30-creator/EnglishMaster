namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed record AddLessonToBookChapterCommand(
    Guid BookChapterId,
    Guid LessonId,
    int SortOrder);
