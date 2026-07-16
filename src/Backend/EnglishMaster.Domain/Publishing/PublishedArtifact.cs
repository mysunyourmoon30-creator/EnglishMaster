namespace EnglishMaster.Domain.Publishing;

public sealed class PublishedArtifact
{
    private PublishedArtifact()
    {
        FileName = string.Empty;
        FilePath = string.Empty;
        PublicUrl = string.Empty;
        ContentType = string.Empty;
    }

    private PublishedArtifact(
        Guid id,
        Guid publishJobId,
        PublishSourceType sourceType,
        Guid sourceId,
        PublishFormat format,
        string? fileName,
        string? filePath,
        string? publicUrl,
        long fileSize,
        string? contentType,
        DateTimeOffset createdAt)
    {
        Id = PublishingDomainGuard.RequiredId(id, nameof(id));
        PublishJobId = PublishingDomainGuard.RequiredId(publishJobId, nameof(publishJobId));
        SourceType = sourceType;
        SourceId = PublishingDomainGuard.RequiredId(sourceId, nameof(sourceId));
        Format = format;
        FileName = PublishingDomainGuard.RequiredText(fileName, nameof(FileName), nameof(fileName), PublishingFieldLimits.ArtifactFileName);
        FilePath = PublishingDomainGuard.RequiredText(filePath, nameof(FilePath), nameof(filePath), PublishingFieldLimits.ArtifactFilePath);
        PublicUrl = PublishingDomainGuard.OptionalText(publicUrl, nameof(PublicUrl), nameof(publicUrl), PublishingFieldLimits.ArtifactPublicUrl);
        FileSize = PublishingDomainGuard.NonNegative(fileSize, nameof(fileSize));
        ContentType = PublishingDomainGuard.OptionalText(contentType, nameof(ContentType), nameof(contentType), PublishingFieldLimits.ArtifactContentType);
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid PublishJobId { get; private set; }

    public PublishSourceType SourceType { get; private set; }

    public Guid SourceId { get; private set; }

    public PublishFormat Format { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string FilePath { get; private set; } = string.Empty;

    public string PublicUrl { get; private set; } = string.Empty;

    public long FileSize { get; private set; }

    public string ContentType { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static PublishedArtifact Create(
        Guid publishJobId,
        PublishSourceType sourceType,
        Guid sourceId,
        PublishFormat format,
        string? fileName,
        string? filePath,
        string? publicUrl,
        long fileSize,
        string? contentType,
        DateTimeOffset now)
    {
        return new PublishedArtifact(
            Guid.NewGuid(),
            publishJobId,
            sourceType,
            sourceId,
            format,
            fileName,
            filePath,
            publicUrl,
            fileSize,
            contentType,
            now);
    }
}
