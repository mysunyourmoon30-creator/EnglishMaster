namespace EnglishMaster.Domain.ContentQuality;

public sealed class ContentQualityRule
{
    private ContentQualityRule()
    {
        Code = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        ContentType = string.Empty;
    }

    private ContentQualityRule(string code, string name, string? description, string contentType, ContentQualitySeverity severity, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        Code = Required(code, nameof(Code), 128);
        Name = Required(name, nameof(Name), 256);
        Description = Optional(description, nameof(Description), 1000);
        ContentType = Required(contentType, nameof(ContentType), 64);
        Severity = severity;
        IsActive = true;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public ContentQualitySeverity Severity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static ContentQualityRule Create(string code, string name, string? description, string contentType, ContentQualitySeverity severity, DateTimeOffset now) =>
        new(code, name, description, contentType, severity, now);

    public void Update(string code, string name, string? description, string contentType, ContentQualitySeverity severity, DateTimeOffset now)
    {
        Code = Required(code, nameof(Code), 128);
        Name = Required(name, nameof(Name), 256);
        Description = Optional(description, nameof(Description), 1000);
        ContentType = Required(contentType, nameof(ContentType), 64);
        Severity = severity;
        UpdatedAt = now;
    }

    public void Activate(DateTimeOffset now)
    {
        IsActive = true;
        UpdatedAt = now;
    }

    public void Deactivate(DateTimeOffset now)
    {
        IsActive = false;
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
