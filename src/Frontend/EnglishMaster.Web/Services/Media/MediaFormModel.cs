using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Media;

namespace EnglishMaster.Web.Services.Media;

public sealed class MediaFormModel
{
    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? FileExtension { get; set; }

    [Required]
    [StringLength(120)]
    public string ContentType { get; set; } = string.Empty;

    [Range(1, long.MaxValue)]
    public long FileSize { get; set; } = 1;

    [Required]
    public string MediaType { get; set; } = "Image";

    [StringLength(500)]
    public string? PublicUrl { get; set; }

    [StringLength(300)]
    public string? AltText { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public static MediaFormModel FromDto(MediaDto media)
    {
        return new MediaFormModel
        {
            FileName = media.FileName,
            OriginalFileName = media.OriginalFileName,
            FileExtension = media.FileExtension,
            ContentType = media.ContentType,
            FileSize = media.FileSize,
            MediaType = media.MediaType,
            PublicUrl = media.PublicUrl,
            AltText = media.AltText,
            Description = media.Description,
            IsActive = media.IsActive
        };
    }

    public CreateMediaRequest ToCreateRequest()
    {
        return new CreateMediaRequest(
            FileName,
            OriginalFileName,
            FileExtension,
            ContentType,
            FileSize,
            MediaType,
            PublicUrl,
            AltText,
            Description);
    }

    public UpdateMediaRequest ToUpdateRequest()
    {
        return new UpdateMediaRequest(
            FileName,
            OriginalFileName,
            FileExtension,
            ContentType,
            FileSize,
            MediaType,
            PublicUrl,
            AltText,
            Description,
            IsActive);
    }
}
