namespace EnglishMaster.Contracts.Words;

public sealed record UpdateWordRequest(
    string Text,
    string? IpaUk,
    string? IpaUs,
    string? ThaiReading,
    string MeaningTh,
    string? MeaningEn,
    string PartOfSpeech,
    string CefrLevel,
    string? ExampleEn,
    string? ExampleTh,
    bool IsActive,
    Guid? CategoryId = null,
    IReadOnlyCollection<Guid>? TagIds = null,
    Guid? ImageMediaId = null,
    Guid? AudioMediaId = null);
