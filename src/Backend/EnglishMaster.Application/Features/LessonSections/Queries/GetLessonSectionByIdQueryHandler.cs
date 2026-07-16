using EnglishMaster.Application.Features.LessonSections.Dtos;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.LessonSections;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.LessonSections.Queries;

public sealed class GetLessonSectionByIdQueryHandler
{
    private readonly ILessonSectionRepository lessonSectionRepository;
    private readonly IMediaRepository mediaRepository;

    public GetLessonSectionByIdQueryHandler(
        ILessonSectionRepository lessonSectionRepository,
        IMediaRepository mediaRepository)
    {
        this.lessonSectionRepository = lessonSectionRepository;
        this.mediaRepository = mediaRepository;
    }

    public async Task<Result<LessonSectionDto>> HandleAsync(
        GetLessonSectionByIdQuery query,
        CancellationToken cancellationToken)
    {
        var lessonSection = await lessonSectionRepository.GetByIdAsync(query.Id, cancellationToken);
        if (lessonSection is null)
        {
            return Result<LessonSectionDto>.NotFound(nameof(query.Id), "Lesson section was not found.");
        }

        var media = lessonSection.MediaId.HasValue
            ? await mediaRepository.GetByIdAsync(lessonSection.MediaId.Value, cancellationToken)
            : null;

        return Result<LessonSectionDto>.Success(LessonSectionMapper.ToDto(lessonSection, media));
    }
}
