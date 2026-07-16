using EnglishMaster.Application.Features.PublicSearch.Dtos;

namespace EnglishMaster.Application.Features.PublicSearch;

public interface IPublicSearchRepository
{
    Task<PublicSearchResponse> SearchAsync(PublicSearchCriteria criteria, CancellationToken cancellationToken);
    Task<PublicSearchFiltersResponse> GetFiltersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<string>> GetSuggestionsAsync(string? query, int count, CancellationToken cancellationToken);
}

