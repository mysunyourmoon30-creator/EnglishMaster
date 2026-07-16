namespace EnglishMaster.Contracts.Media;

public sealed record MediaDto(
    Guid Id,
    string FileName,
    string OriginalFileName,
    string FileExtension,
    string ContentType,
    long FileSize,
    string MediaType,
    string PublicUrl,
    string AltText,
    string Description,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
