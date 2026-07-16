using EnglishMaster.Application.Features.BookChapters.Commands;
using EnglishMaster.Application.Features.Books.Commands;
using EnglishMaster.Application.Features.Books.Queries;
using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;

namespace EnglishMaster.UnitTests.Books;

public sealed class BookUseCaseTests
{
    [Fact]
    public async Task CreateBookCreatesBookWhenInputIsValid()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var books = new FakeBookRepository();
        var handler = new CreateBookCommandHandler(
            books,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeCourseRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateBookCommand(
                "Starter Book",
                "First steps",
                "A short summary",
                null,
                "A1",
                null,
                null,
                null,
                "EnglishMaster Team",
                "First",
                "1.0",
                80,
                0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(books.Books);
        Assert.Equal("starter-book", result.Value!.Slug);
        Assert.Equal("First steps", result.Value.Subtitle);
    }

    [Fact]
    public async Task CreateBookReturnsValidationErrorWhenTitleSlugAlreadyExists()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var books = new FakeBookRepository();
        books.Books.Add(CreateBook("Starter Book", now));
        var handler = new CreateBookCommandHandler(
            books,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeCourseRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateBookCommand("Starter Book", null, null, null, null, null, null, null, null, null, null, 0, 0),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "Title");
        Assert.Single(books.Books);
    }

    [Fact]
    public async Task UpdateBookChangesBookWhenInputIsValid()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var books = new FakeBookRepository();
        var book = CreateBook("Starter Book", now);
        books.Books.Add(book);
        var handler = new UpdateBookCommandHandler(
            books,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeCourseRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new UpdateBookCommand(
                book.Id,
                "Starter Book Updated",
                "Updated subtitle",
                null,
                null,
                "A2",
                null,
                null,
                null,
                null,
                null,
                null,
                90,
                1,
                false,
                true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Starter Book Updated", book.Title);
        Assert.Equal("starter-book-updated", book.Slug);
        Assert.Equal("A2", result.Value!.CefrLevel);
    }

    [Fact]
    public async Task PublishAndUnpublishBookUpdateBook()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var books = new FakeBookRepository();
        var book = CreateBook("Starter Book", now);
        books.Books.Add(book);

        var publishHandler = new PublishBookCommandHandler(
            books,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeCourseRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));
        var publishResult = await publishHandler.HandleAsync(new PublishBookCommand(book.Id), CancellationToken.None);

        Assert.True(publishResult.IsSuccess);
        Assert.True(book.IsPublished);

