using System.Net.Http.Json;
using EnglishMaster.Contracts.PublicSearch;

namespace EnglishMaster.Web.Services.PublicSearch;

public sealed class PublicSearchApiClient : IPublicSearchApiClient
{
    private readonly HttpClient httpClient;

    public PublicSearchApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<PublicSearchResponse> SearchAsync(string? query, string? contentType, string? cefrLevel, Guid? categoryId, Guid? tagId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var parameters = new List<string>
        {
            $"pageNumber={pageNumber}",
            $"pageSize={pageSize}"
        };
        Add(parameters, "q", query);
        Add(parameters, "contentType", contentType);
        Add(parameters, "cefrLevel", cefrLevel);
        Add(parameters, "categoryId", categoryId?.ToString());
        Add(parameters, "tagId", tagId?.ToString());

        var response = await httpClient.GetAsync($"api/v1/public/search?{string.Join("&", parameters)}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PublicSearchResponse>(cancellationToken: cancellationToken)
            ?? new PublicSearchResponse([], pageNumber, pageSize, 0, 0, false, false);
    }

    public async Task<PublicSearchFiltersResponse> GetFiltersAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/public/search/filters", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PublicSearchFiltersResponse>(cancellationToken: cancellationToken)
            ?? new PublicSearchFiltersResponse([], [], [], []);
    }

    private static void Add(ICollection<string> parameters, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            parameters.Add($"{key}={Uri.EscapeDataString(value)}");
        }
    }
}

