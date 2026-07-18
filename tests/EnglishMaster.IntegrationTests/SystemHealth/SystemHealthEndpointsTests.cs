using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Contracts.SystemHealth;

namespace EnglishMaster.IntegrationTests.SystemHealth;

public sealed class SystemHealthEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task GetSystemHealth_ReturnsHealthyStatus_ForSuperAdmin()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync("/api/v1/admin/system-health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<SystemHealthResponse>();
        Assert.NotNull(body);
        Assert.True(body!.DatabaseHealthy);
        Assert.True(body.FailedEmailCount >= 0);
        Assert.True(body.FailedPublishJobCount >= 0);
        Assert.True(body.FailedImportJobCount >= 0);
    }

    [Fact]
    public async Task GetSystemHealth_RequiresSystemHealthReadPermission()
    {
        using var adminClient = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(adminClient);
        await CreateUserWithRoleAsync(adminClient, "health-viewer@englishmaster.test", "ContentEditor");

        using var editorClient = factory.CreateClient(new() { HandleCookies = true });
        await editorClient.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("health-viewer@englishmaster.test", "EditorPass1"));

        var response = await editorClient.GetAsync("/api/v1/admin/system-health");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static async Task CreateUserWithRoleAsync(HttpClient client, string email, string roleName)
    {
        var roles = await client.GetFromJsonAsync<RoleSearchResponse>("/api/v1/roles?pageSize=100");
        var role = roles!.Items.Single(item => item.Name == roleName);
        var createUserResponse = await client.PostAsJsonAsync(
            "/api/v1/users",
            new CreateUserRequest(email, email.Split('@')[0], "Health Viewer", "EditorPass1", []));
        createUserResponse.EnsureSuccessStatusCode();
        var user = await createUserResponse.Content.ReadFromJsonAsync<UserDto>();
        var assignResponse = await client.PostAsync($"/api/v1/users/{user!.Id}/roles/{role.Id}", null);
        assignResponse.EnsureSuccessStatusCode();
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "superadmin@englishmaster.test",
            "TestPassword1"));
}
