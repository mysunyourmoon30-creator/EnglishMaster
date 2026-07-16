using EnglishMaster.Contracts.Media;

namespace EnglishMaster.Application.Features.Media.Dtos;

internal static class MediaMapper
{
    public static MediaDto ToDto(Domain.Media.Media media)
    {
        return new MediaDto(
            media.Id,
            media.FileName,
            media.OriginalFileName,
            media.FileExtension,
            media.ContentType,
            media.FileSize,
            media.MediaType.ToString(),
            media.PublicUrl,
            media.AltText,
            media.Description,
            media.IsActive,
            media.CreatedAt,
            media.UpdatedAt);
    }
}