        var unpublishHandler = new UnpublishBookCommandHandler(
            books,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeCourseRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(2)));
        var unpublishResult = await unpublishHandler.HandleAsync(new UnpublishBookCommand(book.Id), CancellationToken.None);

        Assert.True(unpublishResult.IsSuccess);
        Assert.False(book.IsPublished);
    }

    [Fact]
    public async Task ActivateAndDeactivateBookUpdateBook()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var books = new FakeBookRepository();
        var book = CreateBook("Starter Book", now);
        books.Books.Add(book);

        var deactivateHandler = new DeactivateBookCommandHandler(
            books,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeCourseRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));
        var deactivateResult = await deactivateHandler.HandleAsync(new DeactivateBookCommand(book.Id), CancellationToken.None);

        Assert.True(deactivateResult.IsSuccess);
        Assert.False(book.IsActive);

        var activateHandler = new ActivateBookCommandHandler(
            books,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeCourseRepository(),
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(2)));
        var activateResult = await activateHandler.HandleAsync(new ActivateBookCommand(book.Id), CancellationToken.None);

        Assert.True(activateResult.IsSuccess);
        Assert.True(book.IsActive);
    }

    [Fact]
    public async Task AddBookChapterAddsChapterToBook()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var books = new FakeBookRepository();
        var chapters = new FakeBookChapterRepository();
        var book = CreateBook("Starter Book", now);
        books.Books.Add(book);
        var handler = new AddBookChapterCommandHandler(
            chapters,
            books,
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new AddBookChapterCommand(book.Id, "First Chapter", "Intro", "Content", 0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(chapters.Chapters);
        Assert.Equal("first-chapter", result.Value!.Slug);
    }

    [Fact]
    public async Task AddLessonToBookChapterAddsLessonWhenLessonIsActive()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var chapters = new FakeBookChapterRepository();
        var chapter = CreateChapter(Guid.NewGuid(), "First Chapter", now);
        chapters.Chapters.Add(chapter);
        var lessons = new FakeLessonRepository();
        var lesson = CreateLesson("Daily Routines", now);
        lessons.Lessons.Add(lesson);
        var handler = new AddLessonToBookChapterCommandHandler(
            chapters,
            lessons,
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new AddLessonToBookChapterCommand(chapter.Id, lesson.Id, 0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(chapter.Lessons);
        Assert.Contains(result.Value!.Lessons, item => item.LessonId == lesson.Id);
    }

    [Fact]
    public async Task AddLessonToBookChapterPreventsDuplicateLesson()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var chapters = new FakeBookChapterRepository();
        var chapter = CreateChapter(Guid.NewGuid(), "First Chapter", now);
        chapters.Chapters.Add(chapter);
        var lessons = new FakeLessonRepository();
        var lesson = CreateLesson("Daily Routines", now);
        lessons.Lessons.Add(lesson);
        var handler = new AddLessonToBookChapterCommandHandler(
            chapters,
            lessons,
            new FixedTimeProvider(now.AddMinutes(1)));

        await handler.HandleAsync(new AddLessonToBookChapterCommand(chapter.Id, lesson.Id, 0), CancellationToken.None);
        var result = await handler.HandleAsync(new AddLessonToBookChapterCommand(chapter.Id, lesson.Id, 1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(chapter.Lessons);
    }

    [Fact]
    public async Task ReorderBookChaptersUpdatesSortOrder()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var books = new FakeBookRepository();
        var chapters = new FakeBookChapterRepository();
        var book = CreateBook("Starter Book", now);
        books.Books.Add(book);
        var first = CreateChapter(book.Id, "First Chapter", now, 0);
        var second = CreateChapter(book.Id, "Second Chapter", now, 1);
        chapters.Chapters.Add(first);
        chapters.Chapters.Add(second);
        var handler = new ReorderBookChaptersCommandHandler(
            chapters,
            books,
            new FakeLessonRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new ReorderBookChaptersCommand(book.Id, [second.Id, first.Id]),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(second.Id, result.Value!.First().Id);
        Assert.Equal(0, second.SortOrder);
        Assert.Equal(1, first.SortOrder);
    }

    [Fact]
    public async Task ReorderBookChapterLessonsUpdatesSortOrder()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var chapters = new FakeBookChapterRepository();
        var chapter = CreateChapter(Guid.NewGuid(), "First Chapter", now);
        var lessons = new FakeLessonRepository();
        var firstLesson = CreateLesson("Daily Routines", now);
        var secondLesson = CreateLesson("Conditionals", now);
        lessons.Lessons.Add(firstLesson);
        lessons.Lessons.Add(secondLesson);
        var firstRelation = chapter.AddLesson(firstLesson.Id, 0, now);
        var secondRelation = chapter.AddLesson(secondLesson.Id, 1, now);
        chapters.Chapters.Add(chapter);
        var handler = new ReorderBookChapterLessonsCommandHandler(
            chapters,
            lessons,
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new ReorderBookChapterLessonsCommand(chapter.Id, [secondRelation.Id, firstRelation.Id]),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(secondRelation.Id, result.Value!.First().Id);
        Assert.Equal(0, secondRelation.SortOrder);
        Assert.Equal(1, firstRelation.SortOrder);
    }

    [Fact]
    public async Task SearchBooksFiltersByCefr()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var books = new FakeBookRepository();
        books.Books.Add(CreateBook("Starter Book", now, CefrLevel.A1));
        books.Books.Add(CreateBook("Intermediate Book", now, CefrLevel.B1));
        var handler = new SearchBooksQueryHandler(
            books,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeCourseRepository(),
            new FakeLessonRepository());

        var result = await handler.HandleAsync(
            new SearchBooksQuery(null, "B1", null, null, null, true, 1, 20),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Intermediate Book", result.Value.Items.Single().Title);
    }

    [Fact]
    public async Task SearchBooksFiltersByPublishedStatus()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var books = new FakeBookRepository();
        var draft = CreateBook("Draft Book", now);
        var published = CreateBook("Published Book", now);
        published.Publish(now);
        books.Books.Add(draft);
        books.Books.Add(published);
        var handler = new SearchBooksQueryHandler(
            books,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeCourseRepository(),
            new FakeLessonRepository());

        var result = await handler.HandleAsync(
            new SearchBooksQuery(null, null, null, null, true, true, 1, 20),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Published Book", result.Value.Items.Single().Title);
    }

    private static Book CreateBook(
        string title,
        DateTimeOffset now,
        CefrLevel? cefrLevel = null)
    {
        return Book.Create(title, null, null, null, cefrLevel, null, null, null, null, null, null, 0, 0, now);
    }

    private static BookChapter CreateChapter(
        Guid bookId,
        string title,
        DateTimeOffset now,
        int sortOrder = 0)
    {
        return BookChapter.Create(bookId, title, null, null, sortOrder, now);
    }

    private static Lesson CreateLesson(string title, DateTimeOffset now)
    {
        return Lesson.Create(title, null, null, null, null, null, 0, 0, now);
    }
}
