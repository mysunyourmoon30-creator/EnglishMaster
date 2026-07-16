using EnglishMaster.Application.Features.Courses.Commands;
using EnglishMaster.Application.Features.Courses.Queries;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;

namespace EnglishMaster.UnitTests.Courses;

public sealed class CourseUseCaseTests
{
    [Fact]
    public async Task CreateCourseCreatesCourseWhenInputIsValid()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var courses = new FakeCourseRepository();
        var handler = new CreateCourseCommandHandler(
            courses,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateCourseCommand("Beginner English Path", "Learn the basics", null, "A1", null, null, 120, 0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(courses.Courses);
        Assert.Equal("beginner-english-path", result.Value!.Slug);
    }

    [Fact]
    public async Task CreateCourseReturnsValidationErrorWhenTitleSlugAlreadyExists()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var courses = new FakeCourseRepository();
        courses.Courses.Add(CreateCourse("Beginner English Path", now));
        var handler = new CreateCourseCommandHandler(
            courses,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateCourseCommand("Beginner English Path", null, null, null, null, null, 0, 0),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "Title");
        Assert.Single(courses.Courses);
    }

    [Fact]
    public async Task PublishAndUnpublishCourseUpdateCourse()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var courses = new FakeCourseRepository();
        var course = CreateCourse("Beginner English Path", now);
        courses.Courses.Add(course);

        var publishHandler = new PublishCourseCommandHandler(
            courses,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));
        var publishResult = await publishHandler.HandleAsync(new PublishCourseCommand(course.Id), CancellationToken.None);

        Assert.True(publishResult.IsSuccess);
        Assert.True(course.IsPublished);

        var unpublishHandler = new UnpublishCourseCommandHandler(
            courses,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(2)));
        var unpublishResult = await unpublishHandler.HandleAsync(new UnpublishCourseCommand(course.Id), CancellationToken.None);

        Assert.True(unpublishResult.IsSuccess);
        Assert.False(course.IsPublished);
    }

    [Fact]
    public async Task ActivateAndDeactivateCourseUpdateCourse()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var courses = new FakeCourseRepository();
        var course = CreateCourse("Beginner English Path", now);
        courses.Courses.Add(course);

        var deactivateHandler = new DeactivateCourseCommandHandler(
            courses,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));
        var deactivateResult = await deactivateHandler.HandleAsync(new DeactivateCourseCommand(course.Id), CancellationToken.None);

        Assert.True(deactivateResult.IsSuccess);
        Assert.False(course.IsActive);

        var activateHandler = new ActivateCourseCommandHandler(
            courses,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(2)));
        var activateResult = await activateHandler.HandleAsync(new ActivateCourseCommand(course.Id), CancellationToken.None);

        Assert.True(activateResult.IsSuccess);
        Assert.True(course.IsActive);
    }

    [Fact]
    public async Task AddLessonToCourseAddsLessonWhenLessonIsActive()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var courses = new FakeCourseRepository();
        var course = CreateCourse("Beginner English Path", now);
        courses.Courses.Add(course);
        var lessons = new FakeLessonRepository();
        var lesson = CreateLesson("Daily Routines", now);
        lessons.Lessons.Add(lesson);
        var handler = new AddLessonToCourseCommandHandler(
            courses,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            lessons,
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new AddLessonToCourseCommand(course.Id, lesson.Id, 0, true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(course.Lessons);
        Assert.Contains(result.Value!.Lessons, item => item.LessonId == lesson.Id);
    }

    [Fact]
    public async Task AddLessonToCoursePreventsDuplicateLesson()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var courses = new FakeCourseRepository();
        var course = CreateCourse("Beginner English Path", now);
        courses.Courses.Add(course);
        var lessons = new FakeLessonRepository();
        var lesson = CreateLesson("Daily Routines", now);
        lessons.Lessons.Add(lesson);
        var handler = new AddLessonToCourseCommandHandler(
            courses,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            lessons,
            new FixedTimeProvider(now.AddMinutes(1)));

        await handler.HandleAsync(new AddLessonToCourseCommand(course.Id, lesson.Id, 0, true), CancellationToken.None);
        var result = await handler.HandleAsync(new AddLessonToCourseCommand(course.Id, lesson.Id, 1, false), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(course.Lessons);
        Assert.False(course.Lessons.Single().IsRequired);
    }

    [Fact]
    public async Task ReorderCourseLessonsUpdatesSortOrder()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var courses = new FakeCourseRepository();
        var course = CreateCourse("Beginner English Path", now);
        courses.Courses.Add(course);
        var lessons = new FakeLessonRepository();
        var firstLesson = CreateLesson("Daily Routines", now);
        var secondLesson = CreateLesson("Conditionals", now);
        lessons.Lessons.Add(firstLesson);
        lessons.Lessons.Add(secondLesson);
        var firstRelation = course.AddLesson(firstLesson.Id, 0, true, now);
        var secondRelation = course.AddLesson(secondLesson.Id, 1, true, now);
        var handler = new ReorderCourseLessonsCommandHandler(
            courses,
            lessons,
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new ReorderCourseLessonsCommand(course.Id, [secondRelation.Id, firstRelation.Id]),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(secondRelation.Id, result.Value!.First().Id);
        Assert.Equal(0, secondRelation.SortOrder);
        Assert.Equal(1, firstRelation.SortOrder);
    }

    [Fact]
    public async Task SearchCoursesFiltersByCefr()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var courses = new FakeCourseRepository();
        courses.Courses.Add(CreateCourse("Beginner English Path", now, CefrLevel.A1));
        courses.Courses.Add(CreateCourse("Intermediate Grammar Path", now, CefrLevel.B1));
        var handler = new SearchCoursesQueryHandler(
            courses,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeLessonRepository());

        var result = await handler.HandleAsync(
            new SearchCoursesQuery(null, "B1", null, null, true, 1, 20),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Intermediate Grammar Path", result.Value.Items.Single().Title);
    }

    [Fact]
    public async Task SearchCoursesFiltersByPublishedStatus()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var courses = new FakeCourseRepository();
        var draft = CreateCourse("Draft Course", now);
        var published = CreateCourse("Published Course", now);
        published.Publish(now);
        courses.Courses.Add(draft);
        courses.Courses.Add(published);
        var handler = new SearchCoursesQueryHandler(
            courses,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeLessonRepository());

        var result = await handler.HandleAsync(
            new SearchCoursesQuery(null, null, null, true, true, 1, 20),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Published Course", result.Value.Items.Single().Title);
    }

    private static Course CreateCourse(string title, DateTimeOffset now, CefrLevel? cefrLevel = null)
    {
        return Course.Create(title, null, null, cefrLevel, null, null, 0, 0, now);
    }

    private static Lesson CreateLesson(string title, DateTimeOffset now)
    {
        return Lesson.Create(title, null, null, null, null, null, 0, 0, now);
    }
}
