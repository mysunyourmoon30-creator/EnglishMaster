using EnglishMaster.Application.Features.Achievements.Dtos;
using EnglishMaster.Application.Features.Motivation;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Achievements.Commands;

public sealed record SeedDefaultAchievementDefinitionsCommand;
public sealed record AchievementDefinitionCommand(string Code, string Name, string? Description, string AchievementType, int TargetValue, string? IconName, int SortOrder);
public sealed record UpdateAchievementDefinitionCommand(Guid Id, string Name, string? Description, string AchievementType, int TargetValue, string? IconName, int SortOrder);
public sealed record AchievementDefinitionLifecycleCommand(Guid Id);
public sealed record EvaluateMyAchievementsCommand(Guid UserId);

public sealed class AchievementCommandHandler
{
    private readonly IMotivationRepository repository;

    public AchievementCommandHandler(IMotivationRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<int>> SeedDefaultsAsync(SeedDefaultAchievementDefinitionsCommand command, CancellationToken cancellationToken) =>
        Result<int>.Success(await repository.SeedDefaultAchievementDefinitionsAsync(cancellationToken));

    public async Task<Result<AchievementDefinitionDto>> CreateDefinitionAsync(AchievementDefinitionCommand command, CancellationToken cancellationToken)
    {
        if (command.TargetValue < 0)
        {
            return Result<AchievementDefinitionDto>.Validation(new ValidationError(nameof(command.TargetValue), "Target value must not be negative."));
        }

        var definition = await repository.CreateDefinitionAsync(command.Code, command.Name, command.Description, command.AchievementType, command.TargetValue, command.IconName, command.SortOrder, cancellationToken);
        return definition is null ? Result<AchievementDefinitionDto>.Validation(new ValidationError(nameof(command.Code), "Achievement code already exists.")) : Result<AchievementDefinitionDto>.Success(definition);
    }

    public async Task<Result<AchievementDefinitionDto>> UpdateDefinitionAsync(UpdateAchievementDefinitionCommand command, CancellationToken cancellationToken)
    {
        var definition = await repository.UpdateDefinitionAsync(command.Id, command.Name, command.Description, command.AchievementType, command.TargetValue, command.IconName, command.SortOrder, cancellationToken);
        return definition is null ? Result<AchievementDefinitionDto>.NotFound(nameof(command.Id), "Achievement definition was not found.") : Result<AchievementDefinitionDto>.Success(definition);
    }

    public Task<Result<AchievementDefinitionDto>> ActivateAsync(AchievementDefinitionLifecycleCommand command, CancellationToken cancellationToken) =>
        SetActiveAsync(command, true, cancellationToken);

    public Task<Result<AchievementDefinitionDto>> DeactivateAsync(AchievementDefinitionLifecycleCommand command, CancellationToken cancellationToken) =>
        SetActiveAsync(command, false, cancellationToken);

    public async Task<Result<IReadOnlyCollection<StudentAchievementDto>>> EvaluateAsync(EvaluateMyAchievementsCommand command, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<StudentAchievementDto>>.Success(await repository.EvaluateAchievementsAsync(command.UserId, cancellationToken));

    private async Task<Result<AchievementDefinitionDto>> SetActiveAsync(AchievementDefinitionLifecycleCommand command, bool active, CancellationToken cancellationToken)
    {
        var definition = await repository.SetDefinitionActiveAsync(command.Id, active, cancellationToken);
        return definition is null ? Result<AchievementDefinitionDto>.NotFound(nameof(command.Id), "Achievement definition was not found.") : Result<AchievementDefinitionDto>.Success(definition);
    }
}
