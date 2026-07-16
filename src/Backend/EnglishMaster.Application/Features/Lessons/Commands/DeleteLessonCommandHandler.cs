using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed class DeleteLessonCommandHandler
{
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public DeleteLessonCommandHandler(
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteLessonCommand command,
        CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(command.Id, cancellationToken);
        if (lesson is null)
        {
            return Result.NotFound(nameof(command.Id), "Lesson was not found.");
        }

        lesson.Deactivate(timeProvider.GetUtcNow());
        await lessonRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
