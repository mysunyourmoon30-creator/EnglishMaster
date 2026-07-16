using EnglishMaster.Application.Features.Media;
using EnglishMaster.Domain.Media;
using EnglishMaster.Shared.Results;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.MinimalPairs.Dtos;

internal static class MinimalPairReferenceValidator
{
    public static async Task<MinimalPairReferenceValidation> ValidateAsync(
        IMediaRepository mediaRepository,
        MinimalPairInput input,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        MediaEntity? audioMedia = null;

        if (input.AudioMediaId.HasValue)
        {
            audioMedia = await mediaRepository.GetByIdAsync(input.AudioMediaId.Value, cancellationToken);
            if (audioMedia is null || !audioMedia.IsActive || audioMedia.MediaType != MediaType.Audio)
            {
                errors.Add(new ValidationError(
                    nameof(input.AudioMediaId),
                    "Audio media was not found, is inactive, or is not audio."));
            }
        }

        return new MinimalPairReferenceValidation(audioMedia, errors);
    }
}

internal sealed record MinimalPairReferenceValidation(
    MediaEntity? AudioMedia,
    IReadOnlyCollection<ValidationError> Errors);
