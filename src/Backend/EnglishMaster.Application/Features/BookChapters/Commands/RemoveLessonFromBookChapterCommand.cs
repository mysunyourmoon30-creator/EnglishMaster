namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed record RemoveLessonFromBookChapterCommand(
    Guid BookChapterId,
    Guid LessonId);
