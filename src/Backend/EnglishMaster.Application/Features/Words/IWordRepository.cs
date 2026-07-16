using EnglishMaster.Domain.Words;
using EnglishMaster.Application.Features.Words.Dtos;

namespace EnglishMaster.Application.Features.Words;

public interface IWordRepository
{
    Task AddAsync(Word word, CancellationToken cancellationToken);

    Task<Word?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Word>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedWordId,
        CancellationToken cancellationToken);

    Task<WordSearchResult> SearchAsync(WordSearchCriteria criteria, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
