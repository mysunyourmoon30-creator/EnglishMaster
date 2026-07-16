namespace EnglishMaster.Contracts.Publishing;

public sealed record PublishJobSearchRequest(
    string? SourceType = null,
    Guid? SourceId = null,
    string? Format = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 20,
    string SortBy = "CreatedAt",
    string SortDirection = "Desc");
