namespace EnglishMaster.Application.Features.Media.Commands;

public sealed record UpdateMediaCommand(
    Guid Id,
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
