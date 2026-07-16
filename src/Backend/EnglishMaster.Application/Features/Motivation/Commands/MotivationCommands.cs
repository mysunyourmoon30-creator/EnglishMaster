using EnglishMaster.Application.Features.Motivation.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Motivation.Commands;

public sealed record RecordLearningActivityCommand(Guid UserId, string ActivityType, string? ContentType, Guid? ContentId, string? Title, DateTimeOffset? OccurredAt, int MinutesSpent, string? MetadataJson);
public sealed record UpdateMyStreakCommand(Guid UserId);

public sealed class MotivationCommandHandler
{
    private static readonly HashSet<string> AllowedActivityTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "LessonStarted", "LessonCompleted", "CourseStarted", "CourseCompleted", "BookStarted", "BookCompleted",
        "QuizAttempted", "QuizPassed", "PracticeSessionCompleted", "DailyStudyPlanCompleted", "LearningGoalCompleted"
    };

    private readonly IMotivationRepository repository;
    private readonly TimeProvider timeProvider;

    public MotivationCommandHandler(IMotivationRepository repository, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<LearningActivityDto>> RecordActivityAsync(RecordLearningActivityCommand command, CancellationToken cancellationToken)
    {
        if (!AllowedActivityTypes.Contains(command.ActivityType))
        {
            return Result<LearningActivityDto>.Validation(new ValidationError(nameof(command.ActivityType), "Activity type is not supported."));
        }

        if (command.MinutesSpent < 0)
        {
            return Result<LearningActivityDto>.Validation(new ValidationError(nameof(command.MinutesSpent), "Minutes spent must not be negative."));
        }

        var occurredAt = command.OccurredAt ?? timeProvider.GetUtcNow();
        var activity = await repository.RecordActivityAsync(command.UserId, command.ActivityType, command.ContentType, command.ContentId, command.Title, occurredAt, command.MinutesSpent, command.MetadataJson, cancellationToken);
        return Result<LearningActivityDto>.Success(activity);
    }

    public async Task<Result<StudentStreakDto>> UpdateStreakAsync(UpdateMyStreakCommand command, CancellationToken cancellationToken) =>
        Result<StudentStreakDto>.Success(await repository.UpdateStreakAsync(command.UserId, timeProvider.GetUtcNow(), cancellationToken));
}
