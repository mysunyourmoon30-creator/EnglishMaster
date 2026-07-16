using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed class AddLessonToCourseCommandHandler
{
    private readonly ICourseRepository courseRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public AddLessonToCourseCommandHandler(
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
        AddLessonToCourseCommand command,
        CancellationToken cancellationToken)
    {
        if (command.CourseId == Guid.Empty)
        {
            return Result<CourseDto>.Validation(new ValidationError(nameof(command.CourseId), $"{nameof(command.CourseId)} cannot be empty."));
        }

        if (command.LessonId == Guid.Empty)
        {
            return Result<CourseDto>.Validation(new ValidationError(nameof(command.LessonId), $"{nameof(command.LessonId)} cannot be empty."));
        }

        if (command.SortOrder < 0)
        {
            return Result<CourseDto>.Validation(new ValidationError(nameof(command.SortOrder), $"{nameof(command.SortOrder)} must be greater than or equal to zero."));
        }

        var course = await courseRepository.GetByIdAsync(command.CourseId, cancellationToken);
        if (course is null)
        {
            return Result<CourseDto>.NotFound(nameof(command.CourseId), "Course was not found.");
        }

        if (!course.IsActive)
        {
            return Result<CourseDto>.Validation(new ValidationError(nameof(command.CourseId), "Course is inactive."));
        }

        var lessonErrors = await CourseReferenceValidator.ValidateLessonAsync(
            lessonRepository,
            command.LessonId,
            cancellationToken);
        if (lessonErrors.Count > 0)
        {
            return Result<CourseDto>.Validation([.. lessonErrors]);
        }

        course.AddLesson(command.LessonId, command.SortOrder, command.IsRequired, timeProvider.GetUtcNow());
        await courseRepository.SaveChangesAsync(cancellationToken);

        return Result<CourseDto>.Success(await CourseReadModelBuilder.MapAsync(
            course,
            categoryRepository,
            mediaRepository,
            lessonRepository,
            cancellationToken));
    }
}
