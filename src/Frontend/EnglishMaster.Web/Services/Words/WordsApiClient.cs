using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Words;
using EnglishMaster.Web.Services;

namespace EnglishMaster.Web.Services.Words;

public sealed class WordsApiClient : IWordsApiClient
{
    private readonly HttpClient httpClient;

    public WordsApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<WordSearchResponse> SearchAsync(
        WordSearchRequest request,
        CancellationToken cancellationToken)
    {
        var endpoint = BuildSearchEndpoint(request);

        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        var words = await response.Content.ReadFromJsonAsync<WordSearchResponse>(
            cancellationToken: cancellationToken);

        return words ?? new WordSearchResponse([], request.PageNumber, request.PageSize, 0, 0, false, false);
    }

    private static string BuildSearchEndpoint(WordSearchRequest request)
    {
        var parameters = new List<string>();
        Add(parameters, "search", request.Search);
        Add(parameters, "cefrLevel", request.CefrLevel);
        Add(parameters, "partOfSpeech", request.PartOfSpeech);

        if (request.IsActive.HasValue)
        {
            Add(parameters, "isActive", request.IsActive.Value ? "true" : "false");
        }

        if (request.CategoryId.HasValue)
        {
            Add(parameters, "categoryId", request.CategoryId.Value.ToString());
        }

        if (request.TagId.HasValue)
        {
            Add(parameters, "tagId", request.TagId.Value.ToString());
        }

        Add(parameters, "pageNumber", request.PageNumber.ToString());
        Add(parameters, "pageSize", request.PageSize.ToString());
        Add(parameters, "sortBy", request.SortBy);
        Add(parameters, "sortDirection", request.SortDirection);

        return parameters.Count == 0
            ? "api/v1/words"
            : $"api/v1/words?{string.Join("&", parameters)}";
    }

    private static void Add(ICollection<string> parameters, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        parameters.Add($"{name}={Uri.EscapeDataString(value.Trim())}");
    }

    public async Task<WordDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/words/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<WordDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<WordDto> CreateAsync(
        CreateWordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/words", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<WordDto>(response, cancellationToken);
    }

    public async Task<WordDto> UpdateAsync(
        Guid id,
        UpdateWordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/words/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<WordDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/words/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }
}
