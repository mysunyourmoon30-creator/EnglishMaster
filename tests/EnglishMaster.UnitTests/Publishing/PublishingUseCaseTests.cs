using EnglishMaster.Application.Features.PublishJobs.Commands;
using EnglishMaster.Application.Features.PublishJobs.Queries;
using EnglishMaster.Application.Features.PublishTemplates.Commands;
using EnglishMaster.Application.Features.Publishing;
using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;

namespace EnglishMaster.UnitTests.Publishing;

public sealed class PublishingUseCaseTests
{
    [Fact]
    public async Task CreatePublishJobCreatesPendingJob()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var jobs = new FakePublishJobRepository();
        var handler = new CreatePublishJobCommandHandler(jobs, new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreatePublishJobCommand("Lesson", Guid.NewGuid(), "Html", "Lesson Export", "admin"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(jobs.PublishJobs);
        Assert.Equal("Pending", result.Value!.Status);
    }

    [Fact]
    public async Task CancelPublishJobFailsWhenCompleted()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var jobs = new FakePublishJobRepository();
        var job = PublishJob.Create(PublishSourceType.Course, Guid.NewGuid(), PublishFormat.Markdown, "Course Export", null, now);
        job.MarkRunning(now.AddMinutes(1));
        job.MarkCompleted("course.md", "publishing/course.md", now.AddMinutes(2));
        jobs.PublishJobs.Add(job);
        var handler = new CancelPublishJobCommandHandler(jobs, new FixedTimeProvider(now.AddMinutes(3)));

        var result = await handler.HandleAsync(new CancelPublishJobCommand(job.Id), CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Equal(PublishJobStatus.Completed, job.Status);
    }

    [Fact]
    public async Task SearchPublishJobsFiltersByStatus()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var jobs = new FakePublishJobRepository();
        var pending = PublishJob.Create(PublishSourceType.Lesson, Guid.NewGuid(), PublishFormat.Html, "Pending Job", null, now);
        var completed = PublishJob.Create(PublishSourceType.Book, Guid.NewGuid(), PublishFormat.Markdown, "Completed Job", null, now.AddMinutes(1));
        completed.MarkRunning(now.AddMinutes(2));
        completed.MarkCompleted("book.md", "publishing/book.md", now.AddMinutes(3));
        jobs.PublishJobs.AddRange([pending, completed]);
        var handler = new SearchPublishJobsQueryHandler(jobs);

        var result = await handler.HandleAsync(
            new SearchPublishJobsQuery(null, null, null, "Completed", 1, 20, "CreatedAt", "Desc"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Completed Job", result.Value.Items.Single().Title);
    }

    [Fact]
    public async Task CreatePublishTemplateCreatesSluggedTemplate()
    {
        var templates = new FakePublishTemplateRepository();
        var handler = new CreatePublishTemplateCommandHandler(templates, new FixedTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.HandleAsync(
            new CreatePublishTemplateCommand("Default Markdown", null, "Markdown", "# {{title}}", true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(templates.Templates);
        Assert.Equal("default-markdown", result.Value!.Slug);
    }

    [Fact]
    public async Task RunPublishJobReturnsValidationWhenJobIsCompleted()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var jobs = new FakePublishJobRepository();
        var job = PublishJob.Create(PublishSourceType.Lesson, Guid.NewGuid(), PublishFormat.Html, "Completed Job", null, now);
        job.MarkRunning(now.AddMinutes(1));
        job.MarkCompleted("completed.html", "publishing/completed.html", now.AddMinutes(2));
        jobs.PublishJobs.Add(job);
        var service = CreatePublishingService(jobs, now.AddMinutes(3));

        var result = await service.RunAsync(job.Id, CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Equal(PublishJobStatus.Completed, job.Status);
        Assert.DoesNotContain(result.Errors, error => error.Message.Contains("cannot be failed", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task RunPublishJobCommandValidatesEmptyId()
    {
        var handler = new RunPublishJobCommandHandler(new ThrowingPublishingService());

        var result = await handler.HandleAsync(new RunPublishJobCommand(Guid.Empty), CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public async Task RunPublishJobCanRerunFailedJob()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var jobs = new FakePublishJobRepository();
        var artifacts = new FakePublishedArtifactRepository();
        var job = PublishJob.Create(PublishSourceType.Book, Guid.NewGuid(), PublishFormat.Markdown, "Book Export", null, now);
        job.MarkFailed("Temporary failure.", now.AddMinutes(1));
        jobs.PublishJobs.Add(job);
        var service = CreatePublishingService(jobs, now.AddMinutes(2), artifacts);

        var result = await service.RunAsync(job.Id, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Completed", result.Value!.Status);
        Assert.Single(artifacts.Artifacts);
        Assert.Empty(job.ErrorMessage);
    }

    private static PublishingService CreatePublishingService(
        FakePublishJobRepository jobs,
        DateTimeOffset now,
        FakePublishedArtifactRepository? artifacts = null)
    {
        return new PublishingService(
            jobs,
            artifacts ?? new FakePublishedArtifactRepository(),
            new StaticPublishContentBuilder(),
            new StaticPublishFileStorage(),
            new FixedTimeProvider(now));
    }

    private sealed class StaticPublishContentBuilder : IPublishContentBuilder
    {
        public Task<PublishContent> BuildAsync(
            PublishSourceType sourceType,
            Guid sourceId,
            PublishFormat format,
            string title,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new PublishContent("export.md", "# Export", "text/markdown"));
        }
    }

    private sealed class StaticPublishFileStorage : IPublishFileStorage
    {
        public Task<StoredPublishFile> SaveAsync(
            string fileName,
            string content,
            string contentType,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new StoredPublishFile(fileName, $"publishing/{fileName}", $"/publishing/{fileName}", content.Length, contentType));
        }
    }

    private sealed class ThrowingPublishingService : IPublishingService
    {
        public Task<Result<PublishJobDto>> RunAsync(Guid publishJobId, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("The publishing service should not be called for invalid command input.");
        }
    }
}
