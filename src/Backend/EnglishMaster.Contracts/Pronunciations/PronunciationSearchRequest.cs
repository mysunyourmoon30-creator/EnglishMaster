namespace EnglishMaster.Contracts.Pronunciations;

public sealed record PronunciationSearchRequest(
    string? Search = null,
    Guid? WordId = null,
    bool? IsActive = true,
    int PageNumber = 1,
    int PageSize = 20);
