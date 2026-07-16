using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Lessons.Dtos;

internal static class LessonInputValidator
{
    public static Result<LessonInput> Validate(
        string? title,
        string? summary,
        string? description,
        string? cefrLevel,
        Guid? categoryId,
        Guid? thumbnailMediaId,
        int estimatedMinutes,
        int sortOrder,
        bool isPublished,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedTitle = NormalizeRequired(title, nameof(title), LessonFieldLimits.Title, errors);
        var normalizedSlug = NormalizeSlug(normalizedTitle, nameof(title), errors);
        var normalizedSummary = NormalizeOptional(summary, nameof(summary), LessonFieldLimits.Summary, errors);
        var normalizedDescription = NormalizeOptional(description, nameof(description), LessonFieldLimits.Description, errors);
        var parsedCefrLevel = ParseOptionalCefrLevel(cefrLevel, errors);
        ValidateOptionalId(categoryId, nameof(categoryId), errors);
        ValidateOptionalId(thumbnailMediaId, nameof(thumbnailMediaId), errors);

        if (estimatedMinutes < 0)
        {
            errors.Add(new ValidationError(nameof(estimatedMinutes), $"{nameof(estimatedMinutes)} must be greater than or equal to zero."));
        }

        if (sortOrder < 0)
        {
            errors.Add(new ValidationError(nameof(sortOrder), $"{nameof(sortOrder)} must be greater than or equal to zero."));
        }

        if (errors.Count > 0)
        {
            return Result<LessonInput>.Validation([.. errors]);
        }

        return Result<LessonInput>.Success(new LessonInput(
            normalizedTitle,
            normalizedSlug,
            normalizedSummary,
            normalizedDescription,
            parsedCefrLevel,
            categoryId,
            thumbnailMediaId,
            estimatedMinutes,
            sortOrder,
            isPublished,
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
            return Lesson.GenerateSlug(normalizedTitle);
        }
        catch (ArgumentException)
        {
            errors.Add(new ValidationError(field, $"{field} must contain at least one letter or digit."));
            return string.Empty;
        }
    }

    private static CefrLevel? ParseOptionalCefrLevel(
        string? value,
        ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!Enum.TryParse<CefrLevel>(value, ignoreCase: true, out var parsed) || !Enum.IsDefined(parsed))
        {
            errors.Add(new ValidationError("cefrLevel", "cefrLevel is invalid."));
            return null;
        }

        return parsed;
    }

    private static void ValidateOptionalId(
        Guid? value,
        string field,
        ICollection<ValidationError> errors)
    {
        if (value.HasValue && value.Value == Guid.Empty)
        {
            errors.Add(new ValidationError(field, $"{field} cannot be empty."));
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
}
