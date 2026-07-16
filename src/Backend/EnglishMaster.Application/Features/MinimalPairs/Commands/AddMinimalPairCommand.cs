namespace EnglishMaster.Application.Features.MinimalPairs.Commands;

public sealed record AddMinimalPairCommand(
    Guid PronunciationId,
    string PairWordText,
    string? PairIpa,
    string? PairThaiReading,
    string? DifferenceNote,
    Guid? AudioMediaId,
    int SortOrder = 0);
