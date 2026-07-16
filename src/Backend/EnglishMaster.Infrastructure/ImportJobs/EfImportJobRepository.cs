using EnglishMaster.Application.Features.ImportJobs;
using EnglishMaster.Application.Features.ImportJobs.Dtos;
using EnglishMaster.Domain.ImportJobs;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.ImportJobs;

public sealed class EfImportJobRepository : IImportJobRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfImportJobRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<ImportJobDto> AddAsync(ImportJob job, CancellationToken cancellationToken)
    {
        dbContext.ImportJobs.Add(job);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(job);
    }

    public async Task<ImportJob?> GetEntityAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.ImportJobs.Include(job => job.Rows).ThenInclude(row => row.ValidationErrors).SingleOrDefaultAsync(job => job.Id == id, cancellationToken);

    public async Task<ImportJobDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var job = await dbContext.ImportJobs.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return job is null ? null : ToDto(job);
    }

    public async Task<ImportJobDto> SaveAsync(ImportJob job, CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(job);
    }

    public async Task<ImportJobSearchResponse> SearchAsync(string? importType, string? format, string? status, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.ImportJobs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(importType))
        {
            var normalized = Normalize(importType);
            query = query.Where(job => job.ImportType == normalized);
        }

        if (!string.IsNullOrWhiteSpace(format))
        {
            query = query.Where(job => job.Format == format.Trim().ToUpper());
        }

        if (Enum.TryParse<ImportJobStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            query = query.Where(job => job.Status == parsedStatus);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(job => job.RequestedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArrayAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new ImportJobSearchResponse(items.Select(ToDto).ToArray(), pageNumber, pageSize, total, totalPages, pageNumber > 1, pageNumber < totalPages);
    }

    public async Task<IReadOnlyCollection<ImportJobRowDto>> GetRowsAsync(Guid importJobId, CancellationToken cancellationToken)
    {
        var rows = await dbContext.ImportJobRows.AsNoTracking().Where(row => row.ImportJobId == importJobId).OrderBy(row => row.RowNumber).ToArrayAsync(cancellationToken);
        return rows.Select(ToRowDto).ToArray();
    }

    public async Task<IReadOnlyCollection<ImportValidationErrorDto>> GetErrorsAsync(Guid importJobId, CancellationToken cancellationToken)
    {
        var errors = await dbContext.ImportValidationErrors.AsNoTracking()
            .Join(dbContext.ImportJobRows.AsNoTracking().Where(row => row.ImportJobId == importJobId), error => error.ImportJobRowId, row => row.Id, (error, _) => error)
            .OrderBy(error => error.CreatedAt)
            .ToArrayAsync(cancellationToken);
        return errors.Select(ToErrorDto).ToArray();
    }

    private static ImportJobDto ToDto(ImportJob job) =>
        new(job.Id, job.ImportType, job.Format, job.Status.ToString(), job.FileName, job.OriginalFileName, job.FileSize, job.RequestedBy, job.RequestedAt, job.ValidatedAt, job.ConfirmedAt, job.CompletedAt, job.FailedAt, job.RolledBackAt, job.TotalRows, job.ValidRows, job.InvalidRows, job.ImportedRows, job.FailedRows, job.ErrorMessage, job.CreatedAt, job.UpdatedAt);

    private static ImportJobRowDto ToRowDto(ImportJobRow row) =>
        new(row.Id, row.ImportJobId, row.RowNumber, row.RawDataJson, row.ParsedDataJson, row.Status.ToString(), row.ErrorMessage, row.CreatedEntityType, row.CreatedEntityId, row.UpdatedEntityType, row.UpdatedEntityId, row.CreatedAt, row.UpdatedAt);

    private static ImportValidationErrorDto ToErrorDto(ImportValidationError error) =>
        new(error.Id, error.ImportJobRowId, error.FieldName, error.ErrorCode, error.ErrorMessage, error.Severity.ToString(), error.CreatedAt, error.UpdatedAt);

    private static string Normalize(string value) => value.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().ToLowerInvariant();
}
