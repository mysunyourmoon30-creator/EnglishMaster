using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Certificates;
using EnglishMaster.Contracts.Security;

namespace EnglishMaster.IntegrationTests.Certificates;

public sealed class CertificateTemplateEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task CertificatePermissions_AreListed()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var permissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>("/api/v1/permissions");

        Assert.NotNull(permissions);
        Assert.Contains(permissions, permission => permission.Key == "certificates.read");
        Assert.Contains(permissions, permission => permission.Key == "certificates.manage");
    }

    [Fact]
    public async Task CertificateTemplates_CanCreateSearchAndDeactivate()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var code = $"course-complete-{Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("/api/v1/admin/certificate-templates", new CreateCertificateTemplateRequest(
            code,
            "Course Completion",
            "Issued when a learner completes a course.",
            "Certificate for {{student}} completing {{course}}."));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<CertificateTemplateDto>();

        var search = await client.GetFromJsonAsync<CertificateTemplateSearchResponse>($"/api/v1/admin/certificate-templates?search={Uri.EscapeDataString(code)}");
        var deactivateResponse = await client.PostAsync($"/api/v1/admin/certificate-templates/{created!.Id}/deactivate", null);
        deactivateResponse.EnsureSuccessStatusCode();
        var deactivated = await deactivateResponse.Content.ReadFromJsonAsync<CertificateTemplateDto>();

        Assert.NotNull(created);
        Assert.Equal(code, created!.Code);
        Assert.True(created.IsActive);
        Assert.Contains(search!.Items, template => template.Id == created.Id);
        Assert.False(deactivated!.IsActive);
    }

    [Fact]
    public async Task CertificateTemplates_RejectDuplicateCode()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var code = $"duplicate-{Guid.NewGuid():N}";
        var request = new CreateCertificateTemplateRequest(code, "Duplicate", string.Empty, "Body");
        var first = await client.PostAsJsonAsync("/api/v1/admin/certificate-templates", request);
        first.EnsureSuccessStatusCode();

        var second = await client.PostAsJsonAsync("/api/v1/admin/certificate-templates", request);

        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));
}
