using EnglishMaster.Application.Features.LessonSections.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.LessonSections;
using EnglishMaster.Shared.Results;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.LessonSections.Queries;

public sealed class GetLessonSectionsByLessonIdQueryHandler
{
    private readonly ILessonSectionRepository lessonSectionRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly IMediaRepository mediaRepository;

    public GetLessonSectionsByLessonIdQueryHandler(
        ILessonSectionRepository lessonSectionRepository,
        ILessonRepository lessonRepository,
        IMediaRepository mediaRepository)
    {
        this.lessonSectionRepository = lessonSectionRepository;
        this.lessonRepository = lessonRepository;
        this.mediaRepository = mediaRepository;
    }

    public async Task<Result<IReadOnlyCollection<LessonSectionDto>>> HandleAsync(
        GetLessonSectionsByLessonIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.LessonId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<LessonSectionDto>>.Validation(
                new ValidationError(nameof(query.LessonId), $"{nameof(query.LessonId)} cannot be empty."));
        }

        var lesson = await lessonRepository.GetByIdAsync(query.LessonId, cancellationToken);
        if (lesson is null)
        {
            return Result<IReadOnlyCollection<LessonSectionDto>>.NotFound(nameof(query.LessonId), "Lesson was not found.");
        }

        var sections = await lessonSectionRepository.GetByLessonIdAsync(query.LessonId, cancellationToken);
        var mediaIds = sections
            .Where(section => section.MediaId.HasValue)
            .Select(section => section.MediaId!.Value)
            .Distinct()
            .ToArray();
        var media = mediaIds.Length == 0
            ? []
            : await mediaRepository.GetByIdsAsync(mediaIds, cancellationToken);
        var mediaById = media.ToDictionary(item => item.Id);

        var items = sections
            .Select(section =>
            {
                MediaEntity? sectionMedia = section.MediaId.HasValue &&
                    mediaById.TryGetValue(section.MediaId.Value, out var foundMedia)
                        ? foundMedia
                        : null;
                return LessonSectionMapper.ToDto(section, sectionMedia);
            })
            .ToArray();

        return Result<IReadOnlyCollection<LessonSectionDto>>.Success(items);
    }
}
