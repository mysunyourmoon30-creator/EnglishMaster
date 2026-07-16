using System.Net.Http.Headers;
using System.Net.Http.Json;
using EnglishMaster.Contracts.ImportExport;

namespace EnglishMaster.Web.Services.ImportExport;

public sealed class ImportExportApiClient(HttpClient httpClient) : IImportExportApiClient
{
    public async Task<ContentImportResult> ImportWordsAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);
        content.Add(fileContent, "file", fileName);

        var response = await httpClient.PostAsync("api/v1/import/words", content, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<ContentImportResult>(response, cancellationToken);
    }

    public async Task<ExportFileResult> ExportAsync(
        string entity,
        string format,
        CancellationToken cancellationToken)
    {
        var safeEntity = Uri.EscapeDataString(entity);
        var safeFormat = Uri.EscapeDataString(format);
        var response = await httpClient.GetAsync($"api/v1/export/{safeEntity}?format={safeFormat}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar ??
            response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ??
            $"{entity}.{format}";
        var contentType = response.Content.Headers.ContentType?.MediaType ??
            (format.Equals("json", StringComparison.OrdinalIgnoreCase) ? "application/json" : "text/csv");

        return new ExportFileResult(fileName, contentType, content);
    }
}
