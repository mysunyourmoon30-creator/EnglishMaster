namespace EnglishMaster.Contracts.Pronunciations;

public sealed record UpdatePronunciationRequest(
    Guid WordId,
    string? IpaUk,
    string? IpaUs,
    string? ThaiReading,
    string? Syllables,
    string? StressPattern,
    string? MouthPosition,
    string? TonguePosition,
    string? CommonMistake,
    string? PracticeNote,
    Guid? AudioSlowMediaId,
    Guid? AudioNormalMediaId,
    Guid? MouthImageMediaId,
    bool IsActive);
