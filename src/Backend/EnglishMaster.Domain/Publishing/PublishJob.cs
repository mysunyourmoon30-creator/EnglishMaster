namespace EnglishMaster.Domain.Publishing;

public sealed class PublishJob
{
    private readonly List<PublishedArtifact> artifacts = [];

    private PublishJob()
    {
        Title = string.Empty;
        OutputFileName = string.Empty;
        OutputPath = string.Empty;
        ErrorMessage = string.Empty;
        RequestedBy = string.Empty;
    }

    private PublishJob(
        Guid id,
        PublishSourceType sourceType,
        Guid sourceId,
        PublishFormat format,
        string? title,
        string? requestedBy,
        DateTimeOffset createdAt)
    {
        Id = PublishingDomainGuard.RequiredId(id, nameof(id));
        SourceType = sourceType;
        SourceId = PublishingDomainGuard.RequiredId(sourceId, nameof(sourceId));
        Format = format;
        Status = PublishJobStatus.Pending;
        Title = PublishingDomainGuard.RequiredText(title, nameof(Title), nameof(title), PublishingFieldLimits.Title);
        RequestedBy = PublishingDomainGuard.OptionalText(requestedBy, nameof(RequestedBy), nameof(requestedBy), PublishingFieldLimits.RequestedBy);
        OutputFileName = string.Empty;
        OutputPath = string.Empty;
        ErrorMessage = string.Empty;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public PublishSourceType SourceType { get; private set; }

    public Guid SourceId { get; private set; }

    public PublishFormat Format { get; private set; }

    public PublishJobStatus Status { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string OutputFileName { get; private set; } = string.Empty;

    public string OutputPath { get; private set; } = string.Empty;

    public string ErrorMessage { get; private set; } = string.Empty;

    public string RequestedBy { get; private set; } = string.Empty;

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<PublishedArtifact> Artifacts => artifacts.AsReadOnly();

    public static PublishJob Create(
        PublishSourceType sourceType,
        Guid sourceId,
        PublishFormat format,
        string? title,
        string? requestedBy,
        DateTimeOffset now)
    {
        return new PublishJob(Guid.NewGuid(), sourceType, sourceId, format, title, requestedBy, now);
    }

    public void MarkRunning(DateTimeOffset now)
    {
        if (Status is PublishJobStatus.Completed or PublishJobStatus.Cancelled)
        {
            throw new InvalidOperationException("Completed or cancelled publish jobs cannot be started.");
        }

        Status = PublishJobStatus.Running;
        StartedAt ??= now;
        ErrorMessage = string.Empty;
        UpdatedAt = now;
    }

    public void MarkCompleted(string? outputFileName, string? outputPath, DateTimeOffset now)
    {
        if (Status == PublishJobStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled publish jobs cannot be completed.");
        }

        Status = PublishJobStatus.Completed;
        OutputFileName = PublishingDomainGuard.RequiredText(outputFileName, nameof(OutputFileName), nameof(outputFileName), PublishingFieldLimits.OutputFileName);
        OutputPath = PublishingDomainGuard.RequiredText(outputPath, nameof(OutputPath), nameof(outputPath), PublishingFieldLimits.OutputPath);
        CompletedAt = now;
        UpdatedAt = now;
    }

    public void MarkFailed(string? errorMessage, DateTimeOffset now)
    {
        if (Status == PublishJobStatus.Completed)
        {
            throw new InvalidOperationException("Completed publish jobs cannot be failed.");
        }

        Status = PublishJobStatus.Failed;
        ErrorMessage = PublishingDomainGuard.RequiredText(errorMessage, nameof(ErrorMessage), nameof(errorMessage), PublishingFieldLimits.ErrorMessage);
        CompletedAt = now;
        UpdatedAt = now;
    }

    public void Cancel(DateTimeOffset now)
    {
        if (Status == PublishJobStatus.Completed)
        {
            throw new InvalidOperationException("Completed publish jobs cannot be cancelled.");
        }

        Status = PublishJobStatus.Cancelled;
        CompletedAt = now;
        UpdatedAt = now;
    }
}
