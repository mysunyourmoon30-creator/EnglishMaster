using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.MinimalPairs.Dtos;
using EnglishMaster.Application.Features.Pronunciations;
using EnglishMaster.Contracts.MinimalPairs;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.MinimalPairs.Queries;

public sealed class GetMinimalPairsByPronunciationIdQueryHandler
{
    private readonly IMinimalPairRepository minimalPairRepository;
    private readonly IPronunciationRepository pronunciationRepository;
    private readonly IMediaRepository mediaRepository;

    public GetMinimalPairsByPronunciationIdQueryHandler(
        IMinimalPairRepository minimalPairRepository,
        IPronunciationRepository pronunciationRepository,
        IMediaRepository mediaRepository)
    {
        this.minimalPairRepository = minimalPairRepository;
        this.pronunciationRepository = pronunciationRepository;
        this.mediaRepository = mediaRepository;
    }

    public async Task<Result<IReadOnlyCollection<MinimalPairDto>>> HandleAsync(
        GetMinimalPairsByPronunciationIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.PronunciationId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<MinimalPairDto>>.Validation(
                new ValidationError(nameof(query.PronunciationId), $"{nameof(query.PronunciationId)} cannot be empty."));
        }

        var pronunciation = await pronunciationRepository.GetByIdAsync(query.PronunciationId, cancellationToken);
        if (pronunciation is null)
        {
            return Result<IReadOnlyCollection<MinimalPairDto>>.NotFound(
                nameof(query.PronunciationId),
                "Pronunciation was not found.");
        }

        var minimalPairs = await minimalPairRepository.GetByPronunciationIdAsync(
            query.PronunciationId,
            cancellationToken);
        var audioIds = minimalPairs
            .Select(pair => pair.AudioMediaId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToArray();
        var audioMedia = audioIds.Length == 0
            ? []
            : await mediaRepository.GetByIdsAsync(audioIds, cancellationToken);
        var mediaById = audioMedia.ToDictionary(media => media.Id);

        IReadOnlyCollection<MinimalPairDto> items = minimalPairs
            .OrderBy(pair => pair.SortOrder)
            .ThenBy(pair => pair.PairWordText)
            .Select(pair =>
            {
                var media = pair.AudioMediaId.HasValue &&
                    mediaById.TryGetValue(pair.AudioMediaId.Value, out var foundMedia)
                        ? foundMedia
                        : null;

                return MinimalPairMapper.ToDto(pair, media);
            })
            .ToArray();

        return Result<IReadOnlyCollection<MinimalPairDto>>.Success(items);
    }
}
