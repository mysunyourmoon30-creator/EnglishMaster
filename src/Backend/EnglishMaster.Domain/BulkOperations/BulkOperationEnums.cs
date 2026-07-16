namespace EnglishMaster.Domain.BulkOperations;

public enum BulkOperationType
{
    SubmitForReview = 1,
    Approve = 2,
    RequestChanges = 3,
    Publish = 4,
    Archive = 5,
    Activate = 6,
    Deactivate = 7,
    RunQualityCheck = 8,
    AssignCategory = 9,
    AddTags = 10,
    RemoveTags = 11,
    Export = 12
}

public enum BulkOperationStatus
{
    Pending = 1,
    Running = 2,
    Completed = 3,
    Failed = 4,
    PartiallyCompleted = 5,
    Cancelled = 6
}

public enum BulkOperationItemStatus
{
    Pending = 1,
    Success = 2,
    Failed = 3,
    Skipped = 4
}
