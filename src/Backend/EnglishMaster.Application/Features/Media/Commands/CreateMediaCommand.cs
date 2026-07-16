namespace EnglishMaster.Application.Features.Media.Commands;

public sealed record CreateMediaCommand(
    string FileName,
    string OriginalFileName,
    string? FileExtension,
    string ContentType,
    long FileSize,
    string MediaType,
    string? PublicUrl,
    string? AltText,
    string? Description);
