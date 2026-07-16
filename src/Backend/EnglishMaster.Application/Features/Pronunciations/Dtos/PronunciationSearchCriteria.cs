using EnglishMaster.Domain.Pronunciations;

namespace EnglishMaster.Application.Features.Pronunciations.Dtos;

public sealed record PronunciationSearchCriteria(
    string? SearchTerm,
    Guid? WordId,
    bool? IsActive,
    int PageNumber,
    int PageSize);

public sealed record PronunciationSearchResult(
    IReadOnlyCollection<Pronunciation> Items,
    int TotalCount);
