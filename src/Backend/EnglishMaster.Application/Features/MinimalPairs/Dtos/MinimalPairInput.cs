namespace EnglishMaster.Application.Features.MinimalPairs.Dtos;

internal sealed record MinimalPairInput(
    string PairWordText,
    string PairIpa,
    string PairThaiReading,
    string DifferenceNote,
    Guid? AudioMediaId,
    int SortOrder,
    bool IsActive);
