namespace EnglishMaster.Application.Features.Media.Queries;

public sealed record SearchMediaQuery(
    string? Search,
    string? MediaType,
    string? ContentType,
    bool? IsActive,
    int? PageNumber = null,
    int? PageSize = null);
