using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.UnitTests.Courses;

public sealed class CourseTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var categoryId = Guid.NewGuid();
        var thumbnailMediaId = Guid.NewGuid();

        var course = Course.Create(
            "  Beginner English Path  ",
            "  Learn the basics  ",
            "  A structured path for beginners.  ",
            CefrLevel.A1,
            categoryId,
            thumbnailMediaId,
            estimatedMinutes: 120,
            sortOrder: 2,
            now);

        Assert.NotEqual(Guid.Empty, course.Id);
        Assert.Equal("Beginner English Path", course.Title);
        Assert.Equal("beginner-english-path", course.Slug);
        Assert.Equal("Learn the basics", course.Summary);
        Assert.Equal("A structured path for beginners.", course.Description);
        Assert.Equal(CefrLevel.A1, course.CefrLevel);
        Assert.Equal(categoryId, course.CategoryId);
        Assert.Equal(thumbnailMediaId, course.ThumbnailMediaId);
        Assert.Equal(120, course.EstimatedMinutes);
        Assert.Equal(2, course.SortOrder);
        Assert.False(course.IsPublished);
        Assert.True(course.IsActive);
        Assert.Equal(now, course.CreatedAt);
        Assert.Equal(now, course.UpdatedAt);
    }

    [Theory]
    [InlineData("Beginner English Path", "beginner-english-path")]
    [InlineData(" A1 / Starter Course ", "a1-starter-course")]
    [InlineData("can't stop", "cant-stop")]
    public void GenerateSlugCreatesUrlFriendlyValue(string title, string expectedSlug)
    {
        var slug = Course.GenerateSlug(title);

        Assert.Equal(expectedSlug, slug);
    }

    [Fact]
    public void CreateRequiresTitle()
    {
        var exception = Assert.Throws<ArgumentException>(() => Course.Create(
            " ",
            null,
            null,
            null,
            null,
            null,
            0,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("Title", exception.ParamName);
    }

    [Fact]
    public void EstimatedMinutesMustNotBeNegative()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Course.Create(
            "Beginner English Path",
            null,
            null,
            null,
            null,
            null,
            -1,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("estimatedMinutes", exception.ParamName);
    }

    [Fact]
    public void PublishAndUnpublishUpdateStatusAndAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var course = CreateCourse(createdAt);

        var publishedAt = createdAt.AddMinutes(5);
        course.Publish(publishedAt);
        Assert.True(course.IsPublished);
        Assert.Equal(publishedAt, course.UpdatedAt);

        var unpublishedAt = createdAt.AddMinutes(10);
        course.Unpublish(unpublishedAt);
        Assert.False(course.IsPublished);
        Assert.Equal(unpublishedAt, course.UpdatedAt);
    }

    [Fact]
    public void ActivateAndDeactivateUpdateStatusAndAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var course = CreateCourse(createdAt);

        var deactivatedAt = createdAt.AddMinutes(5);
        course.Deactivate(deactivatedAt);
        Assert.False(course.IsActive);
        Assert.Equal(deactivatedAt, course.UpdatedAt);

        var activatedAt = createdAt.AddMinutes(10);
        course.Activate(activatedAt);
        Assert.True(course.IsActive);
        Assert.Equal(activatedAt, course.UpdatedAt);
    }

    [Fact]
    public void AddLessonPreventsDuplicateLessonInsideCourse()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var course = CreateCourse(now);
        var lessonId = Guid.NewGuid();

        course.AddLesson(lessonId, 0, isRequired: true, now.AddMinutes(1));
        course.AddLesson(lessonId, 1, isRequired: false, now.AddMinutes(2));

        Assert.Single(course.Lessons);
        Assert.Equal(lessonId, course.Lessons.Single().LessonId);
        Assert.False(course.Lessons.Single().IsRequired);
    }

    [Fact]
    public void CourseLessonReorderUpdatesSortOrderAndAuditField()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var relation = CourseLesson.Create(Guid.NewGuid(), Guid.NewGuid(), 0, true, now);

        var reorderedAt = now.AddMinutes(5);
        relation.Reorder(3, reorderedAt);

        Assert.Equal(3, relation.SortOrder);
        Assert.Equal(reorderedAt, relation.UpdatedAt);
    }

    private static Course CreateCourse(DateTimeOffset now)
    {
        return Course.Create(
            "Beginner English Path",
            null,
            null,
            null,
            null,
            null,
            0,
            0,
            now);
    }
}
