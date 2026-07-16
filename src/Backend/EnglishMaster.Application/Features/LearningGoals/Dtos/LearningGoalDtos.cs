namespace EnglishMaster.Application.Features.LearningGoals.Dtos;

public sealed record LearningGoalDto(
    Guid Id,
    string GoalType,
    string Title,
    string Description,
    int TargetValue,
    int CurrentValue,
    string Unit,
    DateTimeOffset? TargetDate,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record LearningGoalSummaryDto(
    int ActiveCount,
    int CompletedCount,
    int PausedCount,
    int CancelledCount);
