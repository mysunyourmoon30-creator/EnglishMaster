namespace EnglishMaster.Application.Features.MinimalPairs.Commands;

public sealed record UpdateMinimalPairCommand(
    Guid Id,
    string PairWordText,
    string? PairIpa,
    string? PairThaiReading,
    string? DifferenceNote,
    Guid? AudioMediaId,
    int SortOrder,
    bool IsActive);
