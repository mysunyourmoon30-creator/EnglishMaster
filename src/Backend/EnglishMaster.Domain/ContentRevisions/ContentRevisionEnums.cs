namespace EnglishMaster.Domain.ContentRevisions;

public enum ContentRevisionEventType
{
    Created = 1,
    Updated = 2,
    SubmittedForReview = 3,
    Approved = 4,
    ChangesRequested = 5,
    Published = 6,
    Archived = 7,
    QualityChecked = 8,
    Restored = 9
}

public enum ContentRevisionRestoreStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Completed = 4,
    Cancelled = 5
}
