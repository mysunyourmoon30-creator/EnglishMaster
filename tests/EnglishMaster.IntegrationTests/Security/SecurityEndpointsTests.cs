using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Security;

namespace EnglishMaster.IntegrationTests.Security;

public sealed class SecurityEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsCurrentUser()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });

        var response = await LoginAsync(client);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(body);
        Assert.Contains("SuperAdmin", body!.User.Roles);
        Assert.Contains("permissions.read", body.User.Permissions);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsValidationProblem()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "superadmin@englishmaster.test",
            "wrong-password"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UserManagement_RequiresAuthentication()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RoleAndPermissionManagement_WorksForSuperAdmin()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var roleResponse = await client.PostAsJsonAsync("/api/v1/roles", new CreateRoleRequest(
            "Test Manager",
            "Can manage test content"));
        roleResponse.EnsureSuccessStatusCode();
        var role = await roleResponse.Content.ReadFromJsonAsync<RoleDto>();
        Assert.NotNull(role);

        var assignPermissionResponse = await client.PostAsJsonAsync(
            $"/api/v1/roles/{role!.Id}/permissions",
            new AssignPermissionRequest("words.read"));
        assignPermissionResponse.EnsureSuccessStatusCode();

        var permissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>($"/api/v1/roles/{role.Id}/permissions");
        Assert.Contains(permissions!, permission => permission.Key == "words.read");
    }

    [Fact]
    public async Task UserRoleAssignment_WorksForSuperAdmin()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var roleResponse = await client.GetFromJsonAsync<RoleSearchResponse>("/api/v1/roles?pageSize=100");
        var viewerRole = roleResponse!.Items.First(role => role.Name == "Viewer");

        var createUserResponse = await client.PostAsJsonAsync("/api/v1/users", new CreateUserRequest(
            "viewer@englishmaster.test",
            "viewer",
            "Viewer User",
            "ViewerPass1",
            []));
        createUserResponse.EnsureSuccessStatusCode();
        var user = await createUserResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);

        var assignRoleResponse = await client.PostAsync($"/api/v1/users/{user!.Id}/roles/{viewerRole.Id}", null);
        assignRoleResponse.EnsureSuccessStatusCode();

        var updatedUser = await client.GetFromJsonAsync<UserDto>($"/api/v1/users/{user.Id}");
        Assert.Contains(updatedUser!.Roles, role => role.Name == "Viewer");
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "superadmin@englishmaster.test",
            "TestPassword1"));
}
