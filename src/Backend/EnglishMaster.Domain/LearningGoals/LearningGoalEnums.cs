namespace EnglishMaster.Domain.LearningGoals;

public enum LearningGoalStatus
{
    Active = 0,
    Completed = 1,
    Paused = 2,
    Cancelled = 3
}

public enum DailyStudyPlanStatus
{
    Planned = 0,
    InProgress = 1,
    Completed = 2,
    Skipped = 3
}

public enum DailyStudyPlanItemStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Skipped = 3
}
