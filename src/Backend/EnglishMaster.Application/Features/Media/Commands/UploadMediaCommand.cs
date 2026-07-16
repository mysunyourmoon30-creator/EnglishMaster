namespace EnglishMaster.Application.Features.Media.Commands;

public sealed record UploadMediaCommand(
    Stream Content,
    string OriginalFileName,
    string ContentType,
    long FileSize,
    string? AltText,
    string? Description);
