namespace EnglishMaster.Contracts.LearningGoals;

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

public sealed record CreateLearningGoalRequest(
    string GoalType,
    string Title,
    string? Description,
    int TargetValue,
    string? Unit,
    DateTimeOffset? TargetDate);

public sealed record UpdateLearningGoalRequest(
    string Title,
    string? Description,
    int TargetValue,
    int CurrentValue,
    string? Unit,
    DateTimeOffset? TargetDate);
