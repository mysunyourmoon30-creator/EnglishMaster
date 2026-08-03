using System.Net;
using System.Net.Http.Json;

using EnglishMaster.Contracts.Security;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.Security;

public sealed class LoopbackCookiePolicyTests
{
    [Fact]
    public async Task StagingLoopbackOverride_AllowsCookieOnHttpLoopback()
    {
        using var factory = CreateFactory();
        using var client = CreateClient(factory, "http://127.0.0.1:7101");

        var response = await LoginAsync(client);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cookies = response.Headers.GetValues("Set-Cookie").ToArray();
        Assert.NotEmpty(cookies);
        Assert.DoesNotContain(
            cookies,
            cookie => cookie.Contains("; secure", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task StagingLoopbackOverride_KeepsCookieSecureForNonLoopbackHost()
    {
        using var factory = CreateFactory();
        using var client = CreateClient(factory, "http://staging.example.test");

        var response = await LoginAsync(client);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cookies = response.Headers.GetValues("Set-Cookie").ToArray();
        Assert.NotEmpty(cookies);
        Assert.All(
            cookies,
            cookie => Assert.Contains("; secure", cookie, StringComparison.OrdinalIgnoreCase));
    }

    private static WebApplicationFactory<Program> CreateFactory()
    {
        var databaseName = $"EnglishMaster-LoopbackCookie-{Guid.NewGuid():N}";
        var dataProtectionPath = Path.Combine(Path.GetTempPath(), databaseName);

        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Staging");
                builder.UseSetting("Auth:AllowInsecureLoopbackCookies", "true");
                builder.ConfigureAppConfiguration((_, configuration) =>
                {
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Auth:AllowInsecureLoopbackCookies"] = "true",
                        ["Auth:InitialSuperAdmin:Email"] = "loopback.admin@englishmaster.test",
                        ["Auth:InitialSuperAdmin:Password"] = "TestPassword1",
                        ["ConnectionStrings:DefaultConnection"] = "InMemory",
                        ["Database:ApplyMigrationsOnStartup"] = "false",
                        ["Database:Name"] = databaseName,
                        ["Database:Provider"] = "InMemory",
                        ["DataProtection:KeysPath"] = dataProtectionPath,
                        ["EmailDeliveryWorker:Enabled"] = "false",
                        ["Logging:FilePath"] = dataProtectionPath,
                        ["SystemHealthWorker:Enabled"] = "false"
                    });
                });
                builder.ConfigureServices(services =>
                {
                    services.AddDataProtection()
                        .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
                        .SetApplicationName(databaseName);
                });
            });
    }

    private static HttpClient CreateClient(
        WebApplicationFactory<Program> factory,
        string baseAddress) =>
        factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(baseAddress),
            HandleCookies = false
        });

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "loopback.admin@englishmaster.test",
            "TestPassword1"));
}
