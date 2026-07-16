using EnglishMaster.Application.Features.BulkOperations.Dtos;
using EnglishMaster.Domain.BulkOperations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BulkOperations.Commands;

public sealed record CreateBulkOperationCommand(string OperationType, string ContentType, IReadOnlyCollection<Guid> ContentIds, string? RequestedBy, string? Note, Guid? CategoryId, IReadOnlyCollection<Guid>? TagIds, string? ExportFormat);
public sealed record RunBulkOperationCommand(Guid Id);
public sealed record CancelBulkOperationCommand(Guid Id);

public sealed class BulkOperationCommandHandler
{
    private readonly IBulkOperationRepository repository;
    private readonly IBulkOperationRunner runner;
    private readonly TimeProvider timeProvider;

    public BulkOperationCommandHandler(IBulkOperationRepository repository, IBulkOperationRunner runner, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.runner = runner;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<BulkOperationDto>> CreateAsync(CreateBulkOperationCommand command, CancellationToken cancellationToken)
    {
        if (command.ContentIds.Count == 0)
        {
            return Result<BulkOperationDto>.Validation(new ValidationError(nameof(command.ContentIds), "At least one content item is required."));
        }

        if (!Enum.TryParse<BulkOperationType>(command.OperationType, ignoreCase: true, out var operationType))
        {
            return Result<BulkOperationDto>.Validation(new ValidationError(nameof(command.OperationType), "OperationType is invalid."));
        }

        try
        {
            var operation = BulkOperation.Create(operationType, Normalize(command.ContentType), command.RequestedBy, command.ContentIds, command.Note, command.CategoryId, command.TagIds, command.ExportFormat, timeProvider.GetUtcNow());
            return Result<BulkOperationDto>.Success(await repository.AddAsync(operation, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return Result<BulkOperationDto>.Validation(new ValidationError(exception.ParamName ?? "bulkOperation", exception.Message));
        }
    }

    public async Task<Result<BulkOperationDto>> RunAsync(RunBulkOperationCommand command, CancellationToken cancellationToken)
    {
        var operation = await repository.GetEntityAsync(command.Id, cancellationToken);
        if (operation is null)
        {
            return Result<BulkOperationDto>.NotFound(nameof(command.Id), "Bulk operation was not found.");
        }

        try
        {
            operation.Start(timeProvider.GetUtcNow());
            await repository.SaveAsync(operation, cancellationToken);
            await runner.RunAsync(operation, cancellationToken);
            operation.Finish(timeProvider.GetUtcNow());
            return Result<BulkOperationDto>.Success(await repository.SaveAsync(operation, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return Result<BulkOperationDto>.Validation(new ValidationError(nameof(command.Id), exception.Message));
        }
    }

    public async Task<Result<BulkOperationDto>> CancelAsync(CancelBulkOperationCommand command, CancellationToken cancellationToken)
    {
        var operation = await repository.GetEntityAsync(command.Id, cancellationToken);
        if (operation is null)
        {
            return Result<BulkOperationDto>.NotFound(nameof(command.Id), "Bulk operation was not found.");
        }

        try
        {
            operation.Cancel(timeProvider.GetUtcNow());
            return Result<BulkOperationDto>.Success(await repository.SaveAsync(operation, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return Result<BulkOperationDto>.Validation(new ValidationError(nameof(command.Id), exception.Message));
        }
    }

    private static string Normalize(string value) => value.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().ToLowerInvariant();
}
