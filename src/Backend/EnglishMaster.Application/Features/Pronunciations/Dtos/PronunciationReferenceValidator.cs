using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Domain.Media;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Pronunciations.Dtos;

internal static class PronunciationReferenceValidator
{
    public static async Task<PronunciationReferenceValidation> ValidateAsync(
        IWordRepository wordRepository,
        IMediaRepository mediaRepository,
        PronunciationInput input,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        Word? word = null;
        MediaEntity? audioSlowMedia = null;
        MediaEntity? audioNormalMedia = null;
        MediaEntity? mouthImageMedia = null;

        word = await wordRepository.GetByIdAsync(input.WordId, cancellationToken);
        if (word is null || !word.IsActive)
        {
            errors.Add(new ValidationError(nameof(input.WordId), "Word was not found or is inactive."));
        }

        if (input.AudioSlowMediaId.HasValue)
        {
            audioSlowMedia = await mediaRepository.GetByIdAsync(input.AudioSlowMediaId.Value, cancellationToken);
            if (audioSlowMedia is null || !audioSlowMedia.IsActive || audioSlowMedia.MediaType != MediaType.Audio)
            {
                errors.Add(new ValidationError(
                    nameof(input.AudioSlowMediaId),
                    "Slow audio media was not found, is inactive, or is not audio."));
            }
        }

        if (input.AudioNormalMediaId.HasValue)
        {
            audioNormalMedia = await mediaRepository.GetByIdAsync(input.AudioNormalMediaId.Value, cancellationToken);
            if (audioNormalMedia is null || !audioNormalMedia.IsActive || audioNormalMedia.MediaType != MediaType.Audio)
            {
                errors.Add(new ValidationError(
                    nameof(input.AudioNormalMediaId),
                    "Normal audio media was not found, is inactive, or is not audio."));
            }
        }

        if (input.MouthImageMediaId.HasValue)
        {
            mouthImageMedia = await mediaRepository.GetByIdAsync(input.MouthImageMediaId.Value, cancellationToken);
            if (mouthImageMedia is null || !mouthImageMedia.IsActive || mouthImageMedia.MediaType != MediaType.Image)
            {
                errors.Add(new ValidationError(
                    nameof(input.MouthImageMediaId),
                    "Mouth image media was not found, is inactive, or is not an image."));
            }
        }

        return new PronunciationReferenceValidation(
            word,
            audioSlowMedia,
            audioNormalMedia,
            mouthImageMedia,
            errors);
    }
}

internal sealed record PronunciationReferenceValidation(
    Word? Word,
    MediaEntity? AudioSlowMedia,
    MediaEntity? AudioNormalMedia,
    MediaEntity? MouthImageMedia,
    IReadOnlyCollection<ValidationError> Errors);
