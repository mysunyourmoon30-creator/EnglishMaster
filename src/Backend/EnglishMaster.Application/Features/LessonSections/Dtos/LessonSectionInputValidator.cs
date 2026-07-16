using EnglishMaster.Domain.Lessons;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.LessonSections.Dtos;

internal static class LessonSectionInputValidator
{
    public static Result<LessonSectionInput> Validate(
        string? title,
        string? contentMarkdown,
        string? sectionType,
        Guid? mediaId,
        int sortOrder,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedTitle = NormalizeRequired(title, nameof(title), LessonSectionFieldLimits.Title, errors);
        var normalizedContentMarkdown = NormalizeOptional(contentMarkdown, nameof(contentMarkdown), LessonSectionFieldLimits.ContentMarkdown, errors);
        var parsedSectionType = ParseSectionType(sectionType, errors);

        if (mediaId.HasValue && mediaId.Value == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(mediaId), $"{nameof(mediaId)} cannot be empty."));
        }

        if (sortOrder < 0)
        {
            errors.Add(new ValidationError(nameof(sortOrder), $"{nameof(sortOrder)} must be greater than or equal to zero."));
        }

        if (errors.Count > 0)
        {
            return Result<LessonSectionInput>.Validation([.. errors]);
        }

        return Result<LessonSectionInput>.Success(new LessonSectionInput(
            normalizedTitle,
            normalizedContentMarkdown,
            parsedSectionType,
            mediaId,
            sortOrder,
            isActive));
    }

    private static SectionType ParseSectionType(
        string? value,
        ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Enum.TryParse<SectionType>(value.Trim(), ignoreCase: true, out var parsed) ||
            !Enum.IsDefined(parsed))
        {
            errors.Add(new ValidationError("sectionType", "sectionType is required and must be a valid section type."));
            return default;
        }

        return parsed;
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
