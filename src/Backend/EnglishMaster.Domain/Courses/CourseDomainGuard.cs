namespace EnglishMaster.Domain.Courses;

internal static class CourseDomainGuard
{
    public static Guid RequiredId(Guid value, string parameterName)
    {
        return value == Guid.Empty
            ? throw new ArgumentException($"{parameterName} cannot be empty.", parameterName)
            : value;
    }

    public static Guid? OptionalId(Guid? value, string parameterName)
    {
        if (value.HasValue && value.Value == Guid.Empty)
        {
            throw new ArgumentException($"{parameterName} cannot be empty.", parameterName);
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

    public static int NonNegative(int value, string parameterName)
    {
        return value < 0
            ? throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be greater than or equal to zero.")
            : value;
    }
}
