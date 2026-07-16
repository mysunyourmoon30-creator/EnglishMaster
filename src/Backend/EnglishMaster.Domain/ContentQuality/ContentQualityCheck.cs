namespace EnglishMaster.Domain.ContentQuality;

public sealed class ContentQualityCheck
{
    private readonly List<ContentQualityFinding> findings = [];

    private ContentQualityCheck()
    {
        ContentType = string.Empty;
        CheckedBy = string.Empty;
    }

    private ContentQualityCheck(string contentType, Guid contentId, string? checkedBy, int score, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        ContentType = Required(contentType, nameof(ContentType), 64);
        ContentId = RequiredId(contentId, nameof(ContentId));
        CheckedBy = Optional(checkedBy, nameof(CheckedBy), 256);
        CheckedAt = now;
        Score = Percentage(score, nameof(score));
        Status = ContentQualityCheckStatus.NotChecked;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public string ContentType { get; private set; } = string.Empty;
    public Guid ContentId { get; private set; }
    public ContentQualityCheckStatus Status { get; private set; }
    public DateTimeOffset CheckedAt { get; private set; }
    public string CheckedBy { get; private set; } = string.Empty;
    public int Score { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<ContentQualityFinding> Findings => findings.AsReadOnly();

    public static ContentQualityCheck Create(string contentType, Guid contentId, string? checkedBy, int score, DateTimeOffset now) =>
        new(contentType, contentId, checkedBy, score, now);

    public void AddFinding(string ruleCode, ContentQualitySeverity severity, string message, string? fieldName, string? recommendation, DateTimeOffset now)
    {
        findings.Add(ContentQualityFinding.Create(Id, ruleCode, severity, message, fieldName, recommendation, now));
        UpdatedAt = now;
        RecalculateStatus();
    }

    public void Complete(DateTimeOffset now)
    {
        RecalculateStatus();
        UpdatedAt = now;
    }

    private void RecalculateStatus()
    {
        if (findings.Any(finding => finding.Severity is ContentQualitySeverity.Error or ContentQualitySeverity.Critical))
        {
            Status = ContentQualityCheckStatus.Failed;
        }
        else if (findings.Any(finding => finding.Severity == ContentQualitySeverity.Warning))
        {
            Status = ContentQualityCheckStatus.Warning;
        }
        else
        {
            Status = ContentQualityCheckStatus.Passed;
        }
    }

    private static Guid RequiredId(Guid value, string fieldName) =>
        value == Guid.Empty ? throw new ArgumentException($"{fieldName} is required.", fieldName) : value;

    private static int Percentage(int value, string fieldName) =>
        value is < 0 or > 100 ? throw new ArgumentException($"{fieldName} must be between 0 and 100.", fieldName) : value;

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
