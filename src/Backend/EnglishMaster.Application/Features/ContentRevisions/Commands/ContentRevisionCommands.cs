using EnglishMaster.Application.Features.ContentRevisions.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.ContentRevisions.Commands;

public sealed record CreateContentRevisionCommand(string ContentType, Guid ContentId, string EventType, string? Title, string? Summary, string? ChangedBy, string? ChangeReason, string SnapshotJson, string? DiffJson);

public sealed class ContentRevisionCommandHandler
{
    private readonly IContentRevisionService revisionService;

    public ContentRevisionCommandHandler(IContentRevisionService revisionService)
    {
        this.revisionService = revisionService;
    }

    public async Task<Result<ContentRevisionDto>> CreateAsync(CreateContentRevisionCommand command, CancellationToken cancellationToken)
    {
        try
        {
            return Result<ContentRevisionDto>.Success(await revisionService.CreateAsync(command.ContentType, command.ContentId, command.EventType, command.Title, command.Summary, command.ChangedBy, command.ChangeReason, command.SnapshotJson, command.DiffJson, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return Result<ContentRevisionDto>.Validation(new ValidationError(exception.ParamName ?? "revision", exception.Message));
        }
    }
}
