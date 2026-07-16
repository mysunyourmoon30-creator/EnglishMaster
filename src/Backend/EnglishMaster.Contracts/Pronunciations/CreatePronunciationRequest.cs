namespace EnglishMaster.Contracts.Pronunciations;

public sealed record CreatePronunciationRequest(
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
    Guid? AudioSlowMediaId = null,
    Guid? AudioNormalMediaId = null,
    Guid? MouthImageMediaId = null);
