namespace EnglishMaster.Contracts.Publishing;

public sealed record PublishedArtifactDto(
    Guid Id,
    Guid PublishJobId,
    string SourceType,
    Guid SourceId,
    string Format,
    string FileName,
    string FilePath,
    string PublicUrl,
    long FileSize,
    string ContentType,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
