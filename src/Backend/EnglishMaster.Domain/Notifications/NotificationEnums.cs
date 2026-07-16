namespace EnglishMaster.Domain.Notifications;

public enum NotificationType
{
    InApp = 1,
    Email = 2
}

public enum NotificationStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3,
    Read = 4,
    Cancelled = 5
}

public enum NotificationEventType
{
    ContentSubmittedForReview = 1,
    ContentApproved = 2,
    ContentChangesRequested = 3,
    ContentPublished = 4,
    PublishJobCompleted = 5,
    PublishJobFailed = 6,
    UserCreated = 7,
    RoleAssigned = 8,
    QuizSubmitted = 9
}

public enum EmailMessageStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3
}
