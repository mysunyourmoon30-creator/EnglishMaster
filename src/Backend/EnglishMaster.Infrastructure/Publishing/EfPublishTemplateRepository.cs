using EnglishMaster.Application.Features.PublishTemplates;
using EnglishMaster.Application.Features.PublishTemplates.Dtos;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Publishing;

internal sealed class EfPublishTemplateRepository : IPublishTemplateRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfPublishTemplateRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(PublishTemplate template, CancellationToken cancellationToken)
    {
        await dbContext.PublishTemplates.AddAsync(template, cancellationToken);
    }

    public Task<PublishTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.PublishTemplates.FirstOrDefaultAsync(template => template.Id == id, cancellationToken);
    }

    public Task<bool> SlugExistsAsync(string slug, Guid? excludedTemplateId, CancellationToken cancellationToken)
    {
        var query = dbContext.PublishTemplates.AsNoTracking().Where(template => template.Slug == slug);
        if (excludedTemplateId.HasValue)
        {
            query = query.Where(template => template.Id != excludedTemplateId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> DefaultExistsAsync(PublishFormat format, Guid? excludedTemplateId, CancellationToken cancellationToken)
    {
        var query = dbContext.PublishTemplates.AsNoTracking().Where(template => template.Format == format && template.IsDefault);
        if (excludedTemplateId.HasValue)
        {
            query = query.Where(template => template.Id != excludedTemplateId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<PublishTemplateSearchResult> SearchAsync(PublishTemplateSearchCriteria criteria, CancellationToken cancellationToken)
    {
        IQueryable<PublishTemplate> query = dbContext.PublishTemplates.AsNoTracking();

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

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new PublishTemplateSearchResult([], totalCount);
        }

        var items = await query
            .OrderBy(template => template.Format)
            .ThenBy(template => template.Name)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new PublishTemplateSearchResult(items, totalCount);
    }

    public void Remove(PublishTemplate template)
    {
        dbContext.PublishTemplates.Remove(template);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
