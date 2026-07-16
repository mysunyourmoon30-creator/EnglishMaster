namespace EnglishMaster.Application.Features.Publishing;

public interface IPublishFileStorage
{
    Task<StoredPublishFile> SaveAsync(
        string fileName,
        string content,
        string contentType,
        CancellationToken cancellationToken);
}
