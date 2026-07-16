using EnglishMaster.Contracts.Tags;

namespace EnglishMaster.Web.Services.Tags;

public interface ITagsApiClient
{
    Task<TagSearchResponse> SearchAsync(
        string? search,
        bool? isActive,
        CancellationToken cancellationToken);

    Task<TagDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<TagDto> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken);

    Task<TagDto> UpdateAsync(Guid id, UpdateTagRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
