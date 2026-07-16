using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Pronunciations;
using EnglishMaster.Domain.Pronunciations;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Pronunciations.Dtos;

internal static class PronunciationReadModelBuilder
{
    public static async Task<PronunciationDto> MapAsync(
        Pronunciation pronunciation,
        IWordRepository wordRepository,
        IMediaRepository mediaRepository,
        bool includeMinimalPairs,
        CancellationToken cancellationToken)
    {
        var word = await wordRepository.GetByIdAsync(pronunciation.WordId, cancellationToken);
        var minimalPairs = includeMinimalPairs
            ? pronunciation.MinimalPairs
            : [];

        var mediaIds = new[]
            {
                pronunciation.AudioSlowMediaId,
                pronunciation.AudioNormalMediaId,
                pronunciation.MouthImageMediaId
            }
            .Concat(minimalPairs.Select(pair => pair.AudioMediaId))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToArray();

        var media = mediaIds.Length == 0
            ? []
            : await mediaRepository.GetByIdsAsync(mediaIds, cancellationToken);
        var mediaById = media.ToDictionary(item => item.Id);

        return PronunciationMapper.ToDto(
            pronunciation,
            word,
            minimalPairs,
            GetMedia(mediaById, pronunciation.AudioSlowMediaId),
            GetMedia(mediaById, pronunciation.AudioNormalMediaId),
            GetMedia(mediaById, pronunciation.MouthImageMediaId),
            mediaById);
    }

    private static MediaEntity? GetMedia(
        IReadOnlyDictionary<Guid, MediaEntity> mediaById,
        Guid? id)
    {
        return id.HasValue && mediaById.TryGetValue(id.Value, out var media)
            ? media
            : null;
    }
}
