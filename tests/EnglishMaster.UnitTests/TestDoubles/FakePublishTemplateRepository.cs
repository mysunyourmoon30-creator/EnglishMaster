using EnglishMaster.Application.Features.PublishTemplates;
using EnglishMaster.Application.Features.PublishTemplates.Dtos;
using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakePublishTemplateRepository : IPublishTemplateRepository
{
    public List<PublishTemplate> Templates { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(PublishTemplate template, CancellationToken cancellationToken)
    {
        Templates.Add(template);
        return Task.CompletedTask;
    }

    public Task<PublishTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Templates.SingleOrDefault(template => template.Id == id));
    }

    public Task<bool> SlugExistsAsync(string slug, Guid? excludedTemplateId, CancellationToken cancellationToken)
    {
        return Task.FromResult(Templates.Any(template => template.Slug == slug && (!excludedTemplateId.HasValue || template.Id != excludedTemplateId.Value)));
    }

    public Task<bool> DefaultExistsAsync(PublishFormat format, Guid? excludedTemplateId, CancellationToken cancellationToken)
    {
        return Task.FromResult(Templates.Any(template => template.Format == format && template.IsDefault && (!excludedTemplateId.HasValue || template.Id != excludedTemplateId.Value)));
    }

    public Task<PublishTemplateSearchResult> SearchAsync(PublishTemplateSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var query = Templates.AsEnumerable();

        if (criteria.Format.HasValue)
        {
            query = query.Where(template => template.Format == criteria.Format.Value);
        }

        if (criteria.IsDefault.HasValue)
        {
            query = query.Where(template => template.IsDefault == criteria.IsDefault.Value);
        }

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(template => template.IsActive == criteria.IsActive.Value);
        }

        var filtered = query.OrderBy(template => template.Name).ToArray();
        var items = filtered.Skip((criteria.PageNumber - 1) * criteria.PageSize).Take(criteria.PageSize).ToArray();

        return Task.FromResult(new PublishTemplateSearchResult(items, filtered.Length));
    }

    public void Remove(PublishTemplate template)
    {
        Templates.Remove(template);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
