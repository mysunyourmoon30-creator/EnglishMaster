using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Categories;
using EnglishMaster.Web.Services;

namespace EnglishMaster.Web.Services.Categories;

public sealed class CategoriesApiClient : ICategoriesApiClient
{
    private readonly HttpClient httpClient;

    public CategoriesApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<CategorySearchResponse> SearchAsync(
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
            ? "api/v1/categories"
            : $"api/v1/categories?{string.Join("&", parameters)}";
        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<CategorySearchResponse>(
            cancellationToken: cancellationToken) ?? new CategorySearchResponse([]);
    }

    public async Task<CategoryDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/categories/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<CategoryDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<CategoryDto> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/categories", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<CategoryDto>(response, cancellationToken);
    }

    public async Task<CategoryDto> UpdateAsync(
        Guid id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/categories/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<CategoryDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/categories/{id}", cancellationToken);
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
