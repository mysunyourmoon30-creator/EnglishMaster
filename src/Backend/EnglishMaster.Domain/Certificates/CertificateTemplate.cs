namespace EnglishMaster.Domain.Certificates;

public sealed class CertificateTemplate
{
    private CertificateTemplate()
    {
        Code = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        BodyTemplate = string.Empty;
    }

    private CertificateTemplate(string code, string name, string? description, string bodyTemplate, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        Code = Required(code, nameof(Code), 64);
        CreatedAt = now;
        Apply(name, description, bodyTemplate, isActive: true, now);
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string BodyTemplate { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static CertificateTemplate Create(string code, string name, string? description, string bodyTemplate, DateTimeOffset now) =>
        new(code, name, description, bodyTemplate, now);

    public void Update(string name, string? description, string bodyTemplate, bool isActive, DateTimeOffset now) =>
        Apply(name, description, bodyTemplate, isActive, now);

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

    private void Apply(string name, string? description, string bodyTemplate, bool isActive, DateTimeOffset now)
    {
        Name = Required(name, nameof(Name), 160);
        Description = Optional(description, nameof(Description), 1000);
        BodyTemplate = Required(bodyTemplate, nameof(BodyTemplate), 8000);
        IsActive = isActive;
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
