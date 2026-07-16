using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Media.Dtos;
using EnglishMaster.Domain.Media;
using EnglishMaster.Shared.Results;
using Microsoft.Extensions.Configuration;

namespace EnglishMaster.Infrastructure.Media;

internal sealed class LocalMediaStorageService : IMediaStorageService
{
    private static readonly Dictionary<string, FileTypeDefinition> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/png"] = new(".png", MediaType.Image, IsPng),
        ["image/jpeg"] = new(".jpg", MediaType.Image, IsJpeg),
        ["image/gif"] = new(".gif", MediaType.Image, IsGif),
        ["image/webp"] = new(".webp", MediaType.Image, IsWebp),
        ["audio/mpeg"] = new(".mp3", MediaType.Audio, IsMp3),
        ["audio/wav"] = new(".wav", MediaType.Audio, IsWav),
        ["video/mp4"] = new(".mp4", MediaType.Video, IsMp4),
        ["application/pdf"] = new(".pdf", MediaType.Document, IsPdf),
        ["text/plain"] = new(".txt", MediaType.Document, IsPlainText)
    };

    private readonly string storageRoot;

    public LocalMediaStorageService(IConfiguration configuration)
    {
        storageRoot = configuration["Media:LocalStoragePath"] is { Length: > 0 } configuredPath
            ? configuredPath
            : Path.Combine(AppContext.BaseDirectory, "media");
    }

    public async Task<Result<StoredMediaFile>> SaveAsync(
        Stream content,
        string originalFileName,
        string contentType,
        long fileSize,
        CancellationToken cancellationToken)
    {
        var trimmedOriginalFileName = originalFileName?.Trim() ?? string.Empty;
        var normalizedOriginalFileName = Path.GetFileName(trimmedOriginalFileName);
        var normalizedContentType = NormalizeContentType(contentType);

        if (string.IsNullOrWhiteSpace(normalizedOriginalFileName))
        {
            return Result<StoredMediaFile>.Validation(
                new ValidationError(nameof(originalFileName), "OriginalFileName is required."));
        }

        if (trimmedOriginalFileName.Contains('/') ||
            trimmedOriginalFileName.Contains('\\') ||
            normalizedOriginalFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            return Result<StoredMediaFile>.Validation(
                new ValidationError(nameof(originalFileName), "OriginalFileName must be a file name, not a path."));
        }

        if (fileSize <= 0)
        {
            return Result<StoredMediaFile>.Validation(
                new ValidationError(nameof(fileSize), "FileSize must be greater than zero."));
        }

        if (fileSize > MediaUploadLimits.MaximumFileSizeBytes)
        {
            return Result<StoredMediaFile>.Validation(
                new ValidationError(nameof(fileSize), $"FileSize must be {MediaUploadLimits.MaximumFileSizeBytes} bytes or fewer."));
        }

        if (!AllowedTypes.TryGetValue(normalizedContentType, out var definition))
        {
            return Result<StoredMediaFile>.Validation(
                new ValidationError(nameof(contentType), "ContentType is not allowed."));
        }

        var header = new byte[512];
        var bytesRead = await content.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);
        if (!definition.SignatureMatches(header.AsSpan(0, bytesRead)))
        {
            return Result<StoredMediaFile>.Validation(
                new ValidationError(nameof(contentType), "File content does not match ContentType."));
        }

        Directory.CreateDirectory(storageRoot);

        var fileName = $"{Guid.NewGuid():N}{definition.Extension}";
        var physicalPath = Path.Combine(storageRoot, fileName);

        var totalBytesWritten = bytesRead;
        var shouldDeletePartialFile = false;
        await using (var output = new FileStream(
            physicalPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true))
        {
            if (bytesRead > 0)
            {
                await output.WriteAsync(header.AsMemory(0, bytesRead), cancellationToken);
            }

            var buffer = new byte[81920];
            int read;
            while ((read = await content.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
            {
                totalBytesWritten += read;
                if (totalBytesWritten > MediaUploadLimits.MaximumFileSizeBytes)
                {
                    shouldDeletePartialFile = true;
                    break;
                }

                await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }
        }

        if (shouldDeletePartialFile)
        {
            File.Delete(physicalPath);
            return Result<StoredMediaFile>.Validation(
                new ValidationError(nameof(fileSize), $"FileSize must be {MediaUploadLimits.MaximumFileSizeBytes} bytes or fewer."));
        }

        if (totalBytesWritten != fileSize)
        {
            File.Delete(physicalPath);
            return Result<StoredMediaFile>.Validation(
                new ValidationError(nameof(fileSize), "FileSize does not match the uploaded content."));
        }

        return Result<StoredMediaFile>.Success(new StoredMediaFile(
            fileName,
            normalizedOriginalFileName,
            definition.Extension,
            normalizedContentType,
            fileSize,
            definition.MediaType,
            $"media/{fileName}",
            $"/media/{fileName}"));
    }

    private static string NormalizeContentType(string? contentType)
    {
        var normalized = contentType?.Trim().ToLowerInvariant() ?? string.Empty;
        var separatorIndex = normalized.IndexOf(';', StringComparison.Ordinal);
        return separatorIndex < 0
            ? normalized
            : normalized[..separatorIndex].Trim();
    }

    private static bool IsPng(ReadOnlySpan<byte> bytes)
    {
        return bytes.Length >= 8 &&
            bytes[0] == 0x89 &&
            bytes[1] == 0x50 &&
            bytes[2] == 0x4E &&
            bytes[3] == 0x47 &&
            bytes[4] == 0x0D &&
            bytes[5] == 0x0A &&
            bytes[6] == 0x1A &&
            bytes[7] == 0x0A;
    }

    private static bool IsJpeg(ReadOnlySpan<byte> bytes)
    {
        return bytes.Length >= 3 &&
            bytes[0] == 0xFF &&
            bytes[1] == 0xD8 &&
            bytes[2] == 0xFF;
    }

    private static bool IsGif(ReadOnlySpan<byte> bytes)
    {
        return bytes.Length >= 6 &&
            bytes[0] == 'G' &&
            bytes[1] == 'I' &&
            bytes[2] == 'F' &&
            bytes[3] == '8';
    }

    private static bool IsWebp(ReadOnlySpan<byte> bytes)
    {
        return bytes.Length >= 12 &&
            bytes[0] == 'R' &&
            bytes[1] == 'I' &&
            bytes[2] == 'F' &&
            bytes[3] == 'F' &&
            bytes[8] == 'W' &&
            bytes[9] == 'E' &&
            bytes[10] == 'B' &&
            bytes[11] == 'P';
    }

    private static bool IsMp3(ReadOnlySpan<byte> bytes)
    {
        return bytes.Length >= 3 &&
            ((bytes[0] == 'I' && bytes[1] == 'D' && bytes[2] == '3') ||
            (bytes[0] == 0xFF && (bytes[1] & 0xE0) == 0xE0));
    }

    private static bool IsWav(ReadOnlySpan<byte> bytes)
    {
        return bytes.Length >= 12 &&
            bytes[0] == 'R' &&
            bytes[1] == 'I' &&
            bytes[2] == 'F' &&
            bytes[3] == 'F' &&
            bytes[8] == 'W' &&
            bytes[9] == 'A' &&
            bytes[10] == 'V' &&
            bytes[11] == 'E';
    }

    private static bool IsMp4(ReadOnlySpan<byte> bytes)
    {
        return bytes.Length >= 12 &&
            bytes[4] == 'f' &&
            bytes[5] == 't' &&
            bytes[6] == 'y' &&
            bytes[7] == 'p';
    }

    private static bool IsPdf(ReadOnlySpan<byte> bytes)
    {
        return bytes.Length >= 4 &&
            bytes[0] == '%' &&
            bytes[1] == 'P' &&
            bytes[2] == 'D' &&
            bytes[3] == 'F';
    }

    private static bool IsPlainText(ReadOnlySpan<byte> bytes)
    {
        return bytes.Length > 0 && !bytes.Contains((byte)0);
    }

    private delegate bool ContentSignatureMatcher(ReadOnlySpan<byte> bytes);

    private sealed record FileTypeDefinition(
        string Extension,
        MediaType MediaType,
        ContentSignatureMatcher SignatureMatches);
}
