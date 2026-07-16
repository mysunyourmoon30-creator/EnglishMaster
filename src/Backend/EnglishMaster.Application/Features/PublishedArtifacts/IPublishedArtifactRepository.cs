using EnglishMaster.Application.Features.PublishedArtifacts.Dtos;
using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.Application.Features.PublishedArtifacts;

public interface IPublishedArtifactRepository
{
    Task AddAsync(PublishedArtifact artifact, CancellationToken cancellationToken);

    Task<PublishedArtifact?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PublishedArtifact>> GetByPublishJobIdAsync(Guid publishJobId, CancellationToken cancellationToken);

    Task<PublishedArtifactSearchResult> SearchAsync(PublishedArtifactSearchCriteria criteria, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
