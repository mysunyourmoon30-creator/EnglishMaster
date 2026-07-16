namespace EnglishMaster.Application.Features.Media.Dtos;

public sealed record StoredMediaFile(
    string FileName,
    string OriginalFileName,
    string FileExtension,
    string ContentType,
    long FileSize,
    Domain.Media.MediaType MediaType,
    string StoragePath,
    string PublicUrl);
