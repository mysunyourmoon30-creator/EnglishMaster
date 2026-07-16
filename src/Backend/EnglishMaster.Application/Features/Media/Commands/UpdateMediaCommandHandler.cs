using EnglishMaster.Application.Features.Media.Dtos;
using EnglishMaster.Contracts.Media;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Media.Commands;

public sealed class UpdateMediaCommandHandler
{
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public UpdateMediaCommandHandler(IMediaRepository mediaRepository, TimeProvider timeProvider)
    {
        this.mediaRepository = mediaRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<MediaDto>> HandleAsync(
        UpdateMediaCommand command,
        CancellationToken cancellationToken)
    {
        var media = await mediaRepository.GetByIdAsync(command.Id, cancellationToken);
        if (media is null)
        {
            return Result<MediaDto>.NotFound(nameof(command.Id), "Media was not found.");
        }

        var validation = MediaInputValidator.Validate(
            command.FileName,
            command.OriginalFileName,
            command.FileExtension,
            command.ContentType,
            command.FileSize,
            command.MediaType,
            media.StoragePath,
            command.PublicUrl,
            command.AltText,
            command.Description,
            command.IsActive);

        if (!validation.IsSuccess)
        {
            return Result<MediaDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        media.Update(
            input.FileName,
            input.OriginalFileName,
            input.FileExtension,
            input.ContentType,
            input.FileSize,
            input.MediaType,
            input.StoragePath,
            input.PublicUrl,
            input.AltText,
            input.Description,
            input.IsActive,
            timeProvider.GetUtcNow());

        await mediaRepository.SaveChangesAsync(cancellationToken);

        return Result<MediaDto>.Success(MediaMapper.ToDto(media));
    }
}
