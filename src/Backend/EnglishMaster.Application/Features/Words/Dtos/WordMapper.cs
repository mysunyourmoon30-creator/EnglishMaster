using EnglishMaster.Contracts.Words;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Domain.Words;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Words.Dtos;

internal static class WordMapper
{
    public static WordDto ToDto(
        Word word,
        Category? category = null,
        IReadOnlyCollection<Tag>? tags = null,
        MediaEntity? imageMedia = null,
        MediaEntity? audioMedia = null,
        Pronunciation? pronunciation = null,
        MediaEntity? pronunciationAudioSlowMedia = null,
        MediaEntity? pronunciationAudioNormalMedia = null,
        MediaEntity? pronunciationMouthImageMedia = null,
        IReadOnlyDictionary<Guid, MediaEntity>? minimalPairMediaById = null)
    {
        return new WordDto(
            word.Id,
            word.Text,
            word.Slug,
            word.IpaUk,
            word.IpaUs,
            word.ThaiReading,
            word.MeaningTh,
            word.MeaningEn,
            word.PartOfSpeech.ToString(),
            word.CefrLevel.ToString(),
            word.ExampleEn,
            word.ExampleTh,
            word.CategoryId,
            category is null
                ? null
                : new WordCategoryDto(category.Id, category.Name, category.Slug),
            tags?.Select(tag => new WordTagDto(tag.Id, tag.Name, tag.Slug)).ToArray() ?? [],
            word.ImageMediaId,
            imageMedia is null
                ? null
                : new WordMediaDto(
                    imageMedia.Id,
                    imageMedia.FileName,
                    imageMedia.ContentType,
                    imageMedia.MediaType.ToString(),
                    imageMedia.PublicUrl,
                    imageMedia.AltText),
            word.AudioMediaId,
            audioMedia is null
                ? null
                : new WordMediaDto(
                    audioMedia.Id,
                    audioMedia.FileName,
                    audioMedia.ContentType,
                    audioMedia.MediaType.ToString(),
                    audioMedia.PublicUrl,
                    audioMedia.AltText),
            pronunciation is null
                ? null
                : new WordPronunciationDto(
                    pronunciation.Id,
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
                    ToWordMediaDto(pronunciationAudioSlowMedia),
                    pronunciation.AudioNormalMediaId,
                    ToWordMediaDto(pronunciationAudioNormalMedia),
                    pronunciation.MouthImageMediaId,
                    ToWordMediaDto(pronunciationMouthImageMedia),
                    pronunciation.MinimalPairs
                        .Where(pair => pair.IsActive)
                        .OrderBy(pair => pair.SortOrder)
                        .ThenBy(pair => pair.PairWordText)
                        .Select(pair =>
                        {
                            MediaEntity? pairAudio = null;
                            if (pair.AudioMediaId.HasValue && minimalPairMediaById is not null)
                            {
                                minimalPairMediaById.TryGetValue(pair.AudioMediaId.Value, out pairAudio);
                            }

                            return new WordMinimalPairDto(
                                pair.Id,
                                pair.PairWordText,
                                pair.PairIpa,
                                pair.PairThaiReading,
                                pair.DifferenceNote,
                                pair.AudioMediaId,
                                ToWordMediaDto(pairAudio),
                                pair.SortOrder,
                                pair.IsActive);
                        })
                        .ToArray(),
                    pronunciation.IsActive),
            word.IsActive,
            word.CreatedAt,
            word.UpdatedAt);
    }

    private static WordMediaDto? ToWordMediaDto(MediaEntity? media)
    {
        return media is null
            ? null
            : new WordMediaDto(
                media.Id,
                media.FileName,
                media.ContentType,
                media.MediaType.ToString(),
                media.PublicUrl,
                media.AltText);
    }
}
