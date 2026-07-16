using EnglishMaster.Application.Features.BulkOperations;
using EnglishMaster.Application.Features.BulkOperations.Dtos;
using EnglishMaster.Domain.BulkOperations;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.BulkOperations;

public sealed class EfBulkOperationRepository : IBulkOperationRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfBulkOperationRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<BulkOperationDto> AddAsync(BulkOperation operation, CancellationToken cancellationToken)
    {
        dbContext.BulkOperations.Add(operation);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(operation);
    }

    public async Task<BulkOperation?> GetEntityAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.BulkOperations.Include(operation => operation.Items).SingleOrDefaultAsync(operation => operation.Id == id, cancellationToken);

    public async Task<BulkOperationDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var operation = await dbContext.BulkOperations.AsNoTracking().Include(item => item.Items).SingleOrDefaultAsync(operation => operation.Id == id, cancellationToken);
        return operation is null ? null : ToDto(operation);
    }

    public async Task<BulkOperationDto> SaveAsync(BulkOperation operation, CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(operation);
    }

    public async Task<BulkOperationSearchResponse> SearchAsync(string? operationType, string? contentType, string? status, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.BulkOperations.AsNoTracking();
        if (Enum.TryParse<BulkOperationType>(operationType, ignoreCase: true, out var parsedOperationType))
        {
            query = query.Where(operation => operation.OperationType == parsedOperationType);
        }

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            var normalizedType = Normalize(contentType);
            query = query.Where(operation => operation.ContentType == normalizedType);
        }

        if (Enum.TryParse<BulkOperationStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            query = query.Where(operation => operation.Status == parsedStatus);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(operation => operation.RequestedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new BulkOperationSearchResponse(items.Select(ToDto).ToArray(), pageNumber, pageSize, total, totalPages, pageNumber > 1, pageNumber < totalPages);
    }

    public async Task<IReadOnlyCollection<BulkOperationItemDto>> GetItemsAsync(Guid operationId, CancellationToken cancellationToken)
    {
        var items = await dbContext.BulkOperationItems.AsNoTracking()
            .Where(item => item.BulkOperationId == operationId)
            .OrderBy(item => item.CreatedAt)
            .ToArrayAsync(cancellationToken);
        return items.Select(ToItemDto).ToArray();
    }

    private static BulkOperationDto ToDto(BulkOperation operation) =>
        new(
            operation.Id,
            operation.OperationType.ToString(),
            operation.ContentType,
            operation.Status.ToString(),
            operation.RequestedBy,
            operation.RequestedAt,
            operation.StartedAt,
            operation.CompletedAt,
            operation.TotalItems,
            operation.SucceededItems,
            operation.FailedItems,
            operation.ErrorMessage,
            operation.Note,
            operation.CategoryId,
            ParseTagIds(operation.TagIds),
            operation.ExportFormat,
            operation.CreatedAt,
            operation.UpdatedAt);

    private static BulkOperationItemDto ToItemDto(BulkOperationItem item) =>
        new(item.Id, item.BulkOperationId, item.ContentId, item.Status.ToString(), item.ErrorMessage, item.CreatedAt, item.UpdatedAt);

    private static Guid[] ParseTagIds(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Guid.Parse).ToArray();

    private static string Normalize(string value) => value.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().ToLowerInvariant();
}
