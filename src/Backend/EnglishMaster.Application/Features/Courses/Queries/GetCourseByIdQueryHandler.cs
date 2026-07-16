using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Courses.Queries;

public sealed class GetCourseByIdQueryHandler
{
    private readonly ICourseRepository courseRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly ILessonRepository lessonRepository;

    public GetCourseByIdQueryHandler(
        ICourseRepository courseRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ILessonRepository lessonRepository)
    {
        this.courseRepository = courseRepository;
        this.categoryRepository = categoryRepository;
        this.mediaRepository = mediaRepository;
        this.lessonRepository = lessonRepository;
    }

    public async Task<Result<CourseDto>> HandleAsync(
        GetCourseByIdQuery query,
        CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(query.Id, cancellationToken);
        if (course is null)
        {
            return Result<CourseDto>.NotFound(nameof(query.Id), "Course was not found.");
        }

        return Result<CourseDto>.Success(await CourseReadModelBuilder.MapAsync(
            course,
            categoryRepository,
            mediaRepository,
            lessonRepository,
            cancellationToken));
    }
}
