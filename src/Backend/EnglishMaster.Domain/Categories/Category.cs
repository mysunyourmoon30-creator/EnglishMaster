using EnglishMaster.Domain.Common;

namespace EnglishMaster.Domain.Categories;

public sealed class Category
{
    private Category()
    {
        Name = string.Empty;
        Slug = string.Empty;
        Description = string.Empty;
    }

    private Category(
        Guid id,
        string name,
        string description,
        int sortOrder,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(name, description, sortOrder, isActive, updatedAt);
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Category Create(
        string name,
        string description,
        int sortOrder,
        DateTimeOffset now)
    {
        return new Category(
            Guid.NewGuid(),
            name,
            description,
            sortOrder,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string name,
        string description,
        int sortOrder,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(name, description, sortOrder, isActive, now);
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
        return SlugGenerator.Generate(name, nameof(Name), nameof(name), CategoryFieldLimits.Name);
    }

    private void Apply(
        string name,
        string description,
        int sortOrder,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Name = NormalizeRequired(name, nameof(Name), CategoryFieldLimits.Name);
        Slug = GenerateSlug(Name);
        Description = NormalizeOptional(description, nameof(Description), CategoryFieldLimits.Description);
        SortOrder = sortOrder;

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
