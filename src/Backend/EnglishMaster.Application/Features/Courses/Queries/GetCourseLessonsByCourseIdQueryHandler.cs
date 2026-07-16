using EnglishMaster.Application.Features.Courses.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Courses.Queries;

public sealed class GetCourseLessonsByCourseIdQueryHandler
{
    private readonly ICourseRepository courseRepository;
    private readonly ILessonRepository lessonRepository;

    public GetCourseLessonsByCourseIdQueryHandler(
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository)
    {
        this.courseRepository = courseRepository;
        this.lessonRepository = lessonRepository;
    }

    public async Task<Result<IReadOnlyCollection<CourseLessonDto>>> HandleAsync(
        GetCourseLessonsByCourseIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.CourseId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<CourseLessonDto>>.Validation(
                new ValidationError(nameof(query.CourseId), $"{nameof(query.CourseId)} cannot be empty."));
        }

        var course = await courseRepository.GetByIdAsync(query.CourseId, cancellationToken);
        if (course is null)
        {
            return Result<IReadOnlyCollection<CourseLessonDto>>.NotFound(nameof(query.CourseId), "Course was not found.");
        }

        var items = await CourseReadModelBuilder.MapLessonsAsync(
            course.Lessons,
            lessonRepository,
            cancellationToken);

        return Result<IReadOnlyCollection<CourseLessonDto>>.Success(items);
    }
}
