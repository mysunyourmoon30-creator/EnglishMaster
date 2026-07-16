using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed class DeleteCourseCommandHandler
{
    private readonly ICourseRepository courseRepository;
    private readonly TimeProvider timeProvider;

    public DeleteCourseCommandHandler(
        ICourseRepository courseRepository,
        TimeProvider timeProvider)
    {
        this.courseRepository = courseRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteCourseCommand command,
        CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(command.Id, cancellationToken);
        if (course is null)
        {
            return Result.NotFound(nameof(command.Id), "Course was not found.");
        }

        course.Deactivate(timeProvider.GetUtcNow());
        await courseRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
