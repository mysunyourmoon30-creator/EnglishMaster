using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.EmailMessages;
using EnglishMaster.Contracts.Notifications;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.Notifications;

public sealed class NotificationEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task NotificationAndEmailPermissions_AreListed()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var permissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>("/api/v1/permissions");
        Assert.NotNull(permissions);

        Assert.Contains(permissions, permission => permission.Key == "notifications.read");
        Assert.Contains(permissions, permission => permission.Key == "notifications.manage");
        Assert.Contains(permissions, permission => permission.Key == "email.read");
        Assert.Contains(permissions, permission => permission.Key == "email.manage");
    }

    [Fact]
    public async Task MyNotifications_ReturnsSearchResponse()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync("/api/v1/me/notifications");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<NotificationSearchResponse>();
        Assert.NotNull(body);
        Assert.True(body!.TotalCount >= 0);
    }

    [Fact]
    public async Task UnreadCount_ReturnsCountResponse()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var body = await client.GetFromJsonAsync<UnreadNotificationCountResponse>("/api/v1/me/notifications/unread-count");

        Assert.NotNull(body);
        Assert.True(body!.Count >= 0);
    }

    [Fact]
    public async Task AdminNotifications_ReturnsSearchResponse()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync("/api/v1/admin/notifications");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<NotificationSearchResponse>();
        Assert.NotNull(body);
    }

    [Fact]
    public async Task EmailMessages_CanQueueAndSearch()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var queueResponse = await client.PostAsJsonAsync("/api/v1/admin/email-messages", new QueueEmailMessageRequest(
            "learner@example.test",
            "Learner",
            "Welcome",
            "Hello from EnglishMaster",
            IsHtml: false));
        queueResponse.EnsureSuccessStatusCode();
        var queued = await queueResponse.Content.ReadFromJsonAsync<EmailMessageDto>();

        var search = await client.GetFromJsonAsync<EmailMessageSearchResponse>("/api/v1/admin/email-messages?toEmail=learner@example.test");

        Assert.NotNull(queued);
        Assert.Contains(search!.Items, email => email.Id == queued!.Id);
    }

    [Fact]
    public async Task EmailProviderStatus_ReturnsSafeConfigurationStatus()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var status = await client.GetFromJsonAsync<EmailProviderStatusDto>("/api/v1/admin/email-provider/status");

        Assert.NotNull(status);
        Assert.Equal("Development", status!.Provider);
        Assert.True(status.IsConfigured);
        Assert.True(status.SupportsTestSend);
        Assert.DoesNotContain("password", status.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task EmailProviderTestSend_UsesConfiguredSender()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/admin/email-provider/test-send", new SendTestEmailRequest(
            "learner@example.test",
            "Learner",
            "Provider test",
            "Hello from the configured provider",
            IsHtml: false));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task EmailProviderTestSend_ValidatesRequiredFields()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/admin/email-provider/test-send", new SendTestEmailRequest(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            IsHtml: false));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EmailProviderTestSend_ValidatesEmailAddress()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/admin/email-provider/test-send", new SendTestEmailRequest(
            "not-an-email",
            string.Empty,
            "Subject",
            "Body",
            IsHtml: false));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EmailDeliveryProcess_SendsPendingMessages()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var toEmail = $"queue-{Guid.NewGuid():N}@example.test";

        var queueResponse = await client.PostAsJsonAsync("/api/v1/admin/email-messages", new QueueEmailMessageRequest(
            toEmail,
            "Queue Learner",
            "Queued message",
            "This should be processed",
            IsHtml: false));
        queueResponse.EnsureSuccessStatusCode();
        var queued = await queueResponse.Content.ReadFromJsonAsync<EmailMessageDto>();

        var processResponse = await client.PostAsJsonAsync("/api/v1/admin/email-delivery/process", new ProcessEmailQueueRequest(MaxItems: 5));
        processResponse.EnsureSuccessStatusCode();
        var result = await processResponse.Content.ReadFromJsonAsync<EmailDeliveryQueueProcessResponse>();

        var search = await client.GetFromJsonAsync<EmailMessageSearchResponse>($"/api/v1/admin/email-messages?toEmail={Uri.EscapeDataString(toEmail)}");

        Assert.NotNull(queued);
        Assert.NotNull(result);
        Assert.True(result!.Processed >= 1);
        Assert.True(result.Sent >= 1);
        Assert.Contains(search!.Items, email => email.Id == queued!.Id && email.Status == "Sent");
    }

    [Fact]
    public async Task EmailDeliveryRetry_SendsFailedMessage()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var toEmail = $"retry-{Guid.NewGuid():N}@example.test";

        var queueResponse = await client.PostAsJsonAsync("/api/v1/admin/email-messages", new QueueEmailMessageRequest(
            toEmail,
            "Retry Learner",
            "Retry message",
            "This should retry",
            IsHtml: false));
        queueResponse.EnsureSuccessStatusCode();
        var queued = await queueResponse.Content.ReadFromJsonAsync<EmailMessageDto>();
        await MarkEmailFailedAsync(queued!.Id);

        var retryResponse = await client.PostAsync($"/api/v1/admin/email-delivery/{queued.Id}/retry", null);
        retryResponse.EnsureSuccessStatusCode();
        var retried = await retryResponse.Content.ReadFromJsonAsync<EmailMessageDto>();

        Assert.NotNull(retried);
        Assert.Equal("Sent", retried!.Status);
        Assert.True(retried.SentAt.HasValue);
        Assert.Empty(retried.ErrorMessage);
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "superadmin@englishmaster.test",
            "TestPassword1"));

    private async Task MarkEmailFailedAsync(Guid id)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        var email = await dbContext.EmailMessages.FindAsync(id);
        Assert.NotNull(email);
        email!.MarkFailed("Simulated failure", DateTimeOffset.UtcNow);
        await dbContext.SaveChangesAsync();
    }
}
