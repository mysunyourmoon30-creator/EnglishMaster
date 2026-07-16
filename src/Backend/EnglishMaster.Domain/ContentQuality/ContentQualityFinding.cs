namespace EnglishMaster.Domain.ContentQuality;

public sealed class ContentQualityFinding
{
    private ContentQualityFinding()
    {
        RuleCode = string.Empty;
        Message = string.Empty;
        FieldName = string.Empty;
        Recommendation = string.Empty;
    }

    private ContentQualityFinding(Guid contentQualityCheckId, string ruleCode, ContentQualitySeverity severity, string message, string? fieldName, string? recommendation, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        ContentQualityCheckId = contentQualityCheckId == Guid.Empty ? throw new ArgumentException("ContentQualityCheckId is required.", nameof(contentQualityCheckId)) : contentQualityCheckId;
        RuleCode = Required(ruleCode, nameof(RuleCode), 128);
        Severity = severity;
        Message = Required(message, nameof(Message), 1000);
        FieldName = Optional(fieldName, nameof(FieldName), 128);
        Recommendation = Optional(recommendation, nameof(Recommendation), 1000);
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid ContentQualityCheckId { get; private set; }
    public string RuleCode { get; private set; } = string.Empty;
    public ContentQualitySeverity Severity { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public string FieldName { get; private set; } = string.Empty;
    public string Recommendation { get; private set; } = string.Empty;
    public bool IsResolved { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static ContentQualityFinding Create(Guid contentQualityCheckId, string ruleCode, ContentQualitySeverity severity, string message, string? fieldName, string? recommendation, DateTimeOffset now) =>
        new(contentQualityCheckId, ruleCode, severity, message, fieldName, recommendation, now);

    public void MarkResolved(DateTimeOffset now)
    {
        IsResolved = true;
        ResolvedAt = now;
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
