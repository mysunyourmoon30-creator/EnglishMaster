namespace EnglishMaster.Application.Features.Words.Commands;

public sealed record UpdateWordCommand(
    Guid Id,
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
