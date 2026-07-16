using EnglishMaster.Domain.Categories;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Categories.Dtos;

internal static class CategoryInputValidator
{
    public static Result<CategoryInput> Validate(
        string? name,
        string? description,
        int sortOrder,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedName = NormalizeRequired(name, nameof(name), CategoryFieldLimits.Name, errors);
        var normalizedSlug = NormalizeSlug(normalizedName, nameof(name), errors);
        var normalizedDescription = NormalizeOptional(
            description,
            nameof(description),
            CategoryFieldLimits.Description,
            errors);

        if (errors.Count > 0)
        {
            return Result<CategoryInput>.Validation([.. errors]);
        }

        return Result<CategoryInput>.Success(new CategoryInput(
            normalizedName,
            normalizedSlug,
            normalizedDescription,
            sortOrder,
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
            return Category.GenerateSlug(normalizedName);
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
