namespace EnglishMaster.Application.Features.Media.Dtos;

internal sealed record MediaInput(
    string FileName,
    string OriginalFileName,
    string FileExtension,
    string ContentType,
    long FileSize,
    Domain.Media.MediaType MediaType,
    string StoragePath,
    string PublicUrl,
    string AltText,
    string Description,
    bool IsActive);
