using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarTopics.Dtos;

internal static class GrammarTopicInputValidator
{
    public static Result<GrammarTopicInput> Validate(
        string? title,
        string? summary,
        string? cefrLevel,
        int sortOrder,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedTitle = NormalizeRequired(title, nameof(title), GrammarTopicFieldLimits.Title, errors);
        var normalizedSlug = NormalizeSlug(normalizedTitle, nameof(title), errors);
        var normalizedSummary = NormalizeOptional(summary, nameof(summary), GrammarTopicFieldLimits.Summary, errors);
        var parsedCefrLevel = ParseRequiredEnum<CefrLevel>(cefrLevel, nameof(cefrLevel), errors);

        if (sortOrder < 0)
        {
            errors.Add(new ValidationError(nameof(sortOrder), $"{nameof(sortOrder)} must be greater than or equal to zero."));
        }

        if (errors.Count > 0)
        {
            return Result<GrammarTopicInput>.Validation([.. errors]);
        }

        return Result<GrammarTopicInput>.Success(new GrammarTopicInput(
            normalizedTitle,
            normalizedSlug,
            normalizedSummary,
            parsedCefrLevel!.Value,
            sortOrder,
            isActive));
    }

    private static string NormalizeSlug(
        string normalizedTitle,
        string field,
        ICollection<ValidationError> errors)
    {
        if (normalizedTitle.Length == 0)
        {
            return string.Empty;
        }

        try
        {
            return GrammarTopic.GenerateSlug(normalizedTitle);
        }
        catch (ArgumentException)
        {
            errors.Add(new ValidationError(field, $"{field} must contain at least one letter or digit."));
            return string.Empty;
        }
    }

    private static string NormalizeRequired(
        string? value,
        string field,
        int maxLength,
        ICollection<ValidationError> errors)
    {
        var normalized = NormalizeOptional(value, field, maxLength, errors);
        if (normalized.Length == 0)
        {
            errors.Add(new ValidationError(field, $"{field} is required."));
        }

        return normalized;
    }

    private static string NormalizeOptional(
        string? value,
        string field,
        int maxLength,
        ICollection<ValidationError> errors)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            errors.Add(new ValidationError(field, $"{field} must be {maxLength} characters or fewer."));
        }

        return normalized;
    }

    private static TEnum? ParseRequiredEnum<TEnum>(
        string? value,
        string field,
        ICollection<ValidationError> errors)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new ValidationError(field, $"{field} is required."));
            return null;
        }

        if (Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out var parsed) &&
            Enum.IsDefined(parsed))
        {
            return parsed;
        }

        errors.Add(new ValidationError(field, $"{field} is invalid."));
        return null;
    }
}
