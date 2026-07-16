using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Application.Features.Publishing;
using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.PublishJobs.Commands;

public sealed class StartPublishJobCommandHandler
{
    private readonly IPublishJobRepository publishJobRepository;
    private readonly TimeProvider timeProvider;

    public StartPublishJobCommandHandler(IPublishJobRepository publishJobRepository, TimeProvider timeProvider)
    {
        this.publishJobRepository = publishJobRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PublishJobDto>> HandleAsync(StartPublishJobCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Result<PublishJobDto>.Validation(new ValidationError(nameof(command.Id), "Id is required."));
        }

        var publishJob = await publishJobRepository.GetByIdAsync(command.Id, cancellationToken);
        if (publishJob is null)
        {
            return Result<PublishJobDto>.NotFound(nameof(command.Id), "Publish job was not found.");
        }

        try
        {
            publishJob.MarkRunning(timeProvider.GetUtcNow());
        }
        catch (InvalidOperationException exception)
        {
            return Result<PublishJobDto>.Validation(new ValidationError(nameof(command.Id), exception.Message));
        }

        await publishJobRepository.SaveChangesAsync(cancellationToken);
        return Result<PublishJobDto>.Success(PublishJobMapper.ToDto(publishJob));
    }
}

public sealed class CompletePublishJobCommandHandler
{
    private readonly IPublishJobRepository publishJobRepository;
    private readonly TimeProvider timeProvider;

    public CompletePublishJobCommandHandler(IPublishJobRepository publishJobRepository, TimeProvider timeProvider)
    {
        this.publishJobRepository = publishJobRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PublishJobDto>> HandleAsync(CompletePublishJobCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Result<PublishJobDto>.Validation(new ValidationError(nameof(command.Id), "Id is required."));
        }

        var publishJob = await publishJobRepository.GetByIdAsync(command.Id, cancellationToken);
        if (publishJob is null)
        {
            return Result<PublishJobDto>.NotFound(nameof(command.Id), "Publish job was not found.");
        }

        try
        {
            publishJob.MarkCompleted(command.OutputFileName, command.OutputPath, timeProvider.GetUtcNow());
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Result<PublishJobDto>.Validation(new ValidationError(nameof(command.Id), exception.Message));
        }

        await publishJobRepository.SaveChangesAsync(cancellationToken);
        return Result<PublishJobDto>.Success(PublishJobMapper.ToDto(publishJob));
    }
}

public sealed class FailPublishJobCommandHandler
{
    private readonly IPublishJobRepository publishJobRepository;
    private readonly TimeProvider timeProvider;

    public FailPublishJobCommandHandler(IPublishJobRepository publishJobRepository, TimeProvider timeProvider)
    {
        this.publishJobRepository = publishJobRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PublishJobDto>> HandleAsync(FailPublishJobCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Result<PublishJobDto>.Validation(new ValidationError(nameof(command.Id), "Id is required."));
        }

        var publishJob = await publishJobRepository.GetByIdAsync(command.Id, cancellationToken);
        if (publishJob is null)
        {
            return Result<PublishJobDto>.NotFound(nameof(command.Id), "Publish job was not found.");
        }

        try
        {
            publishJob.MarkFailed(command.ErrorMessage, timeProvider.GetUtcNow());
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Result<PublishJobDto>.Validation(new ValidationError(nameof(command.ErrorMessage), exception.Message));
        }

        await publishJobRepository.SaveChangesAsync(cancellationToken);
        return Result<PublishJobDto>.Success(PublishJobMapper.ToDto(publishJob));
    }
}

public sealed class CancelPublishJobCommandHandler
{
    private readonly IPublishJobRepository publishJobRepository;
    private readonly TimeProvider timeProvider;

    public CancelPublishJobCommandHandler(IPublishJobRepository publishJobRepository, TimeProvider timeProvider)
    {
        this.publishJobRepository = publishJobRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PublishJobDto>> HandleAsync(CancelPublishJobCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Result<PublishJobDto>.Validation(new ValidationError(nameof(command.Id), "Id is required."));
        }

        var publishJob = await publishJobRepository.GetByIdAsync(command.Id, cancellationToken);
        if (publishJob is null)
        {
            return Result<PublishJobDto>.NotFound(nameof(command.Id), "Publish job was not found.");
        }

        try
        {
            publishJob.Cancel(timeProvider.GetUtcNow());
        }
        catch (InvalidOperationException exception)
        {
            return Result<PublishJobDto>.Validation(new ValidationError(nameof(command.Id), exception.Message));
        }

        await publishJobRepository.SaveChangesAsync(cancellationToken);
        return Result<PublishJobDto>.Success(PublishJobMapper.ToDto(publishJob));
    }
}

public sealed class RunPublishJobCommandHandler
{
    private readonly IPublishingService publishingService;

    public RunPublishJobCommandHandler(IPublishingService publishingService)
    {
        this.publishingService = publishingService;
    }

    public Task<Result<PublishJobDto>> HandleAsync(RunPublishJobCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Task.FromResult(Result<PublishJobDto>.Validation(new ValidationError(nameof(command.Id), "Id is required.")));
        }

        return publishingService.RunAsync(command.Id, cancellationToken);
    }
}
