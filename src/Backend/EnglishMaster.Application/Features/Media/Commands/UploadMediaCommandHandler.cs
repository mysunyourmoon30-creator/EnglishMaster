using EnglishMaster.Application.Features.Media.Dtos;
using EnglishMaster.Contracts.Media;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Media.Commands;

public sealed class UploadMediaCommandHandler
{
    private readonly IMediaRepository mediaRepository;
    private readonly IMediaStorageService storageService;
    private readonly TimeProvider timeProvider;

    public UploadMediaCommandHandler(
        IMediaRepository mediaRepository,
        IMediaStorageService storageService,
        TimeProvider timeProvider)
    {
        this.mediaRepository = mediaRepository;
        this.storageService = storageService;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<MediaDto>> HandleAsync(
        UploadMediaCommand command,
        CancellationToken cancellationToken)
    {
        if (command.FileSize <= 0)
        {
            return Result<MediaDto>.Validation(
                new ValidationError(nameof(command.FileSize), "FileSize must be greater than zero."));
        }

        if (command.FileSize > MediaUploadLimits.MaximumFileSizeBytes)
        {
            return Result<MediaDto>.Validation(
                new ValidationError(nameof(command.FileSize), $"FileSize must be {MediaUploadLimits.MaximumFileSizeBytes} bytes or fewer."));
        }

        var storedResult = await storageService.SaveAsync(
            command.Content,
            command.OriginalFileName,
            command.ContentType,
            command.FileSize,
            cancellationToken);

        if (!storedResult.IsSuccess)
        {
            return Result<MediaDto>.Validation([.. storedResult.Errors]);
        }

        var stored = storedResult.Value!;
        var media = Domain.Media.Media.Create(
            stored.FileName,
            stored.OriginalFileName,
            stored.FileExtension,
            stored.ContentType,
            stored.FileSize,
            stored.MediaType,
            stored.StoragePath,
            stored.PublicUrl,
            command.AltText ?? string.Empty,
            command.Description ?? string.Empty,
            timeProvider.GetUtcNow());

        await mediaRepository.AddAsync(media, cancellationToken);
        await mediaRepository.SaveChangesAsync(cancellationToken);

        return Result<MediaDto>.Success(MediaMapper.ToDto(media));
    }
}
