using EnglishMaster.Domain.Media;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Media.Dtos;

internal static class MediaInputValidator
{
    public static Result<MediaInput> Validate(
        string? fileName,
        string? originalFileName,
        string? fileExtension,
        string? contentType,
        long fileSize,
        string? mediaType,
        string? storagePath,
        string? publicUrl,
        string? altText,
        string? description,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedFileName = NormalizeRequired(fileName, nameof(fileName), MediaFieldLimits.FileName, errors);
        var normalizedOriginalFileName = NormalizeRequired(
            originalFileName,
            nameof(originalFileName),
            MediaFieldLimits.OriginalFileName,
            errors);
        var normalizedFileExtension = NormalizeOptional(
            fileExtension,
            nameof(fileExtension),
            MediaFieldLimits.FileExtension,
            errors);
        var normalizedContentType = NormalizeRequired(contentType, nameof(contentType), MediaFieldLimits.ContentType, errors);
        var parsedMediaType = ParseMediaType(mediaType, nameof(mediaType), errors);
        var normalizedStoragePath = NormalizeRequired(storagePath, nameof(storagePath), MediaFieldLimits.StoragePath, errors);
        var normalizedPublicUrl = NormalizeOptional(publicUrl, nameof(publicUrl), MediaFieldLimits.PublicUrl, errors);
        var normalizedAltText = NormalizeOptional(altText, nameof(altText), MediaFieldLimits.AltText, errors);
        var normalizedDescription = NormalizeOptional(description, nameof(description), MediaFieldLimits.Description, errors);

        ValidateFileName(normalizedFileName, nameof(fileName), errors);
        ValidateFileName(normalizedOriginalFileName, nameof(originalFileName), errors);
        ValidateFileExtension(normalizedFileExtension, nameof(fileExtension), errors);
        ValidateStoragePath(normalizedStoragePath, nameof(storagePath), errors);
        ValidatePublicUrl(normalizedPublicUrl, nameof(publicUrl), errors);

        if (fileSize <= 0)
        {
            errors.Add(new ValidationError(nameof(fileSize), $"{nameof(fileSize)} must be greater than zero."));
        }

        if (errors.Count > 0)
        {
            return Result<MediaInput>.Validation([.. errors]);
        }

        return Result<MediaInput>.Success(new MediaInput(
            normalizedFileName,
            normalizedOriginalFileName,
            normalizedFileExtension,
            normalizedContentType,
            fileSize,
            parsedMediaType!.Value,
            normalizedStoragePath,
            normalizedPublicUrl,
            normalizedAltText,
            normalizedDescription,
            isActive));
    }

    private static Domain.Media.MediaType? ParseMediaType(
        string? value,
        string field,
        ICollection<ValidationError> errors)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            errors.Add(new ValidationError(field, $"{field} is required."));
            return null;
        }

        if (Enum.TryParse<Domain.Media.MediaType>(normalized, ignoreCase: true, out var parsed) &&
            Enum.IsDefined(parsed))
        {
            return parsed;
        }

        errors.Add(new ValidationError(field, $"{field} is invalid."));
        return null;
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

    private static void ValidateFileName(
        string value,
        string field,
        ICollection<ValidationError> errors)
    {
        if (value.Length == 0)
        {
            return;
        }

        if (value.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            value.Contains('/') ||
            value.Contains('\\'))
        {
            errors.Add(new ValidationError(field, $"{field} must be a file name, not a path."));
        }
    }

    private static void ValidateFileExtension(
        string value,
        string field,
        ICollection<ValidationError> errors)
    {
        if (value.Length == 0)
        {
            return;
        }

        if (!value.StartsWith('.') ||
            value.Contains('/') ||
            value.Contains('\\') ||
            value.Contains("..", StringComparison.Ordinal))
        {
            errors.Add(new ValidationError(field, $"{field} must be a simple file extension such as .jpg."));
        }
    }

    private static void ValidateStoragePath(
        string value,
        string field,
        ICollection<ValidationError> errors)
    {
        if (value.Length == 0)
        {
            return;
        }

        if (Path.IsPathRooted(value) ||
            value.Contains('\\') ||
            value.Split('/', StringSplitOptions.RemoveEmptyEntries).Any(part => part is "." or ".."))
        {
            errors.Add(new ValidationError(field, $"{field} must be a safe relative storage path."));
        }
    }

    private static void ValidatePublicUrl(
        string value,
        string field,
        ICollection<ValidationError> errors)
    {
        if (value.Length == 0)
        {
            return;
        }

        if (!value.StartsWith('/') ||
            value.StartsWith("//") ||
            value.Contains('\\') ||
            value.Split('/', StringSplitOptions.RemoveEmptyEntries).Any(part => part is "." or ".."))
        {
            errors.Add(new ValidationError(field, $"{field} must be a safe local URL path."));
        }
    }
}
