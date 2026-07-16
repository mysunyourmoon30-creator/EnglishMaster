namespace EnglishMaster.Contracts.MinimalPairs;

public sealed record CreateMinimalPairRequest(
    string PairWordText,
    string? PairIpa,
    string? PairThaiReading,
    string? DifferenceNote,
    Guid? AudioMediaId,
    int SortOrder = 0);
