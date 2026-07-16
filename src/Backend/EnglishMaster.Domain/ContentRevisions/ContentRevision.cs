namespace EnglishMaster.Domain.ContentRevisions;

public sealed class ContentRevision
{
    private ContentRevision()
    {
        ContentType = string.Empty;
        Title = string.Empty;
        Summary = string.Empty;
        ChangedBy = string.Empty;
        ChangeReason = string.Empty;
        SnapshotJson = string.Empty;
        DiffJson = string.Empty;
    }

    private ContentRevision(string contentType, Guid contentId, int revisionNumber, ContentRevisionEventType eventType, string? title, string? summary, string? changedBy, DateTimeOffset changedAt, string? changeReason, string snapshotJson, string? diffJson, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        ContentType = Required(contentType, nameof(ContentType), 64);
        ContentId = contentId == Guid.Empty ? throw new ArgumentException("ContentId is required.", nameof(contentId)) : contentId;
        RevisionNumber = revisionNumber <= 0 ? throw new ArgumentException("RevisionNumber must be greater than zero.", nameof(revisionNumber)) : revisionNumber;
        EventType = eventType;
        Title = Optional(title, nameof(Title), 256);
        Summary = Optional(summary, nameof(Summary), 1000);
        ChangedBy = Optional(changedBy, nameof(ChangedBy), 256);
        ChangedAt = changedAt;
        ChangeReason = Optional(changeReason, nameof(ChangeReason), 1000);
        SnapshotJson = Required(snapshotJson, nameof(SnapshotJson), 8000);
        DiffJson = Optional(diffJson, nameof(DiffJson), 8000);
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public string ContentType { get; private set; } = string.Empty;
    public Guid ContentId { get; private set; }
    public int RevisionNumber { get; private set; }
    public ContentRevisionEventType EventType { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;
    public string ChangedBy { get; private set; } = string.Empty;
    public DateTimeOffset ChangedAt { get; private set; }
    public string ChangeReason { get; private set; } = string.Empty;
    public string SnapshotJson { get; private set; } = string.Empty;
    public string DiffJson { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static ContentRevision Create(string contentType, Guid contentId, int revisionNumber, ContentRevisionEventType eventType, string? title, string? summary, string? changedBy, DateTimeOffset changedAt, string? changeReason, string snapshotJson, string? diffJson, DateTimeOffset now) =>
        new(contentType, contentId, revisionNumber, eventType, title, summary, changedBy, changedAt, changeReason, snapshotJson, diffJson, now);

    private static string Required(string? value, string fieldName, int maxLength)
    {
        var normalized = Optional(value, fieldName, maxLength);
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
    }

    private static string Optional(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName);
        }

        return normalized;
    }
}
