using EnglishMaster.Application.Features.Pronunciations.Dtos;
using EnglishMaster.Domain.Pronunciations;

namespace EnglishMaster.Application.Features.Pronunciations;

public interface IPronunciationRepository
{
    Task AddAsync(Pronunciation pronunciation, CancellationToken cancellationToken);

    Task<Pronunciation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Pronunciation?> GetByWordIdAsync(Guid wordId, CancellationToken cancellationToken);

    Task<bool> WordHasPronunciationAsync(
        Guid wordId,
        Guid? excludedPronunciationId,
        CancellationToken cancellationToken);

    Task<PronunciationSearchResult> SearchAsync(
        PronunciationSearchCriteria criteria,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
