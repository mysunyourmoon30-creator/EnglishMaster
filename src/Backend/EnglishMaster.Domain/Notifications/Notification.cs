namespace EnglishMaster.Domain.Notifications;

public sealed class Notification
{
    private Notification()
    {
        Title = string.Empty;
        Message = string.Empty;
        ErrorMessage = string.Empty;
    }

    private Notification(
        Guid recipientUserId,
        NotificationType type,
        NotificationEventType eventType,
        string title,
        string message,
        DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        RecipientUserId = RequiredId(recipientUserId, nameof(recipientUserId));
        Type = type;
        EventType = eventType;
        Title = Required(title, nameof(title), 256);
        Message = Required(message, nameof(message), 2000);
        Status = NotificationStatus.Pending;
        CreatedAt = now;
        UpdatedAt = now;
        ErrorMessage = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationEventType EventType { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public DateTimeOffset? FailedAt { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Notification Create(
        Guid recipientUserId,
        NotificationType type,
        NotificationEventType eventType,
        string title,
        string message,
        DateTimeOffset now) =>
        new(recipientUserId, type, eventType, title, message, now);

    public void MarkSent(DateTimeOffset now)
    {
        Status = NotificationStatus.Sent;
        SentAt = now;
        UpdatedAt = now;
        ErrorMessage = string.Empty;
    }

    public void MarkFailed(string errorMessage, DateTimeOffset now)
    {
        Status = NotificationStatus.Failed;
        FailedAt = now;
        UpdatedAt = now;
        ErrorMessage = Required(errorMessage, nameof(errorMessage), 1000);
    }

    public void MarkRead(DateTimeOffset now)
    {
        Status = NotificationStatus.Read;
        ReadAt = now;
        UpdatedAt = now;
    }

    public void Cancel(DateTimeOffset now)
    {
        if (Status == NotificationStatus.Sent)
        {
            throw new InvalidOperationException("Sent notifications cannot be cancelled.");
        }

        Status = NotificationStatus.Cancelled;
        UpdatedAt = now;
    }

    private static Guid RequiredId(Guid value, string fieldName) =>
        value == Guid.Empty ? throw new ArgumentException($"{fieldName} is required.", fieldName) : value;

    private static string Required(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName);
        }

        return normalized;
    }
}
