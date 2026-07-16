using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.MinimalPairs;
using EnglishMaster.Contracts.Pronunciations;

namespace EnglishMaster.Web.Services.Pronunciations;

public sealed class PronunciationsApiClient : IPronunciationsApiClient
{
    private readonly HttpClient httpClient;

    public PronunciationsApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<PronunciationSearchResponse> SearchAsync(
        PronunciationSearchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildSearchEndpoint(request), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        var pronunciations = await response.Content.ReadFromJsonAsync<PronunciationSearchResponse>(
            cancellationToken: cancellationToken);

        return pronunciations ?? new PronunciationSearchResponse(
            [],
            request.PageNumber,
            request.PageSize,
            0,
            0,
            false,
            false);
    }

    public async Task<PronunciationDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/pronunciations/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<PronunciationDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<PronunciationDto?> GetByWordIdAsync(
        Guid wordId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/words/{wordId}/pronunciation", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<PronunciationDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<PronunciationDto> CreateAsync(
        CreatePronunciationRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/pronunciations", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<PronunciationDto>(response, cancellationToken);
    }

    public async Task<PronunciationDto> UpdateAsync(
        Guid id,
        UpdatePronunciationRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/pronunciations/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<PronunciationDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/pronunciations/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<MinimalPairDto>> GetMinimalPairsAsync(
        Guid pronunciationId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"api/v1/pronunciations/{pronunciationId}/minimal-pairs",
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<MinimalPairDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    public async Task<MinimalPairDto> AddMinimalPairAsync(
        Guid pronunciationId,
        CreateMinimalPairRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/pronunciations/{pronunciationId}/minimal-pairs",
            request,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<MinimalPairDto>(response, cancellationToken);
    }

    public async Task<MinimalPairDto> UpdateMinimalPairAsync(
        Guid id,
        UpdateMinimalPairRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/minimal-pairs/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<MinimalPairDto>(response, cancellationToken);
    }

    public async Task DeleteMinimalPairAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/minimal-pairs/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    private static string BuildSearchEndpoint(PronunciationSearchRequest request)
    {
        var parameters = new List<string>();
        Add(parameters, "search", request.Search);

        if (request.WordId.HasValue)
        {
            Add(parameters, "wordId", request.WordId.Value.ToString());
        }

        if (request.IsActive.HasValue)
        {
            Add(parameters, "isActive", request.IsActive.Value ? "true" : "false");
        }

        Add(parameters, "pageNumber", request.PageNumber.ToString());
        Add(parameters, "pageSize", request.PageSize.ToString());

        return parameters.Count == 0
            ? "api/v1/pronunciations"
            : $"api/v1/pronunciations?{string.Join("&", parameters)}";
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
