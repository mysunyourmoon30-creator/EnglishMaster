using System.Reflection;

namespace EnglishMaster.IntegrationTests;

public sealed class FoundationSmokeTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public FoundationSmokeTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Theory]
    [InlineData("EnglishMaster.Api")]
    [InlineData("EnglishMaster.Application")]
    [InlineData("EnglishMaster.Infrastructure")]
    [InlineData("EnglishMaster.Contracts")]
    [InlineData("EnglishMaster.Shared")]
    public void BackendAssembliesCanLoad(string assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);

        Assert.Equal(assemblyName, assembly.GetName().Name);
    }

    [Theory]
    [InlineData("/health")]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task ApiHealthEndpointsReturnSuccess(string path)
    {
        var response = await client.GetAsync(path);

        response.EnsureSuccessStatusCode();
    }
}
