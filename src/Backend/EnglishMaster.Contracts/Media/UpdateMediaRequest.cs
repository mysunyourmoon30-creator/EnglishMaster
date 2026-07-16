namespace EnglishMaster.Contracts.Media;

public sealed record UpdateMediaRequest(
    string FileName,
    string OriginalFileName,
    string? FileExtension,
    string ContentType,
    long FileSize,
    string MediaType,
    string? PublicUrl,
    string? AltText,
    string? Description,
    bool IsActive);
