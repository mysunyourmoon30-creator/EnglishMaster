namespace EnglishMaster.Domain.Publishing;

internal static class PublishingDomainGuard
{
    public static Guid RequiredId(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException($"{parameterName} is required.", parameterName);
        }

        return value;
    }

    public static string RequiredText(string? value, string fieldName, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is required.", parameterName);
        }

        return OptionalText(value, fieldName, parameterName, maxLength);
    }

    public static string OptionalText(string? value, string fieldName, string parameterName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", parameterName);
        }

        return normalized;
    }

    public static long NonNegative(long value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must not be negative.");
        }

        return value;
    }
}
