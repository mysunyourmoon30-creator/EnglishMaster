using EnglishMaster.Domain.Common;

namespace EnglishMaster.Domain.Tags;

public sealed class Tag
{
    private Tag()
    {
        Name = string.Empty;
        Slug = string.Empty;
        Description = string.Empty;
    }

    private Tag(
        Guid id,
        string name,
        string description,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(name, description, isActive, updatedAt);
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Tag Create(
        string name,
        string description,
        DateTimeOffset now)
    {
        return new Tag(
            Guid.NewGuid(),
            name,
            description,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string name,
        string description,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(name, description, isActive, now);
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

    public static string GenerateSlug(string name)
    {
        return SlugGenerator.Generate(name, nameof(Name), nameof(name), TagFieldLimits.Name);
    }

    private void Apply(
        string name,
        string description,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Name = NormalizeRequired(name, nameof(Name), TagFieldLimits.Name);
        Slug = GenerateSlug(Name);
        Description = NormalizeOptional(description, nameof(Description), TagFieldLimits.Description);

        if (isActive)
        {
            Activate(updatedAt);
        }
        else
        {
            Deactivate(updatedAt);
        }
    }

    private static string NormalizeRequired(string? value, string fieldName, int maxLength)
    {
        var normalized = NormalizeOptional(value, fieldName, maxLength);
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
    }

    private static string NormalizeOptional(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName);
        }

        return normalized;
    }
}
