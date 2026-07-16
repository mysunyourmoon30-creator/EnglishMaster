using EnglishMaster.Application.Features.Courses.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed class ReorderCourseLessonsCommandHandler
{
    private readonly ICourseRepository courseRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public ReorderCourseLessonsCommandHandler(
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.courseRepository = courseRepository;
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<IReadOnlyCollection<CourseLessonDto>>> HandleAsync(
        ReorderCourseLessonsCommand command,
        CancellationToken cancellationToken)
    {
        if (command.CourseId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<CourseLessonDto>>.Validation(
                new ValidationError(nameof(command.CourseId), $"{nameof(command.CourseId)} cannot be empty."));
        }

        if (command.OrderedCourseLessonIds is null)
        {
            return Result<IReadOnlyCollection<CourseLessonDto>>.Validation(
                new ValidationError(nameof(command.OrderedCourseLessonIds), $"{nameof(command.OrderedCourseLessonIds)} is required."));
        }

        if (command.OrderedCourseLessonIds.Any(id => id == Guid.Empty))
        {
            return Result<IReadOnlyCollection<CourseLessonDto>>.Validation(
                new ValidationError(nameof(command.OrderedCourseLessonIds), $"{nameof(command.OrderedCourseLessonIds)} cannot contain empty course lesson ids."));
        }

        if (command.OrderedCourseLessonIds.Count != command.OrderedCourseLessonIds.Distinct().Count())
        {
            return Result<IReadOnlyCollection<CourseLessonDto>>.Validation(
                new ValidationError(nameof(command.OrderedCourseLessonIds), $"{nameof(command.OrderedCourseLessonIds)} cannot contain duplicate course lesson ids."));
        }

        var course = await courseRepository.GetByIdAsync(command.CourseId, cancellationToken);
        if (course is null)
        {
            return Result<IReadOnlyCollection<CourseLessonDto>>.NotFound(nameof(command.CourseId), "Course was not found.");
        }

        var existingIds = course.Lessons.Select(relation => relation.Id).ToHashSet();
        var providedIds = command.OrderedCourseLessonIds.ToHashSet();
        if (course.Lessons.Count != command.OrderedCourseLessonIds.Count || !existingIds.SetEquals(providedIds))
        {
            return Result<IReadOnlyCollection<CourseLessonDto>>.Validation(
                new ValidationError(
                    nameof(command.OrderedCourseLessonIds),
                    "orderedCourseLessonIds must contain exactly the current lessons of this course."));
        }

        var relationById = course.Lessons.ToDictionary(relation => relation.Id);
        var now = timeProvider.GetUtcNow();
        for (var index = 0; index < command.OrderedCourseLessonIds.Count; index++)
        {
            relationById[command.OrderedCourseLessonIds[index]].Reorder(index, now);
        }

        await courseRepository.SaveChangesAsync(cancellationToken);

        var ordered = command.OrderedCourseLessonIds
            .Select(id => relationById[id])
            .ToArray();
        var items = await CourseReadModelBuilder.MapLessonsAsync(ordered, lessonRepository, cancellationToken);

        return Result<IReadOnlyCollection<CourseLessonDto>>.Success(items);
    }
}
