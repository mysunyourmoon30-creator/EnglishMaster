using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Pronunciations.Dtos;

internal static class PronunciationInputValidator
{
    public static Result<PronunciationInput> Validate(
        Guid wordId,
        string? ipaUk,
        string? ipaUs,
        string? thaiReading,
        string? syllables,
        string? stressPattern,
        string? mouthPosition,
        string? tonguePosition,
        string? commonMistake,
        string? practiceNote,
        Guid? audioSlowMediaId,
        Guid? audioNormalMediaId,
        Guid? mouthImageMediaId,
        bool isActive)
    {
        var errors = new List<ValidationError>();

        if (wordId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(wordId), $"{nameof(wordId)} cannot be empty."));
        }

        var normalizedIpaUk = NormalizeOptional(ipaUk, nameof(ipaUk), PronunciationFieldLimits.IpaUk, errors);
        var normalizedIpaUs = NormalizeOptional(ipaUs, nameof(ipaUs), PronunciationFieldLimits.IpaUs, errors);
        if (normalizedIpaUk.Length == 0 && normalizedIpaUs.Length == 0)
        {
            errors.Add(new ValidationError(nameof(ipaUk), "IPA UK or IPA US is required."));
        }

        var normalizedThaiReading = NormalizeOptional(
            thaiReading,
            nameof(thaiReading),
            PronunciationFieldLimits.ThaiReading,
            errors);
        var normalizedSyllables = NormalizeOptional(
            syllables,
            nameof(syllables),
            PronunciationFieldLimits.Syllables,
            errors);
        var normalizedStressPattern = NormalizeOptional(
            stressPattern,
            nameof(stressPattern),
            PronunciationFieldLimits.StressPattern,
            errors);
        var normalizedMouthPosition = NormalizeOptional(
            mouthPosition,
            nameof(mouthPosition),
            PronunciationFieldLimits.MouthPosition,
            errors);
        var normalizedTonguePosition = NormalizeOptional(
            tonguePosition,
            nameof(tonguePosition),
            PronunciationFieldLimits.TonguePosition,
            errors);
        var normalizedCommonMistake = NormalizeOptional(
            commonMistake,
            nameof(commonMistake),
            PronunciationFieldLimits.CommonMistake,
            errors);
        var normalizedPracticeNote = NormalizeOptional(
            practiceNote,
            nameof(practiceNote),
            PronunciationFieldLimits.PracticeNote,
            errors);
        var normalizedAudioSlowMediaId = NormalizeOptionalId(audioSlowMediaId, nameof(audioSlowMediaId), errors);
        var normalizedAudioNormalMediaId = NormalizeOptionalId(audioNormalMediaId, nameof(audioNormalMediaId), errors);
        var normalizedMouthImageMediaId = NormalizeOptionalId(mouthImageMediaId, nameof(mouthImageMediaId), errors);

        if (errors.Count > 0)
        {
            return Result<PronunciationInput>.Validation([.. errors]);
        }

        return Result<PronunciationInput>.Success(new PronunciationInput(
            wordId,
            normalizedIpaUk,
            normalizedIpaUs,
            normalizedThaiReading,
            normalizedSyllables,
            normalizedStressPattern,
            normalizedMouthPosition,
            normalizedTonguePosition,
            normalizedCommonMistake,
            normalizedPracticeNote,
            normalizedAudioSlowMediaId,
            normalizedAudioNormalMediaId,
            normalizedMouthImageMediaId,
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
