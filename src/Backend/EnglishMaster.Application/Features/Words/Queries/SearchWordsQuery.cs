namespace EnglishMaster.Application.Features.Words.Queries;

public sealed record SearchWordsQuery(
    string? Search,
    string? PartOfSpeech,
    string? CefrLevel,
    bool? IsActive,
    int? PageNumber = null,
    int? PageSize = null,
    string? SortBy = null,
    string? SortDirection = null,
    Guid? CategoryId = null,
    Guid? TagId = null);
