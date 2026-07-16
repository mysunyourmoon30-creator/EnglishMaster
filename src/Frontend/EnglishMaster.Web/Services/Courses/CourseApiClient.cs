using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Courses;

namespace EnglishMaster.Web.Services.Courses;

public sealed class CourseApiClient : ICourseApiClient
{
    private readonly HttpClient httpClient;

    public CourseApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<CourseSearchResponse> SearchAsync(
        CourseSearchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildSearchEndpoint(request), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<CourseSearchResponse>(
            cancellationToken: cancellationToken) ?? new CourseSearchResponse(
                [],
                request.PageNumber,
                request.PageSize,
                0,
                0,
                false,
                false);
    }

    public async Task<CourseDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/courses/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<CourseDto>(cancellationToken: cancellationToken);
    }

    public async Task<CourseDto> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/courses", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<CourseDto>(response, cancellationToken);
    }

    public async Task<CourseDto> UpdateAsync(
        Guid id,
        UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/courses/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<CourseDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/courses/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<CourseDto> PublishAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/courses/{id}/publish", content: null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<CourseDto>(response, cancellationToken);
    }

    public async Task<CourseDto> UnpublishAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/courses/{id}/unpublish", content: null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<CourseDto>(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CourseLessonDto>> GetLessonsAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/courses/{courseId}/lessons", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<CourseLessonDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    public async Task<CourseDto> AddLessonAsync(
        Guid courseId,
        Guid lessonId,
        int sortOrder,
        bool isRequired,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(
            $"api/v1/courses/{courseId}/lessons/{lessonId}?sortOrder={sortOrder}&isRequired={(isRequired ? "true" : "false")}",
            content: null,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<CourseDto>(response, cancellationToken);
    }

    public async Task RemoveLessonAsync(Guid courseId, Guid lessonId, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/courses/{courseId}/lessons/{lessonId}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CourseLessonDto>> ReorderLessonsAsync(
        Guid courseId,
        IReadOnlyList<Guid> orderedCourseLessonIds,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/courses/{courseId}/lessons/reorder",
            new ReorderCourseLessonsRequest(orderedCourseLessonIds),
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<CourseLessonDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    private static string BuildSearchEndpoint(CourseSearchRequest request)
    {
        var parameters = new List<string>();
        Add(parameters, "search", request.Search);
        Add(parameters, "cefrLevel", request.CefrLevel);
        if (request.CategoryId.HasValue)
        {
            Add(parameters, "categoryId", request.CategoryId.Value.ToString());
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
            ? "api/v1/courses"
            : $"api/v1/courses?{string.Join("&", parameters)}";
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
