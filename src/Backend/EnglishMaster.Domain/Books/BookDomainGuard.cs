namespace EnglishMaster.Domain.Books;

internal static class BookDomainGuard
{
    public static string RequiredText(
        string? value,
        string displayName,
        string parameterName,
        int maxLength)
    {
        var normalized = OptionalText(value, displayName, parameterName, maxLength);
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{displayName} is required.", parameterName);
        }

        return normalized;
    }

    public static string OptionalText(
        string? value,
        string displayName,
        string parameterName,
        int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{displayName} must be {maxLength} characters or fewer.", parameterName);
        }

        return normalized;
    }

    public static Guid RequiredId(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException($"{parameterName} cannot be empty.", parameterName);
        }

        return value;
    }

    public static Guid? OptionalId(Guid? value, string parameterName)
    {
        if (value.HasValue && value.Value == Guid.Empty)
        {
            throw new ArgumentException($"{parameterName} cannot be empty.", parameterName);
        }

        return value;
    }

    public static int NonNegative(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} must be greater than or equal to zero.");
        }

        return value;
    }
}
