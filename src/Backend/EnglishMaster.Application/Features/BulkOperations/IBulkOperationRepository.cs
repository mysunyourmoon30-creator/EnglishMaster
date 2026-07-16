using EnglishMaster.Application.Features.BulkOperations.Dtos;
using EnglishMaster.Domain.BulkOperations;

namespace EnglishMaster.Application.Features.BulkOperations;

public interface IBulkOperationRepository
{
    Task<BulkOperationDto> AddAsync(BulkOperation operation, CancellationToken cancellationToken);

    Task<BulkOperation?> GetEntityAsync(Guid id, CancellationToken cancellationToken);

    Task<BulkOperationDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<BulkOperationDto> SaveAsync(BulkOperation operation, CancellationToken cancellationToken);

    Task<BulkOperationSearchResponse> SearchAsync(string? operationType, string? contentType, string? status, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<BulkOperationItemDto>> GetItemsAsync(Guid operationId, CancellationToken cancellationToken);
}

public interface IBulkOperationRunner
{
    Task RunAsync(BulkOperation operation, CancellationToken cancellationToken);
}
