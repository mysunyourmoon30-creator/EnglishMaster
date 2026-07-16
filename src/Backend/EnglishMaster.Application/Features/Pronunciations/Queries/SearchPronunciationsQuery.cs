namespace EnglishMaster.Application.Features.Pronunciations.Queries;

public sealed record SearchPronunciationsQuery(
    string? Search,
    Guid? WordId,
    bool? IsActive,
    int? PageNumber = null,
    int? PageSize = null);
