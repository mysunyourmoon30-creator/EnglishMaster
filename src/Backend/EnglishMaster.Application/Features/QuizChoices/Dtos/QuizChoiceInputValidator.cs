using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizChoices.Dtos;

internal static class QuizChoiceInputValidator
{
    public static Result<QuizChoiceInput> Validate(
        string? choiceText,
        bool isCorrect,
        string? explanationTh,
        string? explanationEn,
        int sortOrder,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedChoiceText = NormalizeRequired(
            choiceText,
            nameof(choiceText),
            QuizChoiceFieldLimits.ChoiceText,
            errors);
        var normalizedExplanationTh = NormalizeOptional(
            explanationTh,
            nameof(explanationTh),
            QuizChoiceFieldLimits.ExplanationTh,
            errors);
        var normalizedExplanationEn = NormalizeOptional(
            explanationEn,
            nameof(explanationEn),
            QuizChoiceFieldLimits.ExplanationEn,
            errors);
        ValidateNonNegative(sortOrder, nameof(sortOrder), errors);

        return errors.Count > 0
            ? Result<QuizChoiceInput>.Validation([.. errors])
            : Result<QuizChoiceInput>.Success(new QuizChoiceInput(
                normalizedChoiceText,
                isCorrect,
                normalizedExplanationTh,
                normalizedExplanationEn,
                sortOrder,
                isActive));
    }

    private static string NormalizeRequired(string? value, string field, int maxLength, ICollection<ValidationError> errors)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            errors.Add(new ValidationError(field, $"{field} is required."));
            return string.Empty;
        }

        if (normalized.Length > maxLength)
        {
            errors.Add(new ValidationError(field, $"{field} must be {maxLength} characters or fewer."));
        }

        return normalized;
    }

    private static string NormalizeOptional(string? value, string field, int maxLength, ICollection<ValidationError> errors)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            errors.Add(new ValidationError(field, $"{field} must be {maxLength} characters or fewer."));
        }

        return normalized;
    }

    private static void ValidateNonNegative(int value, string field, ICollection<ValidationError> errors)
    {
        if (value < 0)
        {
            errors.Add(new ValidationError(field, $"{field} must be greater than or equal to zero."));
        }
    }
}
