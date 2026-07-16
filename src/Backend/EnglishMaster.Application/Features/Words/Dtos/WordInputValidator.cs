using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Words.Dtos;

internal static class WordInputValidator
{
    public static Result<WordInput> Validate(
        string? text,
        string? ipaUk,
        string? ipaUs,
        string? thaiReading,
        string? meaningTh,
        string? meaningEn,
        string? partOfSpeech,
        string? cefrLevel,
        string? exampleEn,
        string? exampleTh,
        Guid? categoryId,
        IReadOnlyCollection<Guid>? tagIds,
        Guid? imageMediaId,
        Guid? audioMediaId,
        bool isActive)
    {
        var errors = new List<ValidationError>();

        var normalizedText = NormalizeRequired(text, nameof(text), WordFieldLimits.Text, errors);
        var normalizedSlug = NormalizeSlug(normalizedText, nameof(text), errors);
        var normalizedIpaUk = NormalizeOptional(ipaUk, nameof(ipaUk), WordFieldLimits.IpaUk, errors);
        var normalizedIpaUs = NormalizeOptional(ipaUs, nameof(ipaUs), WordFieldLimits.IpaUs, errors);
        var normalizedThaiReading = NormalizeOptional(thaiReading, nameof(thaiReading), WordFieldLimits.ThaiReading, errors);
        var normalizedMeaningTh = NormalizeRequired(meaningTh, nameof(meaningTh), WordFieldLimits.MeaningTh, errors);
        var normalizedMeaningEn = NormalizeOptional(meaningEn, nameof(meaningEn), WordFieldLimits.MeaningEn, errors);
        var normalizedExampleEn = NormalizeOptional(exampleEn, nameof(exampleEn), WordFieldLimits.ExampleEn, errors);
        var normalizedExampleTh = NormalizeOptional(exampleTh, nameof(exampleTh), WordFieldLimits.ExampleTh, errors);

        var parsedPartOfSpeech = ParseEnum<PartOfSpeech>(partOfSpeech, nameof(partOfSpeech), errors);
        var parsedCefrLevel = ParseEnum<CefrLevel>(cefrLevel, nameof(cefrLevel), errors);
        var normalizedCategoryId = NormalizeCategoryId(categoryId, errors);
        var normalizedTagIds = NormalizeTagIds(tagIds, errors);
        var normalizedImageMediaId = NormalizeOptionalId(imageMediaId, nameof(imageMediaId), errors);
        var normalizedAudioMediaId = NormalizeOptionalId(audioMediaId, nameof(audioMediaId), errors);

        if (errors.Count > 0)
        {
            return Result<WordInput>.Validation([.. errors]);
        }

        return Result<WordInput>.Success(new WordInput(
            normalizedText,
            normalizedSlug,
            normalizedIpaUk,
            normalizedIpaUs,
            normalizedThaiReading,
            normalizedMeaningTh,
            normalizedMeaningEn,
            parsedPartOfSpeech!.Value,
            parsedCefrLevel!.Value,
            normalizedExampleEn,
            normalizedExampleTh,
            normalizedCategoryId,
            normalizedTagIds,
            normalizedImageMediaId,
            normalizedAudioMediaId,
            isActive));
    }

    private static Guid? NormalizeCategoryId(
        Guid? categoryId,
        ICollection<ValidationError> errors)
    {
        return NormalizeOptionalId(categoryId, nameof(categoryId), errors);
    }

    private static Guid? NormalizeOptionalId(
        Guid? value,
        string field,
        ICollection<ValidationError> errors)
    {
        if (value == Guid.Empty)
        {
            errors.Add(new ValidationError(field, $"{field} cannot be empty."));
            return null;
        }

        return value;
    }

    private static IReadOnlyCollection<Guid> NormalizeTagIds(
        IReadOnlyCollection<Guid>? tagIds,
        ICollection<ValidationError> errors)
    {
        if (tagIds is null || tagIds.Count == 0)
        {
            return [];
        }

        if (tagIds.Any(tagId => tagId == Guid.Empty))
        {
            errors.Add(new ValidationError(nameof(tagIds), $"{nameof(tagIds)} cannot contain empty values."));
        }

        return tagIds
            .Where(tagId => tagId != Guid.Empty)
            .Distinct()
            .ToArray();
    }

    private static string NormalizeSlug(
        string normalizedText,
        string field,
        ICollection<ValidationError> errors)
    {
        if (normalizedText.Length == 0)
        {
            return string.Empty;
        }

        try
        {
            return Word.GenerateSlug(normalizedText);
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

    private static TEnum? ParseEnum<TEnum>(
        string? value,
        string field,
        ICollection<ValidationError> errors)
        where TEnum : struct, Enum
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            errors.Add(new ValidationError(field, $"{field} is required."));
            return null;
        }

        if (Enum.TryParse<TEnum>(normalized, ignoreCase: true, out var parsed) &&
            Enum.IsDefined(parsed))
        {
            return parsed;
        }

        errors.Add(new ValidationError(field, $"{field} is invalid."));
        return null;
    }
}
