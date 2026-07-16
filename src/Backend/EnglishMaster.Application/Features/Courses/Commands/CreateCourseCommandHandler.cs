using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed class CreateCourseCommandHandler
{
    private readonly ICourseRepository courseRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public CreateCourseCommandHandler(
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
        CreateCourseCommand command,
        CancellationToken cancellationToken)
    {
        var validation = CourseInputValidator.Validate(
            command.Title,
            command.Summary,
            command.Description,
            command.CefrLevel,
            command.CategoryId,
            command.ThumbnailMediaId,
            command.EstimatedMinutes,
            command.SortOrder,
            isPublished: false,
            isActive: true);

        if (!validation.IsSuccess)
        {
            return Result<CourseDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var referenceValidation = await CourseReferenceValidator.ValidateReferencesAsync(
            categoryRepository,
            mediaRepository,
            input,
            cancellationToken);
        if (referenceValidation.Errors.Count > 0)
        {
            return Result<CourseDto>.Validation([.. referenceValidation.Errors]);
        }

        if (await courseRepository.SlugExistsAsync(input.Slug, null, cancellationToken))
        {
            return Result<CourseDto>.Validation(
                new ValidationError(nameof(command.Title), "A course with this title already exists."));
        }

        var course = Course.Create(
            input.Title,
            input.Summary,
            input.Description,
            input.CefrLevel,
            input.CategoryId,
            input.ThumbnailMediaId,
            input.EstimatedMinutes,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await courseRepository.AddAsync(course, cancellationToken);
        await courseRepository.SaveChangesAsync(cancellationToken);

        return Result<CourseDto>.Success(await CourseReadModelBuilder.MapAsync(
            course,
            categoryRepository,
            mediaRepository,
            lessonRepository,
            cancellationToken));
    }
}
