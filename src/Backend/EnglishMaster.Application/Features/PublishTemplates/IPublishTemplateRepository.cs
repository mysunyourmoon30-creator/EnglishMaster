using EnglishMaster.Application.Features.PublishTemplates.Dtos;
using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.Application.Features.PublishTemplates;

public interface IPublishTemplateRepository
{
    Task AddAsync(PublishTemplate template, CancellationToken cancellationToken);

    Task<PublishTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(string slug, Guid? excludedTemplateId, CancellationToken cancellationToken);

    Task<bool> DefaultExistsAsync(PublishFormat format, Guid? excludedTemplateId, CancellationToken cancellationToken);

    Task<PublishTemplateSearchResult> SearchAsync(PublishTemplateSearchCriteria criteria, CancellationToken cancellationToken);

    void Remove(PublishTemplate template);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
