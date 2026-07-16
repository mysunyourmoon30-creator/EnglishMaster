using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.PublishJobs.Commands;

public sealed class CreatePublishJobCommandHandler
{
    private readonly IPublishJobRepository publishJobRepository;
    private readonly TimeProvider timeProvider;

    public CreatePublishJobCommandHandler(IPublishJobRepository publishJobRepository, TimeProvider timeProvider)
    {
        this.publishJobRepository = publishJobRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PublishJobDto>> HandleAsync(
        CreatePublishJobCommand command,
        CancellationToken cancellationToken)
    {
        var errors = PublishInputValidator.ValidateJob(command.SourceType, command.SourceId, command.Format, command.Title, command.RequestedBy);
        if (errors.Count > 0)
        {
            return Result<PublishJobDto>.Validation([.. errors]);
        }

        var publishJob = PublishJob.Create(
            Enum.Parse<PublishSourceType>(command.SourceType!, ignoreCase: true),
            command.SourceId,
            Enum.Parse<PublishFormat>(command.Format!, ignoreCase: true),
            command.Title,
            command.RequestedBy,
            timeProvider.GetUtcNow());

        await publishJobRepository.AddAsync(publishJob, cancellationToken);
        await publishJobRepository.SaveChangesAsync(cancellationToken);

        return Result<PublishJobDto>.Success(PublishJobMapper.ToDto(publishJob));
    }
}
