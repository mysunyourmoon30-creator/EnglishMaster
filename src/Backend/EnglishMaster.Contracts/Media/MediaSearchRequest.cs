namespace EnglishMaster.Contracts.Media;

public sealed record MediaSearchRequest(
    string? Search = null,
    string? MediaType = null,
    string? ContentType = null,
    bool? IsActive = true,
    int PageNumber = 1,
    int PageSize = 20);
