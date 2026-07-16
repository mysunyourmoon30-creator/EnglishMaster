namespace EnglishMaster.Domain.Notifications;

public sealed class EmailMessage
{
    private EmailMessage()
    {
        ToEmail = string.Empty;
        ToName = string.Empty;
        Subject = string.Empty;
        Body = string.Empty;
        ErrorMessage = string.Empty;
    }

    private EmailMessage(string toEmail, string? toName, string subject, string body, bool isHtml, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        ToEmail = Required(toEmail, nameof(toEmail), 256);
        ToName = Optional(toName, nameof(toName), 256);
        Subject = Required(subject, nameof(subject), 256);
        Body = Required(body, nameof(body), 8000);
        IsHtml = isHtml;
        Status = EmailMessageStatus.Pending;
        CreatedAt = now;
        UpdatedAt = now;
        ErrorMessage = string.Empty;
    }

    public Guid Id { get; private set; }
    public string ToEmail { get; private set; } = string.Empty;
    public string ToName { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public bool IsHtml { get; private set; }
    public EmailMessageStatus Status { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public DateTimeOffset? FailedAt { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static EmailMessage Queue(string toEmail, string? toName, string subject, string body, bool isHtml, DateTimeOffset now) =>
        new(toEmail, toName, subject, body, isHtml, now);

    public void MarkSent(DateTimeOffset now)
    {
        Status = EmailMessageStatus.Sent;
        SentAt = now;
        UpdatedAt = now;
        ErrorMessage = string.Empty;
    }

    public void MarkFailed(string errorMessage, DateTimeOffset now)
    {
        Status = EmailMessageStatus.Failed;
        FailedAt = now;
        UpdatedAt = now;
        ErrorMessage = Required(errorMessage, nameof(errorMessage), 1000);
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
