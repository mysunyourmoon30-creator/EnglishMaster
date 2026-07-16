using EnglishMaster.Domain.Tags;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Tags.Dtos;

internal static class TagInputValidator
{
    public static Result<TagInput> Validate(
        string? name,
        string? description,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedName = NormalizeRequired(name, nameof(name), TagFieldLimits.Name, errors);
        var normalizedSlug = NormalizeSlug(normalizedName, nameof(name), errors);
        var normalizedDescription = NormalizeOptional(
            description,
            nameof(description),
            TagFieldLimits.Description,
            errors);

        if (errors.Count > 0)
        {
            return Result<TagInput>.Validation([.. errors]);
        }

        return Result<TagInput>.Success(new TagInput(
            normalizedName,
            normalizedSlug,
            normalizedDescription,
            isActive));
    }

    private static string NormalizeSlug(
        string normalizedName,
        string field,
        ICollection<ValidationError> errors)
    {
        if (normalizedName.Length == 0)
        {
            return string.Empty;
        }

        try
        {
            return Tag.GenerateSlug(normalizedName);
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
}
