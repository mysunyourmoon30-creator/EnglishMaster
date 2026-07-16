using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Contracts.Books;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Contracts.Lessons;

namespace EnglishMaster.IntegrationTests.Books;

public sealed class BookEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public BookEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task BookEndpointsSupportChaptersLessonsPublishAndSoftDelete()
    {
        var course = await CreateCourseAsync("Book Integration Course");
        var bookResponse = await client.PostAsJsonAsync(
            "/api/v1/books",
            new CreateBookRequest(
                "Starter Book Integration",
                "First steps",
                "A compact beginner book",
                "A longer book description.",
                "A1",
                null,
                null,
                course.Id,
                "EnglishMaster Team",
                "First",
                "1.0",
                80,
                0));
        Assert.Equal(HttpStatusCode.Created, bookResponse.StatusCode);
        var book = await bookResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(book);
        Assert.Equal("starter-book-integration", book.Slug);
        Assert.False(book.IsPublished);

        var bookSearch = await client.GetFromJsonAsync<BookSearchResponse>(
            $"/api/v1/books?cefrLevel=A1&search=Starter&courseId={course.Id}");
        Assert.Contains(bookSearch!.Items, item => item.Id == book.Id);

        var firstLesson = await CreateLessonAsync("Book First Lesson Integration");
        var secondLesson = await CreateLessonAsync("Book Second Lesson Integration");

        var firstChapterResponse = await client.PostAsJsonAsync(
            $"/api/v1/books/{book.Id}/chapters",
            new CreateBookChapterRequest("First Chapter", "Start here", "## Chapter one", 0));
        Assert.Equal(HttpStatusCode.Created, firstChapterResponse.StatusCode);
        var firstChapter = await firstChapterResponse.Content.ReadFromJsonAsync<BookChapterDto>();

        var secondChapterResponse = await client.PostAsJsonAsync(
            $"/api/v1/books/{book.Id}/chapters",
            new CreateBookChapterRequest("Second Chapter", "Next steps", "## Chapter two", 1));
        Assert.Equal(HttpStatusCode.Created, secondChapterResponse.StatusCode);
        var secondChapter = await secondChapterResponse.Content.ReadFromJsonAsync<BookChapterDto>();

        var chapters = await client.GetFromJsonAsync<IReadOnlyCollection<BookChapterDto>>(
            $"/api/v1/books/{book.Id}/chapters");
        Assert.Equal(2, chapters!.Count);

        var updateChapterResponse = await client.PutAsJsonAsync(
            $"/api/v1/book-chapters/{firstChapter!.Id}",
            new UpdateBookChapterRequest("First Chapter Updated", "Start here", "## Updated", 0, true));
        Assert.Equal(HttpStatusCode.OK, updateChapterResponse.StatusCode);
        var updatedChapter = await updateChapterResponse.Content.ReadFromJsonAsync<BookChapterDto>();
        Assert.Equal("First Chapter Updated", updatedChapter!.Title);

        var addFirstLessonResponse = await client.PostAsync(
            $"/api/v1/book-chapters/{firstChapter.Id}/lessons/{firstLesson.Id}?sortOrder=0",
            content: null);
        Assert.Equal(HttpStatusCode.OK, addFirstLessonResponse.StatusCode);
        var chapterWithFirstLesson = await addFirstLessonResponse.Content.ReadFromJsonAsync<BookChapterDto>();
        Assert.Contains(chapterWithFirstLesson!.Lessons, item => item.LessonId == firstLesson.Id);

        var duplicateLessonResponse = await client.PostAsync(
            $"/api/v1/book-chapters/{firstChapter.Id}/lessons/{firstLesson.Id}?sortOrder=1",
            content: null);
        Assert.Equal(HttpStatusCode.OK, duplicateLessonResponse.StatusCode);
        var chapterAfterDuplicateAdd = await duplicateLessonResponse.Content.ReadFromJsonAsync<BookChapterDto>();
        Assert.Single(chapterAfterDuplicateAdd!.Lessons, item => item.LessonId == firstLesson.Id);

        var addSecondLessonResponse = await client.PostAsync(
            $"/api/v1/book-chapters/{firstChapter.Id}/lessons/{secondLesson.Id}?sortOrder=1",
            content: null);
        Assert.Equal(HttpStatusCode.OK, addSecondLessonResponse.StatusCode);
        var chapterWithLessons = await addSecondLessonResponse.Content.ReadFromJsonAsync<BookChapterDto>();

        var chapterLessons = await client.GetFromJsonAsync<IReadOnlyCollection<BookChapterLessonDto>>(
            $"/api/v1/book-chapters/{firstChapter.Id}/lessons");
        Assert.Equal(2, chapterLessons!.Count);

        var firstRelation = chapterWithLessons!.Lessons.Single(item => item.LessonId == firstLesson.Id);
        var secondRelation = chapterWithLessons.Lessons.Single(item => item.LessonId == secondLesson.Id);
        var reorderLessonsResponse = await client.PostAsJsonAsync(
            $"/api/v1/book-chapters/{firstChapter.Id}/lessons/reorder",
            new ReorderBookChapterLessonsRequest([secondRelation.Id, firstRelation.Id]));
        Assert.Equal(HttpStatusCode.OK, reorderLessonsResponse.StatusCode);
        var reorderedLessons = await reorderLessonsResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<BookChapterLessonDto>>();
        Assert.Equal(secondRelation.Id, reorderedLessons!.First().Id);

        var reorderChaptersResponse = await client.PostAsJsonAsync(
            $"/api/v1/books/{book.Id}/chapters/reorder",
            new ReorderBookChaptersRequest([secondChapter!.Id, firstChapter.Id]));
        Assert.Equal(HttpStatusCode.OK, reorderChaptersResponse.StatusCode);
        var reorderedChapters = await reorderChaptersResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<BookChapterDto>>();
        Assert.Equal(secondChapter.Id, reorderedChapters!.First().Id);

        var publishResponse = await client.PostAsync($"/api/v1/books/{book.Id}/publish", content: null);
        Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);
        var publishedBook = await publishResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.True(publishedBook!.IsPublished);

        var unpublishResponse = await client.PostAsync($"/api/v1/books/{book.Id}/unpublish", content: null);
        Assert.Equal(HttpStatusCode.OK, unpublishResponse.StatusCode);
        var unpublishedBook = await unpublishResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.False(unpublishedBook!.IsPublished);

        var updateBookResponse = await client.PutAsJsonAsync(
            $"/api/v1/books/{book.Id}",
            new UpdateBookRequest(
                "Starter Book Integration Updated",
                "First steps",
                "A compact beginner book",
                "A longer book description.",
                "A1",
                null,
                null,
                course.Id,
                "EnglishMaster Team",
                "First",
                "1.1",
                90,
                1,
                false,
                true));
        Assert.Equal(HttpStatusCode.OK, updateBookResponse.StatusCode);
        var updatedBook = await updateBookResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.Equal("Starter Book Integration Updated", updatedBook!.Title);

        var removeLessonResponse = await client.DeleteAsync(
            $"/api/v1/book-chapters/{firstChapter.Id}/lessons/{firstLesson.Id}");
        Assert.Equal(HttpStatusCode.NoContent, removeLessonResponse.StatusCode);

        var deleteChapterResponse = await client.DeleteAsync($"/api/v1/book-chapters/{secondChapter.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteChapterResponse.StatusCode);

        var deleteBookResponse = await client.DeleteAsync($"/api/v1/books/{book.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteBookResponse.StatusCode);

        var activeBookSearch = await client.GetFromJsonAsync<BookSearchResponse>(
            "/api/v1/books?search=Updated");
        Assert.DoesNotContain(activeBookSearch!.Items, item => item.Id == book.Id);

        var inactiveBookSearch = await client.GetFromJsonAsync<BookSearchResponse>(
            "/api/v1/books?search=Updated&isActive=false");
        Assert.Contains(inactiveBookSearch!.Items, item => item.Id == book.Id);
    }

    [Fact]
    public async Task CreateBookReturnsValidationProblemWhenTitleIsMissing()
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/books",
            new CreateBookRequest(" ", null, null, null, null, null, null, null, null, null, null, 0, 0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateBookReturnsValidationProblemWhenTitleAlreadyExists()
    {
        var title = $"Duplicate Book Title {Guid.NewGuid():N}";
        var firstResponse = await client.PostAsJsonAsync(
            "/api/v1/books",
            new CreateBookRequest(title, null, null, null, null, null, null, null, null, null, null, 0, 0));
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await client.PostAsJsonAsync(
            "/api/v1/books",
            new CreateBookRequest(title, null, null, null, null, null, null, null, null, null, null, 0, 1));

        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    private async Task<CourseDto> CreateCourseAsync(string title)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/courses",
            new CreateCourseRequest(
                title,
                "Course summary",
                null,
                "A1",
                null,
                null,
                120,
                0));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<CourseDto>())!;
    }

    private async Task<LessonDto> CreateLessonAsync(string title)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/lessons",
            new CreateLessonRequest(
                title,
                "Lesson summary",
                null,
                "A1",
                null,
                null,
                15,
                0));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<LessonDto>())!;
    }
}
