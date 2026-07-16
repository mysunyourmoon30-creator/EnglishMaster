using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed class UnpublishCourseCommandHandler
{
    private readonly ICourseRepository courseRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public UnpublishCourseCommandHandler(
        ICourseRepository courseRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.courseRepository = courseRepository;
        this.categoryRepository = categoryRepository;
        this.mediaRepository = mediaRepository;
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<CourseDto>> HandleAsync(
        UnpublishCourseCommand command,
        CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(command.Id, cancellationToken);
        if (course is null)
        {
            return Result<CourseDto>.NotFound(nameof(command.Id), "Course was not found.");
        }

        course.Unpublish(timeProvider.GetUtcNow());
        await courseRepository.SaveChangesAsync(cancellationToken);

        return Result<CourseDto>.Success(await CourseReadModelBuilder.MapAsync(
            course,
            categoryRepository,
            mediaRepository,
            lessonRepository,
            cancellationToken));
    }
}
