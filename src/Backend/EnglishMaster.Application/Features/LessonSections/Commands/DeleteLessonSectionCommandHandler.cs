using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.LessonSections.Commands;

public sealed class DeleteLessonSectionCommandHandler
{
    private readonly ILessonSectionRepository lessonSectionRepository;
    private readonly TimeProvider timeProvider;

    public DeleteLessonSectionCommandHandler(
        ILessonSectionRepository lessonSectionRepository,
        TimeProvider timeProvider)
    {
        this.lessonSectionRepository = lessonSectionRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteLessonSectionCommand command,
        CancellationToken cancellationToken)
    {
        var lessonSection = await lessonSectionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (lessonSection is null)
        {
            return Result.NotFound(nameof(command.Id), "Lesson section was not found.");
        }

        lessonSection.Deactivate(timeProvider.GetUtcNow());
        await lessonSectionRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
