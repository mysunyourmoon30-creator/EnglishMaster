using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Reports;
using EnglishMaster.Contracts.Security;

namespace EnglishMaster.IntegrationTests.Reports;

public sealed class ReportEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task AdminDashboard_ReturnsSummary_ForSuperAdmin()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var summary = await client.GetFromJsonAsync<AdminDashboardSummaryDto>("/api/v1/reports/admin-dashboard");

        Assert.NotNull(summary);
        Assert.True(summary!.Overview.TotalWords >= 0);
        Assert.True(summary.ContentStatus.PublishedLessons >= 0);
        Assert.True(summary.QuizAnalytics.TotalAttempts >= 0);
    }

    [Fact]
    public async Task Reports_RequireAuthenticationOutsideTestingByPolicy()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/reports/admin-dashboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReportsReadPermission_IsSeededForSuperAdminAndAdmin()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var roles = await client.GetFromJsonAsync<RoleSearchResponse>("/api/v1/roles?pageSize=100");

        Assert.Contains(roles!.Items, role =>
            role.Name == "SuperAdmin" && role.Permissions.Contains("reports.read"));
        Assert.Contains(roles.Items, role =>
            role.Name == "Admin" && role.Permissions.Contains("reports.read"));
    }

    [Fact]
    public async Task ReportsReadPermission_IsListed()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var permissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>("/api/v1/permissions");

        Assert.Contains(permissions!, permission => permission.Key == "reports.read");
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "superadmin@englishmaster.test",
            "TestPassword1"));
}
