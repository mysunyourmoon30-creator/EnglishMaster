using EnglishMaster.Domain.Grammar;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarExamples.Dtos;

internal static class GrammarExampleInputValidator
{
    public static Result<GrammarExampleInput> Validate(
        string? exampleEn,
        string? translationTh,
        string? explanationTh,
        bool isCorrectExample,
        int sortOrder,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedExampleEn = NormalizeRequired(exampleEn, nameof(exampleEn), GrammarExampleFieldLimits.ExampleEn, errors);
        var normalizedTranslationTh = NormalizeOptional(translationTh, nameof(translationTh), GrammarExampleFieldLimits.TranslationTh, errors);
        var normalizedExplanationTh = NormalizeOptional(explanationTh, nameof(explanationTh), GrammarExampleFieldLimits.ExplanationTh, errors);

        if (sortOrder < 0)
        {
            errors.Add(new ValidationError(nameof(sortOrder), $"{nameof(sortOrder)} must be greater than or equal to zero."));
        }

        if (errors.Count > 0)
        {
            return Result<GrammarExampleInput>.Validation([.. errors]);
        }

        return Result<GrammarExampleInput>.Success(new GrammarExampleInput(
            normalizedExampleEn,
            normalizedTranslationTh,
            normalizedExplanationTh,
            isCorrectExample,
            sortOrder,
            isActive));
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
}
