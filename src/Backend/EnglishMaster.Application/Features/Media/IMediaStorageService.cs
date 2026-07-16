using EnglishMaster.Application.Features.Media.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Media;

public interface IMediaStorageService
{
    Task<Result<StoredMediaFile>> SaveAsync(
        Stream content,
        string originalFileName,
        string contentType,
        long fileSize,
        CancellationToken cancellationToken);
}
