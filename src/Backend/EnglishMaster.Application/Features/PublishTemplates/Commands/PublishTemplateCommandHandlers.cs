using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Application.Features.PublishTemplates.Dtos;
using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.PublishTemplates.Commands;

public sealed class CreatePublishTemplateCommandHandler
{
    private readonly IPublishTemplateRepository templateRepository;
    private readonly TimeProvider timeProvider;

    public CreatePublishTemplateCommandHandler(IPublishTemplateRepository templateRepository, TimeProvider timeProvider)
    {
        this.templateRepository = templateRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PublishTemplateDto>> HandleAsync(CreatePublishTemplateCommand command, CancellationToken cancellationToken)
    {
        var errors = PublishInputValidator.ValidateTemplate(command.Name, command.Description, command.Format, command.TemplateContent);
        if (errors.Count > 0)
        {
            return Result<PublishTemplateDto>.Validation([.. errors]);
        }

        var format = Enum.Parse<PublishFormat>(command.Format!, ignoreCase: true);
        var slug = PublishTemplate.GenerateSlug(command.Name);
        if (await templateRepository.SlugExistsAsync(slug, excludedTemplateId: null, cancellationToken))
        {
            return Result<PublishTemplateDto>.Validation(new ValidationError(nameof(command.Name), "A publish template with this slug already exists."));
        }

        if (command.IsDefault && await templateRepository.DefaultExistsAsync(format, excludedTemplateId: null, cancellationToken))
        {
            return Result<PublishTemplateDto>.Validation(new ValidationError(nameof(command.IsDefault), "A default template already exists for this format."));
        }

        var template = PublishTemplate.Create(command.Name, command.Description, format, command.TemplateContent, command.IsDefault, timeProvider.GetUtcNow());
        await templateRepository.AddAsync(template, cancellationToken);
        await templateRepository.SaveChangesAsync(cancellationToken);

        return Result<PublishTemplateDto>.Success(PublishTemplateMapper.ToDto(template));
    }
}

public sealed class UpdatePublishTemplateCommandHandler
{
    private readonly IPublishTemplateRepository templateRepository;
    private readonly TimeProvider timeProvider;

    public UpdatePublishTemplateCommandHandler(IPublishTemplateRepository templateRepository, TimeProvider timeProvider)
    {
        this.templateRepository = templateRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PublishTemplateDto>> HandleAsync(UpdatePublishTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await templateRepository.GetByIdAsync(command.Id, cancellationToken);
        if (template is null)
        {
            return Result<PublishTemplateDto>.NotFound(nameof(command.Id), "Publish template was not found.");
        }

        var errors = PublishInputValidator.ValidateTemplate(command.Name, command.Description, command.Format, command.TemplateContent);
        if (errors.Count > 0)
        {
            return Result<PublishTemplateDto>.Validation([.. errors]);
        }

        var format = Enum.Parse<PublishFormat>(command.Format!, ignoreCase: true);
        var slug = PublishTemplate.GenerateSlug(command.Name);
        if (await templateRepository.SlugExistsAsync(slug, command.Id, cancellationToken))
        {
            return Result<PublishTemplateDto>.Validation(new ValidationError(nameof(command.Name), "A publish template with this slug already exists."));
        }

        if (command.IsDefault && await templateRepository.DefaultExistsAsync(format, command.Id, cancellationToken))
        {
            return Result<PublishTemplateDto>.Validation(new ValidationError(nameof(command.IsDefault), "A default template already exists for this format."));
        }

        template.Update(command.Name, command.Description, format, command.TemplateContent, command.IsDefault, command.IsActive, timeProvider.GetUtcNow());
        await templateRepository.SaveChangesAsync(cancellationToken);

        return Result<PublishTemplateDto>.Success(PublishTemplateMapper.ToDto(template));
    }
}

public sealed class DeletePublishTemplateCommandHandler
{
    private readonly IPublishTemplateRepository templateRepository;

    public DeletePublishTemplateCommandHandler(IPublishTemplateRepository templateRepository)
    {
        this.templateRepository = templateRepository;
    }

    public async Task<Result> HandleAsync(DeletePublishTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await templateRepository.GetByIdAsync(command.Id, cancellationToken);
        if (template is null)
        {
            return Result.NotFound(nameof(command.Id), "Publish template was not found.");
        }

        templateRepository.Remove(template);
        await templateRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class ActivatePublishTemplateCommandHandler
{
    private readonly IPublishTemplateRepository templateRepository;
    private readonly TimeProvider timeProvider;

    public ActivatePublishTemplateCommandHandler(IPublishTemplateRepository templateRepository, TimeProvider timeProvider)
    {
        this.templateRepository = templateRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PublishTemplateDto>> HandleAsync(ActivatePublishTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await templateRepository.GetByIdAsync(command.Id, cancellationToken);
        if (template is null)
        {
            return Result<PublishTemplateDto>.NotFound(nameof(command.Id), "Publish template was not found.");
        }

        template.Activate(timeProvider.GetUtcNow());
        await templateRepository.SaveChangesAsync(cancellationToken);

        return Result<PublishTemplateDto>.Success(PublishTemplateMapper.ToDto(template));
    }
}

public sealed class DeactivatePublishTemplateCommandHandler
{
    private readonly IPublishTemplateRepository templateRepository;
    private readonly TimeProvider timeProvider;

    public DeactivatePublishTemplateCommandHandler(IPublishTemplateRepository templateRepository, TimeProvider timeProvider)
    {
        this.templateRepository = templateRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PublishTemplateDto>> HandleAsync(DeactivatePublishTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await templateRepository.GetByIdAsync(command.Id, cancellationToken);
        if (template is null)
        {
            return Result<PublishTemplateDto>.NotFound(nameof(command.Id), "Publish template was not found.");
        }

        template.Deactivate(timeProvider.GetUtcNow());
        await templateRepository.SaveChangesAsync(cancellationToken);

        return Result<PublishTemplateDto>.Success(PublishTemplateMapper.ToDto(template));
    }
}
