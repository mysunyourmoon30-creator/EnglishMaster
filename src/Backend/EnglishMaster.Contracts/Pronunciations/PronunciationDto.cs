using EnglishMaster.Contracts.MinimalPairs;

namespace EnglishMaster.Contracts.Pronunciations;

public sealed record PronunciationDto(
    Guid Id,
    Guid WordId,
    PronunciationWordDto? Word,
    string IpaUk,
    string IpaUs,
    string ThaiReading,
    string Syllables,
    string StressPattern,
    string MouthPosition,
    string TonguePosition,
    string CommonMistake,
    string PracticeNote,
    Guid? AudioSlowMediaId,
    PronunciationMediaDto? AudioSlowMedia,
    Guid? AudioNormalMediaId,
    PronunciationMediaDto? AudioNormalMedia,
    Guid? MouthImageMediaId,
    PronunciationMediaDto? MouthImageMedia,
    IReadOnlyCollection<MinimalPairDto> MinimalPairs,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record PronunciationWordDto(
    Guid Id,
    string Text,
    string Slug);

public sealed record PronunciationMediaDto(
    Guid Id,
    string FileName,
    string ContentType,
    string MediaType,
    string PublicUrl,
    string AltText);
