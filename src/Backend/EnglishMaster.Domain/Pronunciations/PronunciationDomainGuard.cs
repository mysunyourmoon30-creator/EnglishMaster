namespace EnglishMaster.Domain.Pronunciations;

internal static class PronunciationDomainGuard
{
    public static Guid RequiredId(Guid value, string fieldName)
    {
        return value == Guid.Empty
            ? throw new ArgumentException($"{fieldName} cannot be empty.", fieldName)
            : value;
    }

    public static Guid? OptionalId(Guid? value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);
        }

        return value;
    }

    public static string RequiredText(string? value, string fieldName, int maxLength)
    {
        var normalized = OptionalText(value, fieldName, maxLength);
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
    }

    public static string OptionalText(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName);
        }

        return normalized;
    }
}
