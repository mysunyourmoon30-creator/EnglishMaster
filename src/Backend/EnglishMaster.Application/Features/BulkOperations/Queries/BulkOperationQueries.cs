using EnglishMaster.Application.Features.BulkOperations.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BulkOperations.Queries;

public sealed record GetBulkOperationByIdQuery(Guid Id);
public sealed record SearchBulkOperationsQuery(string? OperationType, string? ContentType, string? Status, int? PageNumber, int? PageSize);
public sealed record GetBulkOperationItemsQuery(Guid Id);

public sealed class BulkOperationQueryHandler
{
    private readonly IBulkOperationRepository repository;

    public BulkOperationQueryHandler(IBulkOperationRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<BulkOperationDto>> GetAsync(GetBulkOperationByIdQuery query, CancellationToken cancellationToken)
    {
        var operation = await repository.GetAsync(query.Id, cancellationToken);
        return operation is null
            ? Result<BulkOperationDto>.NotFound(nameof(query.Id), "Bulk operation was not found.")
            : Result<BulkOperationDto>.Success(operation);
    }

    public async Task<Result<BulkOperationSearchResponse>> SearchAsync(SearchBulkOperationsQuery query, CancellationToken cancellationToken) =>
        Result<BulkOperationSearchResponse>.Success(await repository.SearchAsync(query.OperationType, query.ContentType, query.Status, Math.Max(query.PageNumber ?? 1, 1), Math.Clamp(query.PageSize ?? 20, 1, 100), cancellationToken));

    public async Task<Result<IReadOnlyCollection<BulkOperationItemDto>>> GetItemsAsync(GetBulkOperationItemsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<BulkOperationItemDto>>.Success(await repository.GetItemsAsync(query.Id, cancellationToken));
}
