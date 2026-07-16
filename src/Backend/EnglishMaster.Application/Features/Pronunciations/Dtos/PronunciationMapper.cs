using EnglishMaster.Application.Features.MinimalPairs.Dtos;
using EnglishMaster.Contracts.Pronunciations;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Domain.Words;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Pronunciations.Dtos;

internal static class PronunciationMapper
{
    public static PronunciationDto ToDto(
        Pronunciation pronunciation,
        Word? word = null,
        IReadOnlyCollection<MinimalPair>? minimalPairs = null,
        MediaEntity? audioSlowMedia = null,
        MediaEntity? audioNormalMedia = null,
        MediaEntity? mouthImageMedia = null,
        IReadOnlyDictionary<Guid, MediaEntity>? minimalPairMediaById = null)
    {
        var pairs = minimalPairs ?? pronunciation.MinimalPairs;

        return new PronunciationDto(
            pronunciation.Id,
            pronunciation.WordId,
            word is null ? null : new PronunciationWordDto(word.Id, word.Text, word.Slug),
            pronunciation.IpaUk,
            pronunciation.IpaUs,
            pronunciation.ThaiReading,
            pronunciation.Syllables,
            pronunciation.StressPattern,
            pronunciation.MouthPosition,
            pronunciation.TonguePosition,
            pronunciation.CommonMistake,
            pronunciation.PracticeNote,
            pronunciation.AudioSlowMediaId,
            ToMediaDto(audioSlowMedia),
            pronunciation.AudioNormalMediaId,
            ToMediaDto(audioNormalMedia),
            pronunciation.MouthImageMediaId,
            ToMediaDto(mouthImageMedia),
            pairs
                .OrderBy(pair => pair.SortOrder)
                .ThenBy(pair => pair.PairWordText)
                .Select(pair =>
                {
                    MediaEntity? audioMedia = null;
                    if (pair.AudioMediaId.HasValue && minimalPairMediaById is not null)
                    {
                        minimalPairMediaById.TryGetValue(pair.AudioMediaId.Value, out audioMedia);
                    }

                    return MinimalPairMapper.ToDto(pair, audioMedia);
                })
                .ToArray(),
            pronunciation.IsActive,
            pronunciation.CreatedAt,
            pronunciation.UpdatedAt);
    }

    private static PronunciationMediaDto? ToMediaDto(MediaEntity? media)
    {
        return media is null
            ? null
            : new PronunciationMediaDto(
                media.Id,
                media.FileName,
                media.ContentType,
                media.MediaType.ToString(),
                media.PublicUrl,
                media.AltText);
    }
}
