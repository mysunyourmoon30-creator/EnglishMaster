using EnglishMaster.Contracts.Words;

namespace EnglishMaster.Web.Services.Words;

public interface IWordsApiClient
{
    Task<WordSearchResponse> SearchAsync(WordSearchRequest request, CancellationToken cancellationToken);

    Task<WordDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<WordDto> CreateAsync(CreateWordRequest request, CancellationToken cancellationToken);

    Task<WordDto> UpdateAsync(Guid id, UpdateWordRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
