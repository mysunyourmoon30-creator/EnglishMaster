using EnglishMaster.Application.Features.LessonSections.Dtos;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.LessonSections;
using EnglishMaster.Shared.Results;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.LessonSections.Commands;

public sealed class UpdateLessonSectionCommandHandler
{
    private readonly ILessonSectionRepository lessonSectionRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public UpdateLessonSectionCommandHandler(
        ILessonSectionRepository lessonSectionRepository,
        IMediaRepository mediaRepository,
        TimeProvider timeProvider)
    {
        this.lessonSectionRepository = lessonSectionRepository;
        this.mediaRepository = mediaRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<LessonSectionDto>> HandleAsync(
        UpdateLessonSectionCommand command,
        CancellationToken cancellationToken)
    {
        var lessonSection = await lessonSectionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (lessonSection is null)
        {
            return Result<LessonSectionDto>.NotFound(nameof(command.Id), "Lesson section was not found.");
        }

        var validation = LessonSectionInputValidator.Validate(
            command.Title,
            command.ContentMarkdown,
            command.SectionType,
            command.MediaId,
            command.SortOrder,
            command.IsActive);
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

        lessonSection.Update(
            input.Title,
            input.ContentMarkdown,
            input.SectionType,
            input.MediaId,
            input.SortOrder,
            input.IsActive,
            timeProvider.GetUtcNow());

        await lessonSectionRepository.SaveChangesAsync(cancellationToken);

        return Result<LessonSectionDto>.Success(LessonSectionMapper.ToDto(lessonSection, media));
    }
}
