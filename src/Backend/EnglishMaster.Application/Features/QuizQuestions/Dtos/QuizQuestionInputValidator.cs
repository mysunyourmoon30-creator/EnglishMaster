using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizQuestions.Dtos;

internal static class QuizQuestionInputValidator
{
    public static Result<QuizQuestionInput> Validate(
        string? questionText,
        string? questionType,
        string? explanationTh,
        string? explanationEn,
        int points,
        int sortOrder,
        Guid? wordId,
        Guid? grammarRuleId,
        Guid? pronunciationId,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedQuestionText = NormalizeRequired(
            questionText,
            nameof(questionText),
            QuizQuestionFieldLimits.QuestionText,
            errors);
        var parsedQuestionType = ParseRequiredQuestionType(questionType, nameof(questionType), errors);
        var normalizedExplanationTh = NormalizeOptional(
            explanationTh,
            nameof(explanationTh),
            QuizQuestionFieldLimits.ExplanationTh,
            errors);
        var normalizedExplanationEn = NormalizeOptional(
            explanationEn,
            nameof(explanationEn),
            QuizQuestionFieldLimits.ExplanationEn,
            errors);
        ValidatePositive(points, nameof(points), errors);
        ValidateNonNegative(sortOrder, nameof(sortOrder), errors);
        ValidateOptionalId(wordId, nameof(wordId), errors);
        ValidateOptionalId(grammarRuleId, nameof(grammarRuleId), errors);
        ValidateOptionalId(pronunciationId, nameof(pronunciationId), errors);

        return errors.Count > 0
            ? Result<QuizQuestionInput>.Validation([.. errors])
            : Result<QuizQuestionInput>.Success(new QuizQuestionInput(
                normalizedQuestionText,
                parsedQuestionType,
                normalizedExplanationTh,
                normalizedExplanationEn,
                points,
                sortOrder,
                wordId,
                grammarRuleId,
                pronunciationId,
                isActive));
    }

    private static QuizQuestionType ParseRequiredQuestionType(
        string? value,
        string field,
        ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new ValidationError(field, $"{field} is required."));
            return QuizQuestionType.SingleChoice;
        }

        if (Enum.TryParse<QuizQuestionType>(value.Trim(), ignoreCase: true, out var parsed) &&
            Enum.IsDefined(parsed))
        {
            return parsed;
        }

        errors.Add(new ValidationError(field, $"{field} is invalid."));
        return QuizQuestionType.SingleChoice;
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

    private static void ValidateOptionalId(Guid? value, string field, ICollection<ValidationError> errors)
    {
        if (value == Guid.Empty)
        {
            errors.Add(new ValidationError(field, $"{field} cannot be empty."));
        }
    }

    private static void ValidatePositive(int value, string field, ICollection<ValidationError> errors)
    {
        if (value <= 0)
        {
            errors.Add(new ValidationError(field, $"{field} must be greater than zero."));
        }
    }

    private static void ValidateNonNegative(int value, string field, ICollection<ValidationError> errors)
    {
        if (value < 0)
        {
            errors.Add(new ValidationError(field, $"{field} must be greater than or equal to zero."));
        }
    }
}
