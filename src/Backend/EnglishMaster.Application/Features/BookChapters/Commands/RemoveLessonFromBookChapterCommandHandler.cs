using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed class RemoveLessonFromBookChapterCommandHandler
{
    private readonly IBookChapterRepository chapterRepository;
    private readonly TimeProvider timeProvider;

    public RemoveLessonFromBookChapterCommandHandler(
        IBookChapterRepository chapterRepository,
        TimeProvider timeProvider)
    {
        this.chapterRepository = chapterRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        RemoveLessonFromBookChapterCommand command,
        CancellationToken cancellationToken)
    {
        if (command.BookChapterId == Guid.Empty)
        {
            return Result.Validation(
                new ValidationError(nameof(command.BookChapterId), $"{nameof(command.BookChapterId)} cannot be empty."));
        }

        if (command.LessonId == Guid.Empty)
        {
            return Result.Validation(
                new ValidationError(nameof(command.LessonId), $"{nameof(command.LessonId)} cannot be empty."));
        }

        var chapter = await chapterRepository.GetByIdAsync(command.BookChapterId, cancellationToken);
        if (chapter is null)
        {
            return Result.NotFound(nameof(command.BookChapterId), "Book chapter was not found.");
        }

        var removed = chapter.RemoveLesson(command.LessonId, timeProvider.GetUtcNow());
        if (!removed)
        {
            return Result.NotFound(nameof(command.LessonId), "Lesson was not found in this book chapter.");
        }

        await chapterRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
