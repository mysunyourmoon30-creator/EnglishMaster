using System.Reflection;

namespace EnglishMaster.ArchitectureTests;

public sealed class ProjectReferenceRulesTests
{
    [Theory]
    [InlineData("EnglishMaster.Domain")]
    [InlineData("EnglishMaster.Contracts")]
    [InlineData("EnglishMaster.Shared")]
    public void FoundationProjectsDoNotReferenceOtherEnglishMasterProjects(string assemblyName)
    {
        AssertOnlyReferences(assemblyName);
    }

    [Fact]
    public void ApplicationReferencesOnlyAllowedProjects()
    {
        AssertOnlyReferences(
            "EnglishMaster.Application",
            "EnglishMaster.Domain",
            "EnglishMaster.Contracts",
            "EnglishMaster.Shared");
    }

    [Fact]
    public void InfrastructureReferencesOnlyAllowedProjects()
    {
        AssertOnlyReferences(
            "EnglishMaster.Infrastructure",
            "EnglishMaster.Application",
            "EnglishMaster.Domain",
            "EnglishMaster.Shared");
    }

    [Fact]
    public void ApiReferencesOnlyAllowedProjects()
    {
        AssertOnlyReferences(
            "EnglishMaster.Api",
            "EnglishMaster.Application",
            "EnglishMaster.Infrastructure",
            "EnglishMaster.Contracts",
            "EnglishMaster.Shared");
    }

    [Fact]
    public void WebReferencesOnlyAllowedProjects()
    {
        AssertOnlyReferences(
            "EnglishMaster.Web",
            "EnglishMaster.Contracts",
            "EnglishMaster.Shared");
    }

    private static void AssertOnlyReferences(string assemblyName, params string[] allowedReferences)
    {
        var allowed = allowedReferences.ToHashSet(StringComparer.Ordinal);
        var references = Assembly
            .Load(assemblyName)
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null && name.StartsWith("EnglishMaster.", StringComparison.Ordinal))
            .Cast<string>();

        var unexpectedReferences = references
            .Where(reference => !allowed.Contains(reference))
            .OrderBy(reference => reference)
            .ToArray();

        Assert.Empty(unexpectedReferences);
    }
}
