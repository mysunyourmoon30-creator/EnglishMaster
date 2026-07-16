using EnglishMaster.Application.Features.LessonSections.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.LessonSections;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Shared.Results;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.LessonSections.Commands;

public sealed class AddLessonSectionCommandHandler
{
    private readonly ILessonSectionRepository lessonSectionRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public AddLessonSectionCommandHandler(
        ILessonSectionRepository lessonSectionRepository,
        ILessonRepository lessonRepository,
        IMediaRepository mediaRepository,
        TimeProvider timeProvider)
    {
        this.lessonSectionRepository = lessonSectionRepository;
        this.lessonRepository = lessonRepository;
        this.mediaRepository = mediaRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<LessonSectionDto>> HandleAsync(
        AddLessonSectionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.LessonId == Guid.Empty)
        {
            return Result<LessonSectionDto>.Validation(
                new ValidationError(nameof(command.LessonId), $"{nameof(command.LessonId)} cannot be empty."));
        }

        var lesson = await lessonRepository.GetByIdAsync(command.LessonId, cancellationToken);
        if (lesson is null)
        {
            return Result<LessonSectionDto>.NotFound(nameof(command.LessonId), "Lesson was not found.");
        }

        if (!lesson.IsActive)
        {
            return Result<LessonSectionDto>.Validation(
                new ValidationError(nameof(command.LessonId), "Lesson is inactive."));
        }

        var validation = LessonSectionInputValidator.Validate(
            command.Title,
            command.ContentMarkdown,
            command.SectionType,
            command.MediaId,
            command.SortOrder,
            isActive: true);
        if (!validation.IsSuccess)
        {
            return Result<LessonSectionDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        MediaEntity? media = null;
        if (input.MediaId.HasValue)
        {
            var mediaErrors = await LessonSectionReferenceValidator.ValidateMediaAsync(
                mediaRepository,
                input.MediaId.Value,
                cancellationToken);
            if (mediaErrors.Count > 0)
            {
                return Result<LessonSectionDto>.Validation([.. mediaErrors]);
            }

            media = await mediaRepository.GetByIdAsync(input.MediaId.Value, cancellationToken);
        }

        var lessonSection = LessonSection.Create(
            command.LessonId,
            input.Title,
            input.ContentMarkdown,
            input.SectionType,
            input.MediaId,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await lessonSectionRepository.AddAsync(lessonSection, cancellationToken);
        await lessonSectionRepository.SaveChangesAsync(cancellationToken);

        return Result<LessonSectionDto>.Success(LessonSectionMapper.ToDto(lessonSection, media));
    }
}
