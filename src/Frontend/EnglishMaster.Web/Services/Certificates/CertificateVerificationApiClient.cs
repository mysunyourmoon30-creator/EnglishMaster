using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Certificates;

namespace EnglishMaster.Web.Services.Certificates;

public sealed class CertificateVerificationApiClient : ICertificateVerificationApiClient
{
    private readonly HttpClient httpClient;

    public CertificateVerificationApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<PublicCertificateVerificationDto?> VerifyAsync(string verificationCode, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/public/certificates/{Uri.EscapeDataString(verificationCode)}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PublicCertificateVerificationDto>(cancellationToken: cancellationToken);
    }
}
