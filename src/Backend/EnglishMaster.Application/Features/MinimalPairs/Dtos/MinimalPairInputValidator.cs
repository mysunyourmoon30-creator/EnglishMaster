using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.MinimalPairs.Dtos;

internal static class MinimalPairInputValidator
{
    public static Result<MinimalPairInput> Validate(
        string? pairWordText,
        string? pairIpa,
        string? pairThaiReading,
        string? differenceNote,
        Guid? audioMediaId,
        int sortOrder,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedPairWordText = NormalizeRequired(
            pairWordText,
            nameof(pairWordText),
            MinimalPairFieldLimits.PairWordText,
            errors);
        var normalizedPairIpa = NormalizeOptional(pairIpa, nameof(pairIpa), MinimalPairFieldLimits.PairIpa, errors);
        var normalizedPairThaiReading = NormalizeOptional(
            pairThaiReading,
            nameof(pairThaiReading),
            MinimalPairFieldLimits.PairThaiReading,
            errors);
        var normalizedDifferenceNote = NormalizeOptional(
            differenceNote,
            nameof(differenceNote),
            MinimalPairFieldLimits.DifferenceNote,
            errors);
        var normalizedAudioMediaId = NormalizeOptionalId(audioMediaId, nameof(audioMediaId), errors);

        if (sortOrder < 0)
        {
            errors.Add(new ValidationError(nameof(sortOrder), $"{nameof(sortOrder)} must be greater than or equal to zero."));
        }

        if (errors.Count > 0)
        {
            return Result<MinimalPairInput>.Validation([.. errors]);
        }

        return Result<MinimalPairInput>.Success(new MinimalPairInput(
            normalizedPairWordText,
            normalizedPairIpa,
            normalizedPairThaiReading,
            normalizedDifferenceNote,
            normalizedAudioMediaId,
            sortOrder,
            isActive));
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
