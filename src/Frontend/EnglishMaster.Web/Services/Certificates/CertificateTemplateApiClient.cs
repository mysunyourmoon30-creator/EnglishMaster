using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Certificates;
using EnglishMaster.Web.Services;

namespace EnglishMaster.Web.Services.Certificates;

public sealed class CertificateTemplateApiClient : ICertificateTemplateApiClient
{
    private readonly HttpClient httpClient;

    public CertificateTemplateApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<CertificateTemplateSearchResponse> SearchAsync(
        string? search,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        var endpoint = BuildSearchEndpoint(search, isActive);

        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        var templates = await response.Content.ReadFromJsonAsync<CertificateTemplateSearchResponse>(
            cancellationToken: cancellationToken);

        return templates ?? new CertificateTemplateSearchResponse([], 1, 10, 0, 0, false, false);
    }

    private static string BuildSearchEndpoint(string? search, bool? isActive)
    {
        var parameters = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
        {
            parameters.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        if (isActive.HasValue)
        {
            parameters.Add($"isActive={(isActive.Value ? "true" : "false")}");
        }

        return parameters.Count == 0
            ? "api/v1/admin/certificate-templates"
            : $"api/v1/admin/certificate-templates?{string.Join("&", parameters)}";
    }

    public async Task<CertificateTemplateDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/admin/certificate-templates/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<CertificateTemplateDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<CertificateTemplateDto> CreateAsync(
        CreateCertificateTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/admin/certificate-templates", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<CertificateTemplateDto>(response, cancellationToken);
    }

    public async Task<CertificateTemplateDto> UpdateAsync(
        Guid id,
        UpdateCertificateTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/admin/certificate-templates/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<CertificateTemplateDto>(response, cancellationToken);
    }

    public async Task ActivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/admin/certificate-templates/{id}/activate", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/admin/certificate-templates/{id}/deactivate", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }
}
