using EnglishMaster.Application.Features.Media.Dtos;

namespace EnglishMaster.Application.Features.Media;

public interface IMediaRepository
{
    Task AddAsync(Domain.Media.Media media, CancellationToken cancellationToken);

    Task<Domain.Media.Media?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Domain.Media.Media>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);

    Task<MediaSearchResult> SearchAsync(
        MediaSearchCriteria criteria,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
