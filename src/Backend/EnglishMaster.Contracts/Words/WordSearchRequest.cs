namespace EnglishMaster.Contracts.Words;

public sealed record WordSearchRequest(
    string? Search = null,
    string? CefrLevel = null,
    string? PartOfSpeech = null,
    bool? IsActive = true,
    Guid? CategoryId = null,
    Guid? TagId = null,
    int PageNumber = 1,
    int PageSize = 20,
    string? SortBy = "Text",
    string? SortDirection = "Asc");
