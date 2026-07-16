using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Media.Commands;

public sealed class DeleteMediaCommandHandler
{
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public DeleteMediaCommandHandler(IMediaRepository mediaRepository, TimeProvider timeProvider)
    {
        this.mediaRepository = mediaRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteMediaCommand command,
        CancellationToken cancellationToken)
    {
        var media = await mediaRepository.GetByIdAsync(command.Id, cancellationToken);
        if (media is null)
        {
            return Result.NotFound(nameof(command.Id), "Media was not found.");
        }

        media.Deactivate(timeProvider.GetUtcNow());
        await mediaRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
