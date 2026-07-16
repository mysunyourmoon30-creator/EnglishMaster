using System.Net.Http.Json;
using EnglishMaster.Contracts.EmailMessages;

namespace EnglishMaster.Web.Services.EmailMessages;

public sealed class EmailMessagesApiClient : IEmailMessagesApiClient
{
    private readonly HttpClient httpClient;

    public EmailMessagesApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<EmailMessageSearchResponse> SearchAsync(string? status, string? toEmail, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildEndpoint(status, toEmail), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<EmailMessageSearchResponse>(cancellationToken: cancellationToken) ??
            new EmailMessageSearchResponse([], 1, 20, 0, 0, false, false);
    }

    public async Task<EmailMessageDto> QueueAsync(QueueEmailMessageRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/admin/email-messages", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<EmailMessageDto>(response, cancellationToken);
    }

    private static string BuildEndpoint(string? status, string? toEmail)
    {
        var parameters = new List<string>();
        if (!string.IsNullOrWhiteSpace(status))
        {
            parameters.Add($"status={Uri.EscapeDataString(status)}");
        }

        if (!string.IsNullOrWhiteSpace(toEmail))
        {
            parameters.Add($"toEmail={Uri.EscapeDataString(toEmail)}");
        }

        return parameters.Count == 0
            ? "api/v1/admin/email-messages"
            : $"api/v1/admin/email-messages?{string.Join("&", parameters)}";
    }
}
