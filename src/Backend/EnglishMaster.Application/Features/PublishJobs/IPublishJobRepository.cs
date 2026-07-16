using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.Application.Features.PublishJobs;

public interface IPublishJobRepository
{
    Task AddAsync(PublishJob publishJob, CancellationToken cancellationToken);

    Task<PublishJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PublishJobSearchResult> SearchAsync(PublishJobSearchCriteria criteria, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
