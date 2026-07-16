using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.Words.Dtos;

public sealed record WordSearchCriteria(
    string? SearchTerm,
    PartOfSpeech? PartOfSpeech,
    CefrLevel? CefrLevel,
    bool? IsActive,
    Guid? CategoryId,
    Guid? TagId,
    int PageNumber,
    int PageSize,
    WordSortBy SortBy,
    WordSortDirection SortDirection);

public sealed record WordSearchResult(
    IReadOnlyCollection<Word> Items,
    int TotalCount);

public enum WordSortBy
{
    Text = 0,
    CreatedAt = 1
}

public enum WordSortDirection
{
    Asc = 0,
    Desc = 1
}
