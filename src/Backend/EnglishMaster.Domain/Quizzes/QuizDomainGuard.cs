namespace EnglishMaster.Domain.Quizzes;

internal static class QuizDomainGuard
{
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
        if (value == Guid.Empty)
        {
            throw new ArgumentException($"{parameterName} cannot be empty.", parameterName);
        }

        return value;
    }

    public static string RequiredText(
        string? value,
        string fieldName,
        string parameterName,
        int maxLength)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException($"{fieldName} is required.", parameterName);
        }

        if (normalized.Length > maxLength)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"{fieldName} must be {maxLength} characters or fewer.");
        }

        return normalized;
    }

    public static string OptionalText(
        string? value,
        string fieldName,
        string parameterName,
        int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"{fieldName} must be {maxLength} characters or fewer.");
        }

        return normalized;
    }

    public static int NonNegative(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be greater than or equal to zero.");
        }

        return value;
    }

    public static int Positive(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be greater than zero.");
        }

        return value;
    }

    public static int Percentage(int value, string parameterName)
    {
        if (value is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be between 0 and 100.");
        }

        return value;
    }

    public static TEnum DefinedEnum<TEnum>(TEnum value, string parameterName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} is invalid.");
        }

        return value;
    }
}
