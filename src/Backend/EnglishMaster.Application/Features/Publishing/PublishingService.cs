using EnglishMaster.Application.Features.PublishedArtifacts;
using EnglishMaster.Application.Features.PublishJobs;
using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Publishing;

public sealed class PublishingService : IPublishingService
{
    private readonly IPublishJobRepository publishJobRepository;
    private readonly IPublishedArtifactRepository artifactRepository;
    private readonly IPublishContentBuilder contentBuilder;
    private readonly IPublishFileStorage fileStorage;
    private readonly TimeProvider timeProvider;

    public PublishingService(
        IPublishJobRepository publishJobRepository,
        IPublishedArtifactRepository artifactRepository,
        IPublishContentBuilder contentBuilder,
        IPublishFileStorage fileStorage,
        TimeProvider timeProvider)
    {
        this.publishJobRepository = publishJobRepository;
        this.artifactRepository = artifactRepository;
        this.contentBuilder = contentBuilder;
        this.fileStorage = fileStorage;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PublishJobDto>> RunAsync(Guid publishJobId, CancellationToken cancellationToken)
    {
        if (publishJobId == Guid.Empty)
        {
            return Result<PublishJobDto>.Validation(new ValidationError(nameof(publishJobId), "PublishJobId is required."));
        }

        var publishJob = await publishJobRepository.GetByIdAsync(publishJobId, cancellationToken);
        if (publishJob is null)
        {
            return Result<PublishJobDto>.NotFound(nameof(publishJobId), "Publish job was not found.");
        }

        try
        {
            publishJob.MarkRunning(timeProvider.GetUtcNow());
        }
        catch (InvalidOperationException exception)
        {
            return Result<PublishJobDto>.Validation(new ValidationError(nameof(publishJobId), exception.Message));
        }

        await publishJobRepository.SaveChangesAsync(cancellationToken);

        try
        {
            var content = await contentBuilder.BuildAsync(
                publishJob.SourceType,
                publishJob.SourceId,
                publishJob.Format,
                publishJob.Title,
                cancellationToken);
            var storedFile = await fileStorage.SaveAsync(content.FileName, content.Content, content.ContentType, cancellationToken);
            var now = timeProvider.GetUtcNow();
            var artifact = PublishedArtifact.Create(
                publishJob.Id,
                publishJob.SourceType,
                publishJob.SourceId,
                publishJob.Format,
                storedFile.FileName,
                storedFile.RelativePath,
                storedFile.PublicUrl,
                storedFile.FileSize,
                storedFile.ContentType,
                now);

            await artifactRepository.AddAsync(artifact, cancellationToken);
            publishJob.MarkCompleted(storedFile.FileName, storedFile.RelativePath, now);
            await publishJobRepository.SaveChangesAsync(cancellationToken);

            return Result<PublishJobDto>.Success(PublishJobMapper.ToDto(publishJob));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or IOException)
        {
            publishJob.MarkFailed(exception.Message, timeProvider.GetUtcNow());
            await publishJobRepository.SaveChangesAsync(cancellationToken);

            return Result<PublishJobDto>.Success(PublishJobMapper.ToDto(publishJob));
        }
    }
}
