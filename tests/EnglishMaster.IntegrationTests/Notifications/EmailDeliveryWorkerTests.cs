using System.Net.Http.Json;
using EnglishMaster.Contracts.EmailMessages;
using EnglishMaster.Contracts.Security;

namespace EnglishMaster.IntegrationTests.Notifications;

public sealed class EmailDeliveryWorkerTests(EmailDeliveryWorkerTestFactory factory)
    : IClassFixture<EmailDeliveryWorkerTestFactory>
{
    [Fact]
    public async Task PendingEmail_IsSentAutomaticallyByTheBackgroundWorker()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "superadmin@englishmaster.workertest",
            "TestPassword1"));
        var toEmail = $"worker-{Guid.NewGuid():N}@example.test";

        var queueResponse = await client.PostAsJsonAsync("/api/v1/admin/email-messages", new QueueEmailMessageRequest(
            toEmail,
            "Worker Learner",
            "Worker message",
            "This should be sent by the background worker, not a manual call",
            IsHtml: false));
        queueResponse.EnsureSuccessStatusCode();

        // No call to POST /api/v1/admin/email-delivery/process here - the worker must do this on its own.
        var sent = await PollUntilSentAsync(client, toEmail, TimeSpan.FromSeconds(5));

        Assert.True(sent, "Expected the background worker to automatically send the queued email within the timeout.");
    }

    private static async Task<bool> PollUntilSentAsync(HttpClient client, string toEmail, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        while (!cts.IsCancellationRequested)
        {
            var search = await client.GetFromJsonAsync<EmailMessageSearchResponse>(
                $"/api/v1/admin/email-messages?toEmail={Uri.EscapeDataString(toEmail)}",
                cts.Token);

            if (search?.Items.Any(email => email.Status == "Sent") == true)
            {
                return true;
            }

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200), cts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        return false;
    }
}
