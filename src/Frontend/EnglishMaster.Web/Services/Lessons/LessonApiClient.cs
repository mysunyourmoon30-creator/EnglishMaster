using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.LessonSections;
using EnglishMaster.Contracts.Lessons;

namespace EnglishMaster.Web.Services.Lessons;

public sealed class LessonApiClient : ILessonApiClient
{
    private readonly HttpClient httpClient;

    public LessonApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<LessonSearchResponse> SearchAsync(
        LessonSearchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildSearchEndpoint(request), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<LessonSearchResponse>(
            cancellationToken: cancellationToken) ?? new LessonSearchResponse(
                [],
                request.PageNumber,
                request.PageSize,
                0,
                0,
                false,
                false);
    }

    public async Task<LessonDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/lessons/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<LessonDto>(cancellationToken: cancellationToken);
    }

    public async Task<LessonDto> CreateAsync(CreateLessonRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/lessons", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<LessonDto>(response, cancellationToken);
    }

    public async Task<LessonDto> UpdateAsync(
        Guid id,
        UpdateLessonRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/lessons/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<LessonDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/lessons/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<LessonDto> PublishAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/lessons/{id}/publish", content: null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<LessonDto>(response, cancellationToken);
    }

    public async Task<LessonDto> UnpublishAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/lessons/{id}/unpublish", content: null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<LessonDto>(response, cancellationToken);
    }

    public async Task<LessonDto> AddWordAsync(
        Guid lessonId,
        Guid wordId,
        int sortOrder,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(
            $"api/v1/lessons/{lessonId}/words/{wordId}?sortOrder={sortOrder}",
            content: null,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<LessonDto>(response, cancellationToken);
    }

    public async Task RemoveWordAsync(Guid lessonId, Guid wordId, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/lessons/{lessonId}/words/{wordId}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<LessonDto> AddGrammarRuleAsync(
        Guid lessonId,
        Guid grammarRuleId,
        int sortOrder,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(
            $"api/v1/lessons/{lessonId}/grammar-rules/{grammarRuleId}?sortOrder={sortOrder}",
            content: null,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<LessonDto>(response, cancellationToken);
    }

    public async Task RemoveGrammarRuleAsync(
        Guid lessonId,
        Guid grammarRuleId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync(
            $"api/v1/lessons/{lessonId}/grammar-rules/{grammarRuleId}",
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<LessonSectionDto>> GetSectionsAsync(
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/lessons/{lessonId}/sections", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<LessonSectionDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    public async Task<LessonSectionDto> AddSectionAsync(
        Guid lessonId,
        CreateLessonSectionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/lessons/{lessonId}/sections",
            request,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<LessonSectionDto>(response, cancellationToken);
    }

    public async Task<LessonSectionDto> UpdateSectionAsync(
        Guid id,
        UpdateLessonSectionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/lesson-sections/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<LessonSectionDto>(response, cancellationToken);
    }

    public async Task DeleteSectionAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/lesson-sections/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<LessonSectionDto>> ReorderSectionsAsync(
        Guid lessonId,
        IReadOnlyList<Guid> orderedSectionIds,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/lessons/{lessonId}/sections/reorder",
            new ReorderLessonSectionsRequest(orderedSectionIds),
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<LessonSectionDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    private static string BuildSearchEndpoint(LessonSearchRequest request)
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
            ? "api/v1/lessons"
            : $"api/v1/lessons?{string.Join("&", parameters)}";
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
