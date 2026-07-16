namespace EnglishMaster.Domain.Motivation;

public sealed class AchievementDefinition
{
    private AchievementDefinition()
    {
        Code = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        AchievementType = string.Empty;
        IconName = string.Empty;
    }

    private AchievementDefinition(string code, string name, string? description, string achievementType, int targetValue, string? iconName, int sortOrder, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        Code = RequiredText(code, nameof(Code), 80);
        Name = RequiredText(name, nameof(Name), 160);
        Description = description?.Trim() ?? string.Empty;
        AchievementType = RequiredText(achievementType, nameof(AchievementType), 80);
        TargetValue = targetValue < 0 ? throw new ArgumentException("TargetValue must not be negative.", nameof(targetValue)) : targetValue;
        IconName = iconName?.Trim() ?? string.Empty;
        IsActive = true;
        SortOrder = sortOrder < 0 ? throw new ArgumentException("SortOrder must not be negative.", nameof(sortOrder)) : sortOrder;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string AchievementType { get; private set; } = string.Empty;
    public int TargetValue { get; private set; }
    public string IconName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static AchievementDefinition Create(string code, string name, string? description, string achievementType, int targetValue, string? iconName, int sortOrder, DateTimeOffset now) =>
        new(code, name, description, achievementType, targetValue, iconName, sortOrder, now);

    public void Update(string name, string? description, string achievementType, int targetValue, string? iconName, int sortOrder, DateTimeOffset now)
    {
        Name = RequiredText(name, nameof(Name), 160);
        Description = description?.Trim() ?? string.Empty;
        AchievementType = RequiredText(achievementType, nameof(AchievementType), 80);
        TargetValue = targetValue < 0 ? throw new ArgumentException("TargetValue must not be negative.", nameof(targetValue)) : targetValue;
        IconName = iconName?.Trim() ?? string.Empty;
        SortOrder = sortOrder < 0 ? throw new ArgumentException("SortOrder must not be negative.", nameof(sortOrder)) : sortOrder;
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
