using System.Text.Json;
using EnglishMaster.Application.Features.ContentRevisions;

namespace EnglishMaster.Infrastructure.ContentRevisions;

public sealed class ContentSnapshotSerializer : IContentSnapshotSerializer
{
    private static readonly string[] SensitiveNameFragments =
    {
        "password",
        "token",
        "secret",
        "apikey",
        "connectionstring",
        "cookie",
        "securitystamp"
    };

    public string SanitizeSnapshot(string snapshotJson)
    {
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(snapshotJson) ? "{}" : snapshotJson);
        var sanitized = SanitizeElement(document.RootElement);
        return JsonSerializer.Serialize(sanitized);
    }

    private static object? SanitizeElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject()
                .Where(property => !IsSensitiveName(property.Name))
                .ToDictionary(property => property.Name, property => SanitizeElement(property.Value)),
            JsonValueKind.Array => element.EnumerateArray().Select(SanitizeElement).ToArray(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var number) ? number : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => null
        };
    }

    private static bool IsSensitiveName(string name)
    {
        var normalized = name.Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Trim();
        return SensitiveNameFragments.Any(fragment => normalized.Contains(fragment, StringComparison.OrdinalIgnoreCase));
    }
}
