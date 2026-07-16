using EnglishMaster.Contracts.BulkOperations;

namespace EnglishMaster.Web.Services.BulkOperations;

public interface IBulkOperationApiClient
{
    Task<BulkOperationSearchResponse> SearchAsync(string? operationType, string? contentType, string? status, CancellationToken cancellationToken);

    Task<BulkOperationDto> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<BulkOperationItemDto>> GetItemsAsync(Guid id, CancellationToken cancellationToken);

    Task<BulkOperationDto> CreateAsync(CreateBulkOperationRequest request, CancellationToken cancellationToken);

    Task<BulkOperationDto> RunAsync(Guid id, CancellationToken cancellationToken);

    Task<BulkOperationDto> CancelAsync(Guid id, CancellationToken cancellationToken);
}
