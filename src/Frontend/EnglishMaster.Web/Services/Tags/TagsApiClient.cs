using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Tags;
using EnglishMaster.Web.Services;

namespace EnglishMaster.Web.Services.Tags;

public sealed class TagsApiClient : ITagsApiClient
{
    private readonly HttpClient httpClient;

    public TagsApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<TagSearchResponse> SearchAsync(
        string? search,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        var parameters = new List<string>();
        Add(parameters, "search", search);
        if (isActive.HasValue)
        {
            Add(parameters, "isActive", isActive.Value ? "true" : "false");
        }

        var endpoint = parameters.Count == 0
            ? "api/v1/tags"
            : $"api/v1/tags?{string.Join("&", parameters)}";
        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TagSearchResponse>(
            cancellationToken: cancellationToken) ?? new TagSearchResponse([]);
    }

    public async Task<TagDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/tags/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TagDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<TagDto> CreateAsync(
        CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/tags", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<TagDto>(response, cancellationToken);
    }

    public async Task<TagDto> UpdateAsync(
        Guid id,
        UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/tags/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<TagDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/tags/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
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
