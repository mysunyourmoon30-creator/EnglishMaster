using EnglishMaster.Contracts.Media;
using EnglishMaster.Application.Features.Media.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Media.Commands;

public sealed class ActivateMediaCommandHandler
{
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public ActivateMediaCommandHandler(IMediaRepository mediaRepository, TimeProvider timeProvider)
    {
        this.mediaRepository = mediaRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<MediaDto>> HandleAsync(
        ActivateMediaCommand command,
        CancellationToken cancellationToken)
    {
        var media = await mediaRepository.GetByIdAsync(command.Id, cancellationToken);
        if (media is null)
        {
            return Result<MediaDto>.NotFound(nameof(command.Id), "Media was not found.");
        }

        media.Activate(timeProvider.GetUtcNow());
        await mediaRepository.SaveChangesAsync(cancellationToken);

        return Result<MediaDto>.Success(MediaMapper.ToDto(media));
    }
}
