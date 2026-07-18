using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Security;

namespace EnglishMaster.IntegrationTests.Persistence;

/// <summary>
/// Regression coverage for the class of defects (V030-DEF-001/002/003/007/008) that let 8
/// Blocker-severity bugs reach staging undetected: 3 EF Core FK cascade-path migration failures
/// (SQL Server error 1785) and 16 tables present in the EF model/snapshot but missing from
/// migration history. None of those were catchable by the InMemory-provider-based
/// <see cref="EnglishMasterApiFactory"/>, since EF Core's InMemory provider never runs real
/// migrations. This test exercises the real migration path against LocalDB and confirms a login
/// works immediately afterward, proving auth issuance survives a from-empty deployment.
/// </summary>
/// <remarks>
/// Scope: this covers the API-side migration + auth-issuance gap only. It does not cover the
/// V030-DEF-004/005/006 class of bugs (Blazor route collision, chunked-cookie forwarding), which
/// live in the EnglishMaster.Web BFF layer and would require a second, full-stack test wiring
/// both the API and Web hosts together.
/// </remarks>
public sealed class FreshDatabaseMigrationTests(RelationalEnglishMasterApiFactory factory)
    : IClassFixture<RelationalEnglishMasterApiFactory>
{
    [Fact]
    public async Task FreshLocalDbDeployment_MigratesAndAllowsLogin()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "superadmin@englishmaster.migrationtest",
            "TestPassword1"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(body);
        Assert.Contains("SuperAdmin", body!.User.Roles);
    }
}
