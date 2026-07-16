using EnglishMaster.Contracts.MinimalPairs;
using EnglishMaster.Domain.Pronunciations;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.MinimalPairs.Dtos;

internal static class MinimalPairMapper
{
    public static MinimalPairDto ToDto(
        MinimalPair minimalPair,
        MediaEntity? audioMedia = null)
    {
        return new MinimalPairDto(
            minimalPair.Id,
            minimalPair.PronunciationId,
            minimalPair.PairWordText,
            minimalPair.PairIpa,
            minimalPair.PairThaiReading,
            minimalPair.DifferenceNote,
            minimalPair.AudioMediaId,
            audioMedia is null
                ? null
                : new MinimalPairMediaDto(
                    audioMedia.Id,
                    audioMedia.FileName,
                    audioMedia.ContentType,
                    audioMedia.MediaType.ToString(),
                    audioMedia.PublicUrl,
                    audioMedia.AltText),
            minimalPair.SortOrder,
            minimalPair.IsActive,
            minimalPair.CreatedAt,
            minimalPair.UpdatedAt);
    }
}
