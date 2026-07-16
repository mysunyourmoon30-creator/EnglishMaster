using System.Net.Http.Json;
using System.Text.Json;

namespace EnglishMaster.Web.Services;

internal static class ApiClientResponseHandler
{
    public static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw await CreateExceptionAsync(response, cancellationToken);
    }

    public static async Task<T> ReadRequiredAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var value = await response.Content.ReadFromJsonAsync<T>(
            cancellationToken: cancellationToken);

        return value ?? throw new ApiRequestException(
            "The API returned an empty response.",
            response.StatusCode,
            new Dictionary<string, string[]>());
    }

    private static async Task<ApiRequestException> CreateExceptionAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var message = $"Request failed with status {(int)response.StatusCode}.";
        var validationErrors = new Dictionary<string, string[]>();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                using var document = JsonDocument.Parse(content);
                var root = document.RootElement;

                if (root.TryGetProperty("title", out var title) &&
                    title.ValueKind == JsonValueKind.String)
                {
                    message = title.GetString() ?? message;
                }

                if (root.TryGetProperty("errors", out var errors) &&
                    errors.ValueKind == JsonValueKind.Object)
                {
                    foreach (var error in errors.EnumerateObject())
                    {
                        validationErrors[error.Name] = error.Value.ValueKind == JsonValueKind.Array
                            ? error.Value.EnumerateArray()
                                .Select(item => item.GetString() ?? "Validation error.")
                                .ToArray()
                            : ["Validation error."];
                    }
                }
            }
            catch (JsonException)
            {
                message = content;
            }
        }

        return new ApiRequestException(message, response.StatusCode, validationErrors);
    }
}
