using EnglishMaster.Contracts.PublicSearch;

namespace EnglishMaster.Web.Services.PublicSearch;

public interface IPublicSearchApiClient
{
    Task<PublicSearchResponse> SearchAsync(
        string? query,
        string? contentType,
        string? cefrLevel,
        Guid? categoryId,
        Guid? tagId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<PublicSearchFiltersResponse> GetFiltersAsync(CancellationToken cancellationToken);
}

