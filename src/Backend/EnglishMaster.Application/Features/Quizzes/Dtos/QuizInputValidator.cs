using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Quizzes.Dtos;

internal static class QuizInputValidator
{
    public static Result<QuizInput> Validate(
        string? title,
        string? summary,
        string? description,
        string? cefrLevel,
        Guid? categoryId,
        Guid? lessonId,
        Guid? courseId,
        Guid? bookId,
        int timeLimitMinutes,
        int passingScore,
        int sortOrder,
        bool isPublished,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedTitle = NormalizeRequired(title, nameof(title), QuizFieldLimits.Title, errors);
        var normalizedSummary = NormalizeOptional(summary, nameof(summary), QuizFieldLimits.Summary, errors);
        var normalizedDescription = NormalizeOptional(description, nameof(description), QuizFieldLimits.Description, errors);
        var parsedCefrLevel = ParseOptionalEnum<CefrLevel>(cefrLevel, nameof(cefrLevel), errors);
        ValidateOptionalId(categoryId, nameof(categoryId), errors);
        ValidateOptionalId(lessonId, nameof(lessonId), errors);
        ValidateOptionalId(courseId, nameof(courseId), errors);
        ValidateOptionalId(bookId, nameof(bookId), errors);
        ValidateNonNegative(timeLimitMinutes, nameof(timeLimitMinutes), errors);
        ValidatePercentage(passingScore, nameof(passingScore), errors);
        ValidateNonNegative(sortOrder, nameof(sortOrder), errors);

        string slug = string.Empty;
        if (!string.IsNullOrWhiteSpace(normalizedTitle))
        {
            try
            {
                slug = Quiz.GenerateSlug(normalizedTitle);
            }
            catch (ArgumentException exception)
            {
                errors.Add(new ValidationError(nameof(title), exception.Message));
            }
        }

        return errors.Count > 0
            ? Result<QuizInput>.Validation([.. errors])
            : Result<QuizInput>.Success(new QuizInput(
                normalizedTitle,
                slug,
                normalizedSummary,
                normalizedDescription,
                parsedCefrLevel,
                categoryId,
                lessonId,
                courseId,
                bookId,
                timeLimitMinutes,
                passingScore,
                sortOrder,
                isPublished,
                isActive));
    }

    private static string NormalizeRequired(
        string? value,
        string field,
        int maxLength,
        ICollection<ValidationError> errors)
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

    private static TEnum? ParseOptionalEnum<TEnum>(
        string? value,
        string field,
        ICollection<ValidationError> errors)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
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

    private static void ValidateOptionalId(Guid? value, string field, ICollection<ValidationError> errors)
    {
        if (value == Guid.Empty)
        {
            errors.Add(new ValidationError(field, $"{field} cannot be empty."));
        }
    }

    private static void ValidateNonNegative(int value, string field, ICollection<ValidationError> errors)
    {
        if (value < 0)
        {
            errors.Add(new ValidationError(field, $"{field} must be greater than or equal to zero."));
        }
    }

    private static void ValidatePercentage(int value, string field, ICollection<ValidationError> errors)
    {
        if (value is < 0 or > 100)
        {
            errors.Add(new ValidationError(field, $"{field} must be between 0 and 100."));
        }
    }
}
