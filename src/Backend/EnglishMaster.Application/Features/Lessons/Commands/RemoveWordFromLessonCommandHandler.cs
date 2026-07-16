using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed class RemoveWordFromLessonCommandHandler
{
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public RemoveWordFromLessonCommandHandler(
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        RemoveWordFromLessonCommand command,
        CancellationToken cancellationToken)
    {
        if (command.LessonId == Guid.Empty)
        {
            return Result.Validation(new ValidationError(nameof(command.LessonId), $"{nameof(command.LessonId)} cannot be empty."));
        }

        if (command.WordId == Guid.Empty)
        {
            return Result.Validation(new ValidationError(nameof(command.WordId), $"{nameof(command.WordId)} cannot be empty."));
        }

        var lesson = await lessonRepository.GetByIdAsync(command.LessonId, cancellationToken);
        if (lesson is null)
        {
            return Result.NotFound(nameof(command.LessonId), "Lesson was not found.");
        }

        if (!lesson.RemoveWord(command.WordId, timeProvider.GetUtcNow()))
        {
            return Result.NotFound(nameof(command.WordId), "Related word was not found on this lesson.");
        }

        await lessonRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
