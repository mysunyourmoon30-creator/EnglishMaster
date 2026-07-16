using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Contracts.Books;

namespace EnglishMaster.Web.Services.Books;

public sealed class BookApiClient : IBookApiClient
{
    private readonly HttpClient httpClient;

    public BookApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<BookSearchResponse> SearchAsync(
        BookSearchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildSearchEndpoint(request), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<BookSearchResponse>(
            cancellationToken: cancellationToken) ?? new BookSearchResponse(
                [],
                request.PageNumber,
                request.PageSize,
                0,
                0,
                false,
                false);
    }

    public async Task<BookDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/books/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<BookDto>(cancellationToken: cancellationToken);
    }

    public async Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/books", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<BookDto>(response, cancellationToken);
    }

    public async Task<BookDto> UpdateAsync(
        Guid id,
        UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/books/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<BookDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/books/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<BookDto> PublishAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/books/{id}/publish", content: null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<BookDto>(response, cancellationToken);
    }

    public async Task<BookDto> UnpublishAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/books/{id}/unpublish", content: null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<BookDto>(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<BookChapterDto>> GetChaptersAsync(
        Guid bookId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/books/{bookId}/chapters", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<BookChapterDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    public async Task<BookChapterDto> AddChapterAsync(
        Guid bookId,
        CreateBookChapterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/books/{bookId}/chapters",
            request,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<BookChapterDto>(response, cancellationToken);
    }

    public async Task<BookChapterDto> UpdateChapterAsync(
        Guid id,
        UpdateBookChapterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync(
            $"api/v1/book-chapters/{id}",
            request,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<BookChapterDto>(response, cancellationToken);
    }

    public async Task DeleteChapterAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/book-chapters/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<BookChapterDto>> ReorderChaptersAsync(
        Guid bookId,
        IReadOnlyList<Guid> orderedChapterIds,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/books/{bookId}/chapters/reorder",
            new ReorderBookChaptersRequest(orderedChapterIds),
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<BookChapterDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    public async Task<IReadOnlyCollection<BookChapterLessonDto>> GetChapterLessonsAsync(
        Guid chapterId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/book-chapters/{chapterId}/lessons", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<BookChapterLessonDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    public async Task<BookChapterDto> AddLessonAsync(
        Guid chapterId,
        Guid lessonId,
        int sortOrder,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(
            $"api/v1/book-chapters/{chapterId}/lessons/{lessonId}?sortOrder={sortOrder}",
            content: null,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<BookChapterDto>(response, cancellationToken);
    }

    public async Task RemoveLessonAsync(
        Guid chapterId,
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync(
            $"api/v1/book-chapters/{chapterId}/lessons/{lessonId}",
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<BookChapterLessonDto>> ReorderChapterLessonsAsync(
        Guid chapterId,
        IReadOnlyList<Guid> orderedBookChapterLessonIds,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/book-chapters/{chapterId}/lessons/reorder",
            new ReorderBookChapterLessonsRequest(orderedBookChapterLessonIds),
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<BookChapterLessonDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    private static string BuildSearchEndpoint(BookSearchRequest request)
    {
        var parameters = new List<string>();
        Add(parameters, "search", request.Search);
        Add(parameters, "cefrLevel", request.CefrLevel);
        if (request.CategoryId.HasValue)
        {
            Add(parameters, "categoryId", request.CategoryId.Value.ToString());
        }

        if (request.CourseId.HasValue)
        {
            Add(parameters, "courseId", request.CourseId.Value.ToString());
        }

        if (request.IsPublished.HasValue)
        {
            Add(parameters, "isPublished", request.IsPublished.Value ? "true" : "false");
        }

        if (request.IsActive.HasValue)
        {
            Add(parameters, "isActive", request.IsActive.Value ? "true" : "false");
        }

        Add(parameters, "pageNumber", request.PageNumber.ToString());
        Add(parameters, "pageSize", request.PageSize.ToString());
        Add(parameters, "sortBy", request.SortBy);
        Add(parameters, "sortDirection", request.SortDirection);

        return parameters.Count == 0
            ? "api/v1/books"
            : $"api/v1/books?{string.Join("&", parameters)}";
    }

    private static void Add(ICollection<string> parameters, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        parameters.Add($"{name}={Uri.EscapeDataString(value.Trim())}");
    }
}
