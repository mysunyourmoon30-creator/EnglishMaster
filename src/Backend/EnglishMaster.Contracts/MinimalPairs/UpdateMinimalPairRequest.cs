namespace EnglishMaster.Contracts.MinimalPairs;

public sealed record UpdateMinimalPairRequest(
    string PairWordText,
    string? PairIpa,
    string? PairThaiReading,
    string? DifferenceNote,
    Guid? AudioMediaId,
    int SortOrder,
    bool IsActive);
