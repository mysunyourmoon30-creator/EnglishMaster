using System.Reflection;

namespace EnglishMaster.UnitTests;

public sealed class FoundationSmokeTests
{
    [Theory]
    [InlineData("EnglishMaster.Domain")]
    [InlineData("EnglishMaster.Application")]
    [InlineData("EnglishMaster.Shared")]
    public void CoreAssembliesCanLoad(string assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);

        Assert.Equal(assemblyName, assembly.GetName().Name);
    }
}
