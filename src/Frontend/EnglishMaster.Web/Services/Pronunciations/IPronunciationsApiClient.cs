using EnglishMaster.Contracts.MinimalPairs;
using EnglishMaster.Contracts.Pronunciations;

namespace EnglishMaster.Web.Services.Pronunciations;

public interface IPronunciationsApiClient
{
    Task<PronunciationSearchResponse> SearchAsync(
        PronunciationSearchRequest request,
        CancellationToken cancellationToken);

    Task<PronunciationDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<PronunciationDto?> GetByWordIdAsync(Guid wordId, CancellationToken cancellationToken);

    Task<PronunciationDto> CreateAsync(
        CreatePronunciationRequest request,
        CancellationToken cancellationToken);

    Task<PronunciationDto> UpdateAsync(
        Guid id,
        UpdatePronunciationRequest request,
        CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<MinimalPairDto>> GetMinimalPairsAsync(
        Guid pronunciationId,
        CancellationToken cancellationToken);

    Task<MinimalPairDto> AddMinimalPairAsync(
        Guid pronunciationId,
        CreateMinimalPairRequest request,
        CancellationToken cancellationToken);

    Task<MinimalPairDto> UpdateMinimalPairAsync(
        Guid id,
        UpdateMinimalPairRequest request,
        CancellationToken cancellationToken);

    Task DeleteMinimalPairAsync(Guid id, CancellationToken cancellationToken);
}
