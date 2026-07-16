using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.UnitTests.Publishing;

public sealed class PublishingDomainTests
{
    [Fact]
    public void PublishJobCreateStartsPending()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var job = PublishJob.Create(PublishSourceType.Lesson, Guid.NewGuid(), PublishFormat.Html, "Daily Lesson", "admin", now);

        Assert.Equal(PublishJobStatus.Pending, job.Status);
        Assert.Equal("Daily Lesson", job.Title);
        Assert.Equal(now, job.CreatedAt);
    }

    [Fact]
    public void PublishJobStatusTransitionsSetAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var job = PublishJob.Create(PublishSourceType.Book, Guid.NewGuid(), PublishFormat.Markdown, "Starter Book", null, now);

        job.MarkRunning(now.AddMinutes(1));
        job.MarkCompleted("starter-book.md", "publishing/starter-book.md", now.AddMinutes(2));

        Assert.Equal(PublishJobStatus.Completed, job.Status);
        Assert.NotNull(job.StartedAt);
        Assert.Equal(now.AddMinutes(2), job.CompletedAt);
    }

    [Fact]
    public void FailPublishJobRequiresErrorMessage()
    {
        var job = PublishJob.Create(PublishSourceType.Course, Guid.NewGuid(), PublishFormat.Pdf, "Course", null, DateTimeOffset.UtcNow);

        var exception = Assert.Throws<ArgumentException>(() => job.MarkFailed("", DateTimeOffset.UtcNow));

        Assert.Contains("ErrorMessage is required", exception.Message);
    }

    [Fact]
    public void CompletedPublishJobCannotBeCancelled()
    {
        var now = DateTimeOffset.UtcNow;
        var job = PublishJob.Create(PublishSourceType.Quiz, Guid.NewGuid(), PublishFormat.Html, "Quiz", null, now);
        job.MarkRunning(now.AddMinutes(1));
        job.MarkCompleted("quiz.html", "publishing/quiz.html", now.AddMinutes(2));

        Assert.Throws<InvalidOperationException>(() => job.Cancel(now.AddMinutes(3)));
    }

    [Fact]
    public void PublishTemplateGeneratesSlug()
    {
        var template = PublishTemplate.Create("Default HTML Template", null, PublishFormat.Html, null, true, DateTimeOffset.UtcNow);

        Assert.Equal("default-html-template", template.Slug);
        Assert.True(template.IsActive);
    }

    [Fact]
    public void PublishedArtifactCreateProtectsFileSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PublishedArtifact.Create(
                Guid.NewGuid(),
                PublishSourceType.Lesson,
                Guid.NewGuid(),
                PublishFormat.Html,
                "lesson.html",
                "publishing/lesson.html",
                "/publishing/lesson.html",
                -1,
                "text/html",
                DateTimeOffset.UtcNow));
    }
}
