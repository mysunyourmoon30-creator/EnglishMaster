using EnglishMaster.Application.Features.ContentRevisionRestores.Dtos;
using EnglishMaster.Application.Features.ContentRevisions;
using EnglishMaster.Domain.ContentRevisions;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.ContentRevisionRestores.Commands;

public sealed record CreateContentRevisionRestoreRequestCommand(Guid ContentRevisionId, string RequestedBy, string Reason);
public sealed record ApproveContentRevisionRestoreRequestCommand(Guid Id, string ReviewedBy, string? ReviewNote);
public sealed record RejectContentRevisionRestoreRequestCommand(Guid Id, string ReviewedBy, string? ReviewNote);
public sealed record CompleteContentRevisionRestoreRequestCommand(Guid Id);

public sealed class ContentRevisionRestoreCommandHandler
{
    private readonly IContentRevisionRepository repository;
    private readonly TimeProvider timeProvider;

    public ContentRevisionRestoreCommandHandler(IContentRevisionRepository repository, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<ContentRevisionRestoreRequestDto>> CreateAsync(CreateContentRevisionRestoreRequestCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (await repository.GetRevisionAsync(command.ContentRevisionId, cancellationToken) is null)
            {
                return Result<ContentRevisionRestoreRequestDto>.NotFound(nameof(command.ContentRevisionId), "Content revision was not found.");
            }

            var now = timeProvider.GetUtcNow();
            var request = ContentRevisionRestoreRequest.Create(command.ContentRevisionId, command.RequestedBy, now, command.Reason, now);
            return Result<ContentRevisionRestoreRequestDto>.Success(await repository.AddRestoreRequestAsync(request, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return Result<ContentRevisionRestoreRequestDto>.Validation(new ValidationError(exception.ParamName ?? "restoreRequest", exception.Message));
        }
    }

    public async Task<Result<ContentRevisionRestoreRequestDto>> ApproveAsync(ApproveContentRevisionRestoreRequestCommand command, CancellationToken cancellationToken) =>
        await ReviewAsync(command.Id, command.ReviewedBy, command.ReviewNote, approve: true, cancellationToken);

    public async Task<Result<ContentRevisionRestoreRequestDto>> RejectAsync(RejectContentRevisionRestoreRequestCommand command, CancellationToken cancellationToken) =>
        await ReviewAsync(command.Id, command.ReviewedBy, command.ReviewNote, approve: false, cancellationToken);

    public async Task<Result<ContentRevisionRestoreRequestDto>> CompleteAsync(CompleteContentRevisionRestoreRequestCommand command, CancellationToken cancellationToken)
    {
        var request = await repository.GetRestoreRequestEntityAsync(command.Id, cancellationToken);
        if (request is null)
        {
            return Result<ContentRevisionRestoreRequestDto>.NotFound(nameof(command.Id), "Restore request was not found.");
        }

        try
        {
            request.Complete(timeProvider.GetUtcNow());
            return Result<ContentRevisionRestoreRequestDto>.Success(await repository.SaveRestoreRequestAsync(request, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return Result<ContentRevisionRestoreRequestDto>.Validation(new ValidationError(nameof(command.Id), exception.Message));
        }
    }

    private async Task<Result<ContentRevisionRestoreRequestDto>> ReviewAsync(Guid id, string reviewedBy, string? note, bool approve, CancellationToken cancellationToken)
    {
        var request = await repository.GetRestoreRequestEntityAsync(id, cancellationToken);
        if (request is null)
        {
            return Result<ContentRevisionRestoreRequestDto>.NotFound(nameof(id), "Restore request was not found.");
        }

        var now = timeProvider.GetUtcNow();
        if (approve)
        {
            try
            {
                request.Approve(reviewedBy, note, now);
            }
            catch (InvalidOperationException exception)
            {
                return Result<ContentRevisionRestoreRequestDto>.Validation(new ValidationError(nameof(id), exception.Message));
            }
        }
        else
        {
            try
            {
                request.Reject(reviewedBy, note, now);
            }
            catch (InvalidOperationException exception)
            {
                return Result<ContentRevisionRestoreRequestDto>.Validation(new ValidationError(nameof(id), exception.Message));
            }
        }

        return Result<ContentRevisionRestoreRequestDto>.Success(await repository.SaveRestoreRequestAsync(request, cancellationToken));
    }
}
