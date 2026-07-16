namespace EnglishMaster.Application.Features.Media.Dtos;

public sealed record MediaSearchCriteria(
    string? SearchTerm,
    Domain.Media.MediaType? MediaType,
    string? ContentType,
    bool? IsActive,
    int PageNumber,
    int PageSize);

public sealed record MediaSearchResult(
    IReadOnlyCollection<Domain.Media.Media> Items,
    int TotalCount);
