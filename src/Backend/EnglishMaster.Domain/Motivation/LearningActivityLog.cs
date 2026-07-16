namespace EnglishMaster.Domain.Motivation;

public sealed class LearningActivityLog
{
    private LearningActivityLog()
    {
        ActivityType = string.Empty;
        ContentType = string.Empty;
        Title = string.Empty;
        MetadataJson = string.Empty;
    }

    private LearningActivityLog(Guid studentProfileId, string activityType, string? contentType, Guid? contentId, string? title, DateTimeOffset occurredAt, int minutesSpent, string? metadataJson, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        StudentProfileId = studentProfileId == Guid.Empty ? throw new ArgumentException("StudentProfileId is required.", nameof(studentProfileId)) : studentProfileId;
        ActivityType = RequiredText(activityType, nameof(ActivityType), 64);
        ContentType = contentType?.Trim() ?? string.Empty;
        ContentId = contentId;
        Title = title?.Trim() ?? string.Empty;
        OccurredAt = occurredAt;
        MinutesSpent = minutesSpent < 0 ? throw new ArgumentException("MinutesSpent must not be negative.", nameof(minutesSpent)) : minutesSpent;
        MetadataJson = SanitizeMetadata(metadataJson);
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid StudentProfileId { get; private set; }
    public string ActivityType { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public Guid? ContentId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public int MinutesSpent { get; private set; }
    public string MetadataJson { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static LearningActivityLog Create(Guid studentProfileId, string activityType, string? contentType, Guid? contentId, string? title, DateTimeOffset occurredAt, int minutesSpent, string? metadataJson, DateTimeOffset now) =>
        new(studentProfileId, activityType, contentType, contentId, title, occurredAt, minutesSpent, metadataJson, now);

    private static string SanitizeMetadata(string? metadataJson)
    {
        var value = metadataJson?.Trim() ?? string.Empty;
        var lower = value.ToLowerInvariant();
        return lower.Contains("password", StringComparison.Ordinal) ||
            lower.Contains("token", StringComparison.Ordinal) ||
            lower.Contains("secret", StringComparison.Ordinal) ||
            lower.Contains("api_key", StringComparison.Ordinal) ||
            lower.Contains("authorization", StringComparison.Ordinal) ||
            lower.Contains("cookie", StringComparison.Ordinal) ||
            lower.Contains("credential", StringComparison.Ordinal)
            ? string.Empty
            : value;
    }

    private static string RequiredText(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized.Length > maxLength ? throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName) : normalized;
    }
}
