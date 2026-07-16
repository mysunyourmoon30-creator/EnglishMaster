using EnglishMaster.Domain.Notifications;

namespace EnglishMaster.UnitTests.Notifications;

public sealed class NotificationDomainTests
{
    [Fact]
    public void Notification_StartsPendingAndCanTransition()
    {
        var now = DateTimeOffset.UtcNow;
        var notification = Notification.Create(
            Guid.NewGuid(),
            NotificationType.InApp,
            NotificationEventType.ContentApproved,
            "Approved",
            "Content approved.",
            now);

        Assert.Equal(NotificationStatus.Pending, notification.Status);

        notification.MarkSent(now.AddMinutes(1));
        Assert.Equal(NotificationStatus.Sent, notification.Status);
        Assert.NotNull(notification.SentAt);

        notification.MarkRead(now.AddMinutes(2));
        Assert.Equal(NotificationStatus.Read, notification.Status);
        Assert.NotNull(notification.ReadAt);

        notification.MarkFailed("Delivery failed.", now.AddMinutes(3));
        Assert.Equal(NotificationStatus.Failed, notification.Status);
        Assert.NotNull(notification.FailedAt);
    }

    [Fact]
    public void EmailMessage_StartsPendingAndCanTransition()
    {
        var now = DateTimeOffset.UtcNow;
        var email = EmailMessage.Queue(
            "learner@example.test",
            "Learner",
            "Welcome",
            "Hello.",
            isHtml: false,
            now);

        Assert.Equal(EmailMessageStatus.Pending, email.Status);

        email.MarkSent(now.AddMinutes(1));
        Assert.Equal(EmailMessageStatus.Sent, email.Status);
        Assert.NotNull(email.SentAt);

        email.MarkFailed("Provider unavailable.", now.AddMinutes(2));
        Assert.Equal(EmailMessageStatus.Failed, email.Status);
        Assert.NotNull(email.FailedAt);
    }
}
