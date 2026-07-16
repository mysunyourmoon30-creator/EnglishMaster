using EnglishMaster.Application.Features.Media.Dtos;
using EnglishMaster.Contracts.Media;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Media.Commands;

public sealed class CreateMediaCommandHandler
{
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public CreateMediaCommandHandler(IMediaRepository mediaRepository, TimeProvider timeProvider)
    {
        this.mediaRepository = mediaRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<MediaDto>> HandleAsync(
        CreateMediaCommand command,
        CancellationToken cancellationToken)
    {
        var validation = MediaInputValidator.Validate(
            command.FileName,
            command.OriginalFileName,
            command.FileExtension,
            command.ContentType,
            command.FileSize,
            command.MediaType,
            BuildStoragePath(command.FileName),
            command.PublicUrl,
            command.AltText,
            command.Description,
            isActive: true);

        if (!validation.IsSuccess)
        {
            return Result<MediaDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var media = Domain.Media.Media.Create(
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
            timeProvider.GetUtcNow());

        await mediaRepository.AddAsync(media, cancellationToken);
        await mediaRepository.SaveChangesAsync(cancellationToken);

        return Result<MediaDto>.Success(MediaMapper.ToDto(media));
    }

    private static string BuildStoragePath(string? fileName)
    {
        return $"media/{fileName?.Trim() ?? string.Empty}";
    }
}
