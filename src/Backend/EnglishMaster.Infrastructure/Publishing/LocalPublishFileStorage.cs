using System.Text;
using EnglishMaster.Application.Features.Publishing;
using Microsoft.Extensions.Configuration;

namespace EnglishMaster.Infrastructure.Publishing;

internal sealed class LocalPublishFileStorage : IPublishFileStorage
{
    private readonly string rootPath;

    public LocalPublishFileStorage(IConfiguration configuration)
    {
        rootPath = configuration["Publishing:LocalStoragePath"] is { Length: > 0 } configuredPath
            ? configuredPath
            : Path.Combine(AppContext.BaseDirectory, "publishing");
    }

    public async Task<StoredPublishFile> SaveAsync(
        string fileName,
        string content,
        string contentType,
        CancellationToken cancellationToken)
    {
        var safeFileName = SanitizeFileName(fileName);
        Directory.CreateDirectory(rootPath);

        var fullPath = Path.GetFullPath(Path.Combine(rootPath, safeFileName));
        var normalizedRoot = Path.GetFullPath(rootPath);
        var relativePath = Path.GetRelativePath(normalizedRoot, fullPath);
        if (Path.IsPathRooted(relativePath) ||
            relativePath.Equals("..", StringComparison.Ordinal) ||
            relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal) ||
            relativePath.StartsWith($"..{Path.AltDirectorySeparatorChar}", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Invalid publish output path.");
        }

        await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8, cancellationToken);
        var fileInfo = new FileInfo(fullPath);

        return new StoredPublishFile(
            safeFileName,
            $"publishing/{safeFileName}",
            $"/publishing/{Uri.EscapeDataString(safeFileName)}",
            fileInfo.Length,
            contentType);
    }

    private static string SanitizeFileName(string fileName)
    {
        var safeName = Path.GetFileName(fileName);
        foreach (var invalid in Path.GetInvalidFileNameChars())
        {
            safeName = safeName.Replace(invalid, '-');
        }

        return string.IsNullOrWhiteSpace(safeName)
            ? $"publish-{Guid.NewGuid():N}.txt"
            : safeName;
    }
}
