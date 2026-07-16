using EnglishMaster.Application.Features.LearningGoals.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.LearningGoals.Commands;

public sealed record CreateLearningGoalCommand(Guid UserId, string GoalType, string Title, string? Description, int TargetValue, string? Unit, DateTimeOffset? TargetDate);
public sealed record UpdateLearningGoalCommand(Guid UserId, Guid GoalId, string Title, string? Description, int TargetValue, int CurrentValue, string? Unit, DateTimeOffset? TargetDate);
public sealed record LearningGoalLifecycleCommand(Guid UserId, Guid GoalId);

public sealed class LearningGoalCommandHandler
{
    private static readonly HashSet<string> AllowedGoalTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "DailyStudyMinutes",
        "WeeklyStudyMinutes",
        "DailyPracticeItems",
        "WeeklyLessonsCompleted",
        "WeeklyQuizAttempts",
        "TargetCefrLevel",
        "CompleteCourse",
        "CompleteBook"
    };

    private readonly ILearningGoalRepository repository;

    public LearningGoalCommandHandler(ILearningGoalRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<LearningGoalDto>> CreateAsync(CreateLearningGoalCommand command, CancellationToken cancellationToken)
    {
        if (!AllowedGoalTypes.Contains(command.GoalType))
        {
            return Result<LearningGoalDto>.Validation(new ValidationError(nameof(command.GoalType), "Goal type is not supported."));
        }

        if (command.TargetValue < 0)
        {
            return Result<LearningGoalDto>.Validation(new ValidationError(nameof(command.TargetValue), "Target value must not be negative."));
        }

        return Result<LearningGoalDto>.Success(await repository.CreateAsync(command.UserId, command.GoalType, command.Title, command.Description, command.TargetValue, command.Unit, command.TargetDate, cancellationToken));
    }

    public async Task<Result<LearningGoalDto>> UpdateAsync(UpdateLearningGoalCommand command, CancellationToken cancellationToken)
    {
        if (command.TargetValue < 0 || command.CurrentValue < 0)
        {
            return Result<LearningGoalDto>.Validation(new ValidationError(nameof(command.TargetValue), "Goal values must not be negative."));
        }

        var goal = await repository.UpdateAsync(command.UserId, command.GoalId, command.Title, command.Description, command.TargetValue, command.CurrentValue, command.Unit, command.TargetDate, cancellationToken);
        return goal is null ? Result<LearningGoalDto>.NotFound(nameof(command.GoalId), "Learning goal was not found.") : Result<LearningGoalDto>.Success(goal);
    }

    public Task<Result<LearningGoalDto>> PauseAsync(LearningGoalLifecycleCommand command, CancellationToken cancellationToken) =>
        ChangeStatusAsync(command, "pause", cancellationToken);

    public Task<Result<LearningGoalDto>> ResumeAsync(LearningGoalLifecycleCommand command, CancellationToken cancellationToken) =>
        ChangeStatusAsync(command, "resume", cancellationToken);

    public Task<Result<LearningGoalDto>> CompleteAsync(LearningGoalLifecycleCommand command, CancellationToken cancellationToken) =>
        ChangeStatusAsync(command, "complete", cancellationToken);

    public Task<Result<LearningGoalDto>> CancelAsync(LearningGoalLifecycleCommand command, CancellationToken cancellationToken) =>
        ChangeStatusAsync(command, "cancel", cancellationToken);

    private async Task<Result<LearningGoalDto>> ChangeStatusAsync(LearningGoalLifecycleCommand command, string action, CancellationToken cancellationToken)
    {
        var goal = await repository.ChangeStatusAsync(command.UserId, command.GoalId, action, cancellationToken);
        return goal is null ? Result<LearningGoalDto>.NotFound(nameof(command.GoalId), "Learning goal was not found.") : Result<LearningGoalDto>.Success(goal);
    }
}
