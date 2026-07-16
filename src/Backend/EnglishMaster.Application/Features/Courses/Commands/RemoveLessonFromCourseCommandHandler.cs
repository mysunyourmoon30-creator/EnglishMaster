using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed class RemoveLessonFromCourseCommandHandler
{
    private readonly ICourseRepository courseRepository;
    private readonly TimeProvider timeProvider;

    public RemoveLessonFromCourseCommandHandler(
        ICourseRepository courseRepository,
        TimeProvider timeProvider)
    {
        this.courseRepository = courseRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        RemoveLessonFromCourseCommand command,
        CancellationToken cancellationToken)
    {
        if (command.CourseId == Guid.Empty)
        {
            return Result.Validation(new ValidationError(nameof(command.CourseId), $"{nameof(command.CourseId)} cannot be empty."));
        }

        if (command.LessonId == Guid.Empty)
        {
            return Result.Validation(new ValidationError(nameof(command.LessonId), $"{nameof(command.LessonId)} cannot be empty."));
        }

        var course = await courseRepository.GetByIdAsync(command.CourseId, cancellationToken);
        if (course is null)
        {
            return Result.NotFound(nameof(command.CourseId), "Course was not found.");
        }

        if (!course.RemoveLesson(command.LessonId, timeProvider.GetUtcNow()))
        {
            return Result.NotFound(nameof(command.LessonId), "Related lesson was not found on this course.");
        }

        await courseRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
