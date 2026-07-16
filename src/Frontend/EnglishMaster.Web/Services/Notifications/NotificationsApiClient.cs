using System.Net.Http.Json;
using EnglishMaster.Contracts.Notifications;

namespace EnglishMaster.Web.Services.Notifications;

public sealed class NotificationsApiClient : INotificationsApiClient
{
    private readonly HttpClient httpClient;

    public NotificationsApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<NotificationSearchResponse> GetMyNotificationsAsync(string? status, string? eventType, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildEndpoint("api/v1/me/notifications", status, eventType), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ReadSearchAsync(response, cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/me/notifications/unread-count", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        var count = await response.Content.ReadFromJsonAsync<UnreadNotificationCountResponse>(cancellationToken: cancellationToken);
        return count?.Count ?? 0;
    }

    public async Task<NotificationDto> MarkReadAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/me/notifications/{id}/read", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<NotificationDto>(response, cancellationToken);
    }

    public async Task<NotificationSearchResponse> SearchAdminAsync(string? status, string? eventType, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildEndpoint("api/v1/admin/notifications", status, eventType), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ReadSearchAsync(response, cancellationToken);
    }

    private static async Task<NotificationSearchResponse> ReadSearchAsync(HttpResponseMessage response, CancellationToken cancellationToken) =>
        await response.Content.ReadFromJsonAsync<NotificationSearchResponse>(cancellationToken: cancellationToken) ??
        new NotificationSearchResponse([], 1, 20, 0, 0, false, false);

    private static string BuildEndpoint(string path, string? status, string? eventType)
    {
        var parameters = new List<string>();
        if (!string.IsNullOrWhiteSpace(status))
        {
            parameters.Add($"status={Uri.EscapeDataString(status)}");
        }

        if (!string.IsNullOrWhiteSpace(eventType))
        {
            parameters.Add($"eventType={Uri.EscapeDataString(eventType)}");
        }

        return parameters.Count == 0 ? path : $"{path}?{string.Join("&", parameters)}";
    }
}
