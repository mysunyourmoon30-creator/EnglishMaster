using EnglishMaster.Domain.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.PublishJobs.Dtos;

internal static class PublishInputValidator
{
    public static List<ValidationError> ValidateJob(
        string? sourceType,
        Guid sourceId,
        string? format,
        string? title,
        string? requestedBy)
    {
        var errors = new List<ValidationError>();
        ParseOptionalEnum<PublishSourceType>(sourceType, nameof(sourceType), errors, required: true);
        ValidateRequiredId(sourceId, nameof(sourceId), errors);
        ParseOptionalEnum<PublishFormat>(format, nameof(format), errors, required: true);
        ValidateText(title, nameof(title), PublishingFieldLimits.Title, required: true, errors);
        ValidateText(requestedBy, nameof(requestedBy), PublishingFieldLimits.RequestedBy, required: false, errors);
        return errors;
    }

    public static List<ValidationError> ValidateTemplate(
        string? name,
        string? description,
        string? format,
        string? templateContent)
    {
        var errors = new List<ValidationError>();
        ValidateText(name, nameof(name), PublishingFieldLimits.TemplateName, required: true, errors);
        ValidateText(description, nameof(description), PublishingFieldLimits.TemplateDescription, required: false, errors);
        ParseOptionalEnum<PublishFormat>(format, nameof(format), errors, required: true);
        ValidateText(templateContent, nameof(templateContent), PublishingFieldLimits.TemplateContent, required: false, errors);
        return errors;
    }

    public static TEnum? ParseOptionalEnum<TEnum>(string? value, string field, ICollection<ValidationError> errors, bool required = false)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (required)
            {
                errors.Add(new ValidationError(field, $"{field} is required."));
            }

            return null;
        }

        if (Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out var parsed) && Enum.IsDefined(parsed))
        {
            return parsed;
        }

        errors.Add(new ValidationError(field, $"{field} is invalid."));
        return null;
    }

    public static TEnum ParseOptionalEnum<TEnum>(string? value, string field, TEnum defaultValue, ICollection<ValidationError> errors)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        return ParseOptionalEnum<TEnum>(value, field, errors) ?? defaultValue;
    }

    public static void ValidateOptionalId(Guid? value, string field, ICollection<ValidationError> errors)
    {
        if (value == Guid.Empty)
        {
            errors.Add(new ValidationError(field, $"{field} cannot be empty."));
        }
    }

    private static void ValidateRequiredId(Guid value, string field, ICollection<ValidationError> errors)
    {
        if (value == Guid.Empty)
        {
            errors.Add(new ValidationError(field, $"{field} is required."));
        }
    }

    private static void ValidateText(string? value, string field, int maxLength, bool required, ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (required)
            {
                errors.Add(new ValidationError(field, $"{field} is required."));
            }

            return;
        }

        if (value.Trim().Length > maxLength)
        {
            errors.Add(new ValidationError(field, $"{field} must be {maxLength} characters or fewer."));
        }
    }
}
