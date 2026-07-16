using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Media;
using EnglishMaster.Web.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace EnglishMaster.Web.Services.Media;

public sealed class MediaApiClient : IMediaApiClient
{
    private const long MaximumUploadBytes = 25 * 1024 * 1024;

    private readonly HttpClient httpClient;

    public MediaApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<MediaSearchResponse> SearchAsync(
        MediaSearchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildSearchEndpoint(request), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        var media = await response.Content.ReadFromJsonAsync<MediaSearchResponse>(
            cancellationToken: cancellationToken);

        return media ?? new MediaSearchResponse([], request.PageNumber, request.PageSize, 0, 0, false, false);
    }

    public async Task<MediaDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/media/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<MediaDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<MediaDto> CreateAsync(
        CreateMediaRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/media", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<MediaDto>(response, cancellationToken);
    }

    public async Task<MediaDto> UploadAsync(
        IBrowserFile file,
        string? altText,
        string? description,
        CancellationToken cancellationToken)
    {
        using var form = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream(MaximumUploadBytes, cancellationToken);
        using var streamContent = new StreamContent(stream);
        if (!string.IsNullOrWhiteSpace(file.ContentType))
        {
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        }

        form.Add(streamContent, "file", file.Name);
        if (!string.IsNullOrWhiteSpace(altText))
        {
            form.Add(new StringContent(altText), "altText");
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            form.Add(new StringContent(description), "description");
        }

        var response = await httpClient.PostAsync("api/v1/media/upload", form, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<MediaDto>(response, cancellationToken);
    }

    public async Task<MediaDto> UpdateAsync(
        Guid id,
        UpdateMediaRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/media/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<MediaDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/media/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    private static string BuildSearchEndpoint(MediaSearchRequest request)
    {
        var parameters = new List<string>();
        Add(parameters, "search", request.Search);
        Add(parameters, "mediaType", request.MediaType);
        Add(parameters, "contentType", request.ContentType);

        if (request.IsActive.HasValue)
        {
            Add(parameters, "isActive", request.IsActive.Value ? "true" : "false");
        }

        Add(parameters, "pageNumber", request.PageNumber.ToString());
        Add(parameters, "pageSize", request.PageSize.ToString());

        return parameters.Count == 0
            ? "api/v1/media"
            : $"api/v1/media?{string.Join("&", parameters)}";
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
