namespace EnglishMaster.Contracts.MinimalPairs;

public sealed record MinimalPairDto(
    Guid Id,
    Guid PronunciationId,
    string PairWordText,
    string PairIpa,
    string PairThaiReading,
    string DifferenceNote,
    Guid? AudioMediaId,
    MinimalPairMediaDto? AudioMedia,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record MinimalPairMediaDto(
    Guid Id,
    string FileName,
    string ContentType,
    string MediaType,
    string PublicUrl,
    string AltText);
