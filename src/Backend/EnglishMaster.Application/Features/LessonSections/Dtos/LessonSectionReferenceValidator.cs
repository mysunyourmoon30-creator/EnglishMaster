using EnglishMaster.Application.Features.Media;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.LessonSections.Dtos;

internal static class LessonSectionReferenceValidator
{
    public static async Task<IReadOnlyCollection<ValidationError>> ValidateMediaAsync(
        IMediaRepository mediaRepository,
        Guid mediaId,
        CancellationToken cancellationToken)
    {
        var media = await mediaRepository.GetByIdAsync(mediaId, cancellationToken);
        if (media is null || !media.IsActive)
        {
            return [new ValidationError(nameof(mediaId), "Media was not found or is inactive.")];
        }

        return [];
    }
}
