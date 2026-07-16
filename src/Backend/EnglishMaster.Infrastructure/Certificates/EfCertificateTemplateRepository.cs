using EnglishMaster.Application.Features.Certificates;
using EnglishMaster.Domain.Certificates;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Certificates;

public sealed class EfCertificateTemplateRepository : ICertificateTemplateRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfCertificateTemplateRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<CertificateTemplateDto> AddAsync(CertificateTemplate template, CancellationToken cancellationToken)
    {
        dbContext.CertificateTemplates.Add(template);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(template);
    }

    public async Task<CertificateTemplate?> GetEntityByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.CertificateTemplates.SingleOrDefaultAsync(template => template.Id == id, cancellationToken);

    public async Task<CertificateTemplateDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var template = await dbContext.CertificateTemplates.AsNoTracking().SingleOrDefaultAsync(template => template.Id == id, cancellationToken);
        return template is null ? null : ToDto(template);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludedId, CancellationToken cancellationToken)
    {
        var query = dbContext.CertificateTemplates.AsNoTracking().Where(template => template.Code == code);
        if (excludedId.HasValue)
        {
            query = query.Where(template => template.Id != excludedId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<CertificateTemplateSearchResponse> SearchAsync(string? search, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.CertificateTemplates.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(template => template.Code.ToLower().Contains(term) || template.Name.ToLower().Contains(term));
        }

        if (isActive.HasValue)
        {
            query = query.Where(template => template.IsActive == isActive.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(template => template.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new CertificateTemplateSearchResponse(items.Select(ToDto).ToArray(), pageNumber, pageSize, total, totalPages, pageNumber > 1, pageNumber < totalPages);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);

    private static CertificateTemplateDto ToDto(CertificateTemplate template) =>
        new(template.Id, template.Code, template.Name, template.Description, template.BodyTemplate, template.IsActive, template.CreatedAt, template.UpdatedAt);
}
