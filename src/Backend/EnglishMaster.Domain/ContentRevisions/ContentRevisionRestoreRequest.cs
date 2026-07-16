namespace EnglishMaster.Domain.ContentRevisions;

public sealed class ContentRevisionRestoreRequest
{
    private ContentRevisionRestoreRequest()
    {
        RequestedBy = string.Empty;
        Reason = string.Empty;
        ReviewedBy = string.Empty;
        ReviewNote = string.Empty;
    }

    private ContentRevisionRestoreRequest(Guid contentRevisionId, string requestedBy, DateTimeOffset requestedAt, string reason, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        ContentRevisionId = contentRevisionId == Guid.Empty ? throw new ArgumentException("ContentRevisionId is required.", nameof(contentRevisionId)) : contentRevisionId;
        RequestedBy = Required(requestedBy, nameof(RequestedBy), 256);
        RequestedAt = requestedAt;
        Reason = Required(reason, nameof(Reason), 1000);
        Status = ContentRevisionRestoreStatus.Pending;
        CreatedAt = now;
        UpdatedAt = now;
        ReviewedBy = string.Empty;
        ReviewNote = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid ContentRevisionId { get; private set; }
    public string RequestedBy { get; private set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public ContentRevisionRestoreStatus Status { get; private set; }
    public string ReviewedBy { get; private set; } = string.Empty;
    public DateTimeOffset? ReviewedAt { get; private set; }
    public string ReviewNote { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static ContentRevisionRestoreRequest Create(Guid contentRevisionId, string requestedBy, DateTimeOffset requestedAt, string reason, DateTimeOffset now) =>
        new(contentRevisionId, requestedBy, requestedAt, reason, now);

    public void Approve(string reviewedBy, string? note, DateTimeOffset now)
    {
        if (Status != ContentRevisionRestoreStatus.Pending)
        {
            throw new InvalidOperationException("Only pending restore requests can be approved.");
        }

        Status = ContentRevisionRestoreStatus.Approved;
        ReviewedBy = Required(reviewedBy, nameof(reviewedBy), 256);
        ReviewNote = Optional(note, nameof(note), 1000);
        ReviewedAt = now;
        UpdatedAt = now;
    }

    public void Reject(string reviewedBy, string? note, DateTimeOffset now)
    {
        if (Status != ContentRevisionRestoreStatus.Pending)
        {
            throw new InvalidOperationException("Only pending restore requests can be rejected.");
        }

        Status = ContentRevisionRestoreStatus.Rejected;
        ReviewedBy = Required(reviewedBy, nameof(reviewedBy), 256);
        ReviewNote = Optional(note, nameof(note), 1000);
        ReviewedAt = now;
        UpdatedAt = now;
    }

    public void Complete(DateTimeOffset now)
    {
        if (Status != ContentRevisionRestoreStatus.Approved)
        {
            throw new InvalidOperationException("Only approved restore requests can be completed.");
        }

        Status = ContentRevisionRestoreStatus.Completed;
        UpdatedAt = now;
    }

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
