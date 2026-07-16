namespace EnglishMaster.Contracts.Words;

public sealed record WordDto(
    Guid Id,
    string Text,
    string Slug,
    string IpaUk,
    string IpaUs,
    string ThaiReading,
    string MeaningTh,
    string MeaningEn,
    string PartOfSpeech,
    string CefrLevel,
    string ExampleEn,
    string ExampleTh,
    Guid? CategoryId,
    WordCategoryDto? Category,
    IReadOnlyCollection<WordTagDto> Tags,
    Guid? ImageMediaId,
    WordMediaDto? ImageMedia,
    Guid? AudioMediaId,
    WordMediaDto? AudioMedia,
    WordPronunciationDto? Pronunciation,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record WordCategoryDto(
    Guid Id,
    string Name,
    string Slug);

public sealed record WordTagDto(
    Guid Id,
    string Name,
    string Slug);

public sealed record WordMediaDto(
    Guid Id,
    string FileName,
    string ContentType,
    string MediaType,
    string PublicUrl,
    string AltText);

public sealed record WordPronunciationDto(
    Guid Id,
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
    WordMediaDto? AudioSlowMedia,
    Guid? AudioNormalMediaId,
    WordMediaDto? AudioNormalMedia,
    Guid? MouthImageMediaId,
    WordMediaDto? MouthImageMedia,
    IReadOnlyCollection<WordMinimalPairDto> MinimalPairs,
    bool IsActive);

public sealed record WordMinimalPairDto(
    Guid Id,
    string PairWordText,
    string PairIpa,
    string PairThaiReading,
    string DifferenceNote,
    Guid? AudioMediaId,
    WordMediaDto? AudioMedia,
    int SortOrder,
    bool IsActive);
