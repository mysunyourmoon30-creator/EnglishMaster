using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.Words.Dtos;

internal sealed record WordInput(
    string Text,
    string Slug,
    string IpaUk,
    string IpaUs,
    string ThaiReading,
    string MeaningTh,
    string MeaningEn,
    PartOfSpeech PartOfSpeech,
    CefrLevel CefrLevel,
    string ExampleEn,
    string ExampleTh,
    Guid? CategoryId,
    IReadOnlyCollection<Guid> TagIds,
    Guid? ImageMediaId,
    Guid? AudioMediaId,
    bool IsActive);
