namespace EnglishMaster.Application.Features.Publishing;

public sealed record StoredPublishFile(
    string FileName,
    string RelativePath,
    string PublicUrl,
    long FileSize,
    string ContentType);
