using EnglishMaster.Application.Features.ContentQuality.Dtos;
using EnglishMaster.Domain.ContentQuality;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.ContentQuality.Commands;

public sealed record CreateContentQualityRuleCommand(string Code, string Name, string? Description, string ContentType, string Severity);
public sealed record UpdateContentQualityRuleCommand(Guid Id, string Code, string Name, string? Description, string ContentType, string Severity);
public sealed record ActivateContentQualityRuleCommand(Guid Id);
public sealed record DeactivateContentQualityRuleCommand(Guid Id);
public sealed record RunContentQualityCheckCommand(string ContentType, Guid ContentId, string? CheckedBy);
public sealed record MarkContentQualityFindingResolvedCommand(Guid Id);

public sealed class ContentQualityCommandHandler
{
    private readonly IContentQualityRepository repository;
    private readonly IContentQualityService qualityService;
    private readonly TimeProvider timeProvider;

    public ContentQualityCommandHandler(IContentQualityRepository repository, IContentQualityService qualityService, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.qualityService = qualityService;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<ContentQualityRuleDto>> CreateRuleAsync(CreateContentQualityRuleCommand command, CancellationToken cancellationToken)
    {
        if (await repository.RuleCodeExistsAsync(command.Code.Trim(), excludingId: null, cancellationToken))
        {
            return Result<ContentQualityRuleDto>.Validation(new ValidationError(nameof(command.Code), "Quality rule code is already used."));
        }

        if (!Enum.TryParse<ContentQualitySeverity>(command.Severity, ignoreCase: true, out var severity))
        {
            return Result<ContentQualityRuleDto>.Validation(new ValidationError(nameof(command.Severity), "Severity is invalid."));
        }

        try
        {
            var rule = ContentQualityRule.Create(command.Code, command.Name, command.Description, command.ContentType, severity, timeProvider.GetUtcNow());
            return Result<ContentQualityRuleDto>.Success(await repository.AddRuleAsync(rule, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return Result<ContentQualityRuleDto>.Validation(new ValidationError(exception.ParamName ?? "rule", exception.Message));
        }
    }

    public async Task<Result<ContentQualityRuleDto>> UpdateRuleAsync(UpdateContentQualityRuleCommand command, CancellationToken cancellationToken)
    {
        var rule = await repository.GetRuleEntityAsync(command.Id, cancellationToken);
        if (rule is null)
        {
            return Result<ContentQualityRuleDto>.NotFound(nameof(command.Id), "Quality rule was not found.");
        }

        if (await repository.RuleCodeExistsAsync(command.Code.Trim(), command.Id, cancellationToken))
        {
            return Result<ContentQualityRuleDto>.Validation(new ValidationError(nameof(command.Code), "Quality rule code is already used."));
        }

        if (!Enum.TryParse<ContentQualitySeverity>(command.Severity, ignoreCase: true, out var severity))
        {
            return Result<ContentQualityRuleDto>.Validation(new ValidationError(nameof(command.Severity), "Severity is invalid."));
        }

        try
        {
            rule.Update(command.Code, command.Name, command.Description, command.ContentType, severity, timeProvider.GetUtcNow());
            return Result<ContentQualityRuleDto>.Success(await repository.SaveRuleAsync(rule, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return Result<ContentQualityRuleDto>.Validation(new ValidationError(exception.ParamName ?? "rule", exception.Message));
        }
    }

    public async Task<Result<ContentQualityRuleDto>> ActivateRuleAsync(ActivateContentQualityRuleCommand command, CancellationToken cancellationToken)
    {
        var rule = await repository.GetRuleEntityAsync(command.Id, cancellationToken);
        if (rule is null)
        {
            return Result<ContentQualityRuleDto>.NotFound(nameof(command.Id), "Quality rule was not found.");
        }

        rule.Activate(timeProvider.GetUtcNow());
        return Result<ContentQualityRuleDto>.Success(await repository.SaveRuleAsync(rule, cancellationToken));
    }

    public async Task<Result<ContentQualityRuleDto>> DeactivateRuleAsync(DeactivateContentQualityRuleCommand command, CancellationToken cancellationToken)
    {
        var rule = await repository.GetRuleEntityAsync(command.Id, cancellationToken);
        if (rule is null)
        {
            return Result<ContentQualityRuleDto>.NotFound(nameof(command.Id), "Quality rule was not found.");
        }

        rule.Deactivate(timeProvider.GetUtcNow());
        return Result<ContentQualityRuleDto>.Success(await repository.SaveRuleAsync(rule, cancellationToken));
    }

    public async Task<Result<ContentQualityCheckDto>> RunAsync(RunContentQualityCheckCommand command, CancellationToken cancellationToken)
    {
        var check = await qualityService.RunAsync(command.ContentType, command.ContentId, command.CheckedBy, cancellationToken);
        return check is null
            ? Result<ContentQualityCheckDto>.NotFound(nameof(command.ContentId), "Content was not found.")
            : Result<ContentQualityCheckDto>.Success(check);
    }

    public async Task<Result<ContentQualityFindingDto>> MarkFindingResolvedAsync(MarkContentQualityFindingResolvedCommand command, CancellationToken cancellationToken)
    {
        var finding = await repository.MarkFindingResolvedAsync(command.Id, timeProvider.GetUtcNow(), cancellationToken);
        return finding is null
            ? Result<ContentQualityFindingDto>.NotFound(nameof(command.Id), "Quality finding was not found.")
            : Result<ContentQualityFindingDto>.Success(finding);
    }
}
