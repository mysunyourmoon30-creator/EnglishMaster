using System.Globalization;
using System.Text;
using System.Text.Json;
using EnglishMaster.Application.Features.ImportExport;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using EnglishMaster.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.ImportExport;

internal sealed class ContentImportExportService(
    EnglishMasterDbContext dbContext,
    TimeProvider timeProvider) : IContentImportService, IContentExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public async Task<Result<ContentImportResult>> ImportWordsAsync(
        Stream stream,
        string fileName,
        string contentType,
        long fileSize,
        CancellationToken cancellationToken)
    {
        var fileValidation = ValidateImportFile(fileName, contentType, fileSize);
        if (fileValidation.Count > 0)
        {
            return Result<ContentImportResult>.Validation(fileValidation.ToArray());
        }

        var normalizedFileName = Path.GetFileName(fileName);
        List<WordImportRow> rows;
        try
        {
            rows = IsJson(normalizedFileName, contentType)
                ? await ReadWordJsonAsync(stream, cancellationToken)
                : await ReadWordCsvAsync(stream, cancellationToken);
        }
        catch (JsonException)
        {
            return Result<ContentImportResult>.Validation(
                new ValidationError("file", "The JSON import file could not be parsed."));
        }

        if (rows.Count == 0)
        {
            return Result<ContentImportResult>.Validation(
                new ValidationError("file", "The import file does not contain any rows."));
        }

        var categoryItems = await dbContext.Categories
            .AsNoTracking()
            .Select(category => new { category.Slug, category.Id })
            .ToListAsync(cancellationToken);
        var categories = categoryItems.ToDictionary(
            category => category.Slug,
            category => category.Id,
            StringComparer.OrdinalIgnoreCase);
        var tagItems = await dbContext.Tags
            .AsNoTracking()
            .Select(tag => new { tag.Slug, tag.Id })
            .ToListAsync(cancellationToken);
        var tags = tagItems.ToDictionary(
            tag => tag.Slug,
            tag => tag.Id,
            StringComparer.OrdinalIgnoreCase);
        var existingWordSlugs = await dbContext.Words
            .AsNoTracking()
            .Select(word => word.Slug)
            .ToListAsync(cancellationToken);
        var seenSlugs = new HashSet<string>(existingWordSlugs, StringComparer.OrdinalIgnoreCase);
        var errors = new List<ContentImportError>();
        var words = new List<Word>();
        var now = timeProvider.GetUtcNow();

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rowErrors = ValidateWordImportRow(row, categories, tags, seenSlugs);
            if (rowErrors.Count > 0)
            {
                errors.AddRange(rowErrors);
                continue;
            }

            var text = row.Text!.Trim();
            var categoryId = ResolveCategoryId(row.CategorySlug, categories);
            var tagIds = ResolveTagIds(row, tags);

            var word = Word.Create(
                text,
                row.IpaUk?.Trim() ?? string.Empty,
                row.IpaUs?.Trim() ?? string.Empty,
                row.ThaiReading?.Trim() ?? string.Empty,
                row.MeaningTh!.Trim(),
                row.MeaningEn?.Trim() ?? string.Empty,
                Enum.Parse<PartOfSpeech>(row.PartOfSpeech!, ignoreCase: true),
                Enum.Parse<CefrLevel>(row.CefrLevel!, ignoreCase: true),
                row.ExampleEn?.Trim() ?? string.Empty,
                row.ExampleTh?.Trim() ?? string.Empty,
                categoryId,
                tagIds,
                imageMediaId: null,
                audioMediaId: null,
                now);

            seenSlugs.Add(word.Slug);
            words.Add(word);
        }

        if (words.Count > 0)
        {
            dbContext.Words.AddRange(words);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result<ContentImportResult>.Success(new ContentImportResult(
            rows.Count,
            words.Count,
            errors.Select(error => error.RowNumber).Distinct().Count(),
            errors));
    }

    public async Task<Result<ContentExportResult>> ExportWordsAsync(string? format, CancellationToken cancellationToken)
    {
        var rows = (await dbContext.Words
                .AsNoTracking()
                .OrderBy(word => word.Text)
                .ToListAsync(cancellationToken))
            .Select(word => new Dictionary<string, string?>
            {
                ["Id"] = word.Id.ToString(),
                ["Text"] = word.Text,
                ["Slug"] = word.Slug,
                ["IpaUk"] = word.IpaUk,
                ["IpaUs"] = word.IpaUs,
                ["ThaiReading"] = word.ThaiReading,
                ["MeaningTh"] = word.MeaningTh,
                ["MeaningEn"] = word.MeaningEn,
                ["PartOfSpeech"] = word.PartOfSpeech.ToString(),
                ["CefrLevel"] = word.CefrLevel.ToString(),
                ["ExampleEn"] = word.ExampleEn,
                ["ExampleTh"] = word.ExampleTh,
                ["CategoryId"] = word.CategoryId?.ToString(),
                ["ImageMediaId"] = word.ImageMediaId?.ToString(),
                ["AudioMediaId"] = word.AudioMediaId?.ToString(),
                ["IsActive"] = word.IsActive.ToString(CultureInfo.InvariantCulture),
                ["CreatedAt"] = word.CreatedAt.ToString("O", CultureInfo.InvariantCulture),
                ["UpdatedAt"] = word.UpdatedAt.ToString("O", CultureInfo.InvariantCulture)
            })
            .ToList();

        return Export("words", rows, format);
    }

    public async Task<Result<ContentExportResult>> ExportGrammarTopicsAsync(string? format, CancellationToken cancellationToken)
    {
        var rows = (await dbContext.GrammarTopics
                .AsNoTracking()
                .OrderBy(topic => topic.Title)
                .ToListAsync(cancellationToken))
            .Select(topic => new Dictionary<string, string?>
            {
                ["Id"] = topic.Id.ToString(),
                ["Title"] = topic.Title,
                ["Slug"] = topic.Slug,
                ["Summary"] = topic.Summary,
                ["CefrLevel"] = topic.CefrLevel.ToString(),
                ["SortOrder"] = topic.SortOrder.ToString(CultureInfo.InvariantCulture),
                ["IsActive"] = topic.IsActive.ToString(CultureInfo.InvariantCulture),
                ["CreatedAt"] = topic.CreatedAt.ToString("O", CultureInfo.InvariantCulture),
                ["UpdatedAt"] = topic.UpdatedAt.ToString("O", CultureInfo.InvariantCulture)
            })
            .ToList();

        return Export("grammar-topics", rows, format);
    }

    public async Task<Result<ContentExportResult>> ExportLessonsAsync(string? format, CancellationToken cancellationToken)
    {
        var rows = (await dbContext.Lessons
                .AsNoTracking()
                .OrderBy(lesson => lesson.Title)
                .ToListAsync(cancellationToken))
            .Select(lesson => new Dictionary<string, string?>
            {
                ["Id"] = lesson.Id.ToString(),
                ["Title"] = lesson.Title,
                ["Slug"] = lesson.Slug,
                ["Summary"] = lesson.Summary,
                ["Description"] = lesson.Description,
                ["CefrLevel"] = lesson.CefrLevel?.ToString(),
                ["CategoryId"] = lesson.CategoryId?.ToString(),
                ["ThumbnailMediaId"] = lesson.ThumbnailMediaId?.ToString(),
                ["EstimatedMinutes"] = lesson.EstimatedMinutes.ToString(CultureInfo.InvariantCulture),
                ["SortOrder"] = lesson.SortOrder.ToString(CultureInfo.InvariantCulture),
                ["IsPublished"] = lesson.IsPublished.ToString(CultureInfo.InvariantCulture),
                ["IsActive"] = lesson.IsActive.ToString(CultureInfo.InvariantCulture),
                ["CreatedAt"] = lesson.CreatedAt.ToString("O", CultureInfo.InvariantCulture),
                ["UpdatedAt"] = lesson.UpdatedAt.ToString("O", CultureInfo.InvariantCulture)
            })
            .ToList();

        return Export("lessons", rows, format);
    }

    public async Task<Result<ContentExportResult>> ExportCoursesAsync(string? format, CancellationToken cancellationToken)
    {
        var rows = (await dbContext.Courses
                .AsNoTracking()
                .OrderBy(course => course.Title)
                .ToListAsync(cancellationToken))
            .Select(course => new Dictionary<string, string?>
            {
                ["Id"] = course.Id.ToString(),
                ["Title"] = course.Title,
                ["Slug"] = course.Slug,
                ["Summary"] = course.Summary,
                ["Description"] = course.Description,
                ["CefrLevel"] = course.CefrLevel?.ToString(),
                ["CategoryId"] = course.CategoryId?.ToString(),
                ["ThumbnailMediaId"] = course.ThumbnailMediaId?.ToString(),
                ["EstimatedMinutes"] = course.EstimatedMinutes.ToString(CultureInfo.InvariantCulture),
                ["SortOrder"] = course.SortOrder.ToString(CultureInfo.InvariantCulture),
                ["IsPublished"] = course.IsPublished.ToString(CultureInfo.InvariantCulture),
                ["IsActive"] = course.IsActive.ToString(CultureInfo.InvariantCulture),
                ["CreatedAt"] = course.CreatedAt.ToString("O", CultureInfo.InvariantCulture),
                ["UpdatedAt"] = course.UpdatedAt.ToString("O", CultureInfo.InvariantCulture)
            })
            .ToList();

        return Export("courses", rows, format);
    }

    public async Task<Result<ContentExportResult>> ExportBooksAsync(string? format, CancellationToken cancellationToken)
    {
        var rows = (await dbContext.Books
                .AsNoTracking()
                .OrderBy(book => book.Title)
                .ToListAsync(cancellationToken))
            .Select(book => new Dictionary<string, string?>
            {
                ["Id"] = book.Id.ToString(),
                ["Title"] = book.Title,
                ["Slug"] = book.Slug,
                ["Subtitle"] = book.Subtitle,
                ["Summary"] = book.Summary,
                ["Description"] = book.Description,
                ["CefrLevel"] = book.CefrLevel?.ToString(),
                ["CategoryId"] = book.CategoryId?.ToString(),
                ["CoverMediaId"] = book.CoverMediaId?.ToString(),
                ["CourseId"] = book.CourseId?.ToString(),
                ["AuthorName"] = book.AuthorName,
                ["Edition"] = book.Edition,
                ["Version"] = book.Version,
                ["EstimatedPages"] = book.EstimatedPages.ToString(CultureInfo.InvariantCulture),
                ["SortOrder"] = book.SortOrder.ToString(CultureInfo.InvariantCulture),
                ["IsPublished"] = book.IsPublished.ToString(CultureInfo.InvariantCulture),
                ["IsActive"] = book.IsActive.ToString(CultureInfo.InvariantCulture),
                ["CreatedAt"] = book.CreatedAt.ToString("O", CultureInfo.InvariantCulture),
                ["UpdatedAt"] = book.UpdatedAt.ToString("O", CultureInfo.InvariantCulture)
            })
            .ToList();

        return Export("books", rows, format);
    }

    public async Task<Result<ContentExportResult>> ExportQuizzesAsync(string? format, CancellationToken cancellationToken)
    {
        var rows = (await dbContext.Quizzes
                .AsNoTracking()
                .OrderBy(quiz => quiz.Title)
                .ToListAsync(cancellationToken))
            .Select(quiz => new Dictionary<string, string?>
            {
                ["Id"] = quiz.Id.ToString(),
                ["Title"] = quiz.Title,
                ["Slug"] = quiz.Slug,
                ["Summary"] = quiz.Summary,
                ["Description"] = quiz.Description,
                ["CefrLevel"] = quiz.CefrLevel?.ToString(),
                ["CategoryId"] = quiz.CategoryId?.ToString(),
                ["LessonId"] = quiz.LessonId?.ToString(),
                ["CourseId"] = quiz.CourseId?.ToString(),
                ["BookId"] = quiz.BookId?.ToString(),
                ["TimeLimitMinutes"] = quiz.TimeLimitMinutes.ToString(CultureInfo.InvariantCulture),
                ["PassingScore"] = quiz.PassingScore.ToString(CultureInfo.InvariantCulture),
                ["SortOrder"] = quiz.SortOrder.ToString(CultureInfo.InvariantCulture),
                ["IsPublished"] = quiz.IsPublished.ToString(CultureInfo.InvariantCulture),
                ["IsActive"] = quiz.IsActive.ToString(CultureInfo.InvariantCulture),
                ["CreatedAt"] = quiz.CreatedAt.ToString("O", CultureInfo.InvariantCulture),
                ["UpdatedAt"] = quiz.UpdatedAt.ToString("O", CultureInfo.InvariantCulture)
            })
            .ToList();

        return Export("quizzes", rows, format);
    }

    private static List<ValidationError> ValidateImportFile(string fileName, string contentType, long fileSize)
    {
        var errors = new List<ValidationError>();
        var normalizedFileName = Path.GetFileName(fileName);
        if (string.IsNullOrWhiteSpace(normalizedFileName))
        {
            errors.Add(new ValidationError("file", "A file name is required."));
        }

        if (fileSize <= 0)
        {
            errors.Add(new ValidationError("file", "The import file is empty."));
        }

        if (fileSize > ContentImportExportLimits.MaximumImportFileSizeBytes)
        {
            errors.Add(new ValidationError("file", "The import file must be 1 MB or smaller."));
        }

        if (!IsCsv(normalizedFileName, contentType) && !IsJson(normalizedFileName, contentType))
        {
            errors.Add(new ValidationError("file", "Only CSV and JSON import files are supported."));
        }

        return errors;
    }

    private static async Task<List<WordImportRow>> ReadWordJsonAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        var rows = await JsonSerializer.DeserializeAsync<List<WordImportRow>>(
            stream,
            JsonOptions,
            cancellationToken);

        rows ??= [];
        for (var index = 0; index < rows.Count; index++)
        {
            rows[index].RowNumber = index + 1;
        }

        return rows;
    }

    private static async Task<List<WordImportRow>> ReadWordCsvAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var headerLine = await reader.ReadLineAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(headerLine))
        {
            return [];
        }

        var headers = ParseCsvLine(headerLine)
            .Select(header => header.Trim())
            .ToArray();
        var rows = new List<WordImportRow>();
        var rowNumber = 1;

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(cancellationToken);
            rowNumber++;
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var values = ParseCsvLine(line);
            var fields = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < headers.Length; index++)
            {
                fields[headers[index]] = index < values.Count ? values[index] : null;
            }

            rows.Add(new WordImportRow
            {
                RowNumber = rowNumber,
                Text = Get(fields, "Text"),
                IpaUk = Get(fields, "IpaUk"),
                IpaUs = Get(fields, "IpaUs"),
                ThaiReading = Get(fields, "ThaiReading"),
                MeaningTh = Get(fields, "MeaningTh"),
                MeaningEn = Get(fields, "MeaningEn"),
                PartOfSpeech = Get(fields, "PartOfSpeech"),
                CefrLevel = Get(fields, "CefrLevel"),
                ExampleEn = Get(fields, "ExampleEn"),
                ExampleTh = Get(fields, "ExampleTh"),
                CategorySlug = Get(fields, "CategorySlug"),
                Tags = Get(fields, "Tags")
            });
        }

        return rows;
    }

    private static List<ContentImportError> ValidateWordImportRow(
        WordImportRow row,
        IReadOnlyDictionary<string, Guid> categories,
        IReadOnlyDictionary<string, Guid> tags,
        ISet<string> seenWordSlugs)
    {
        var errors = new List<ContentImportError>();
        Required(row.RowNumber, nameof(row.Text), row.Text, errors);
        Required(row.RowNumber, nameof(row.MeaningTh), row.MeaningTh, errors);
        Required(row.RowNumber, nameof(row.PartOfSpeech), row.PartOfSpeech, errors);
        Required(row.RowNumber, nameof(row.CefrLevel), row.CefrLevel, errors);

        if (!string.IsNullOrWhiteSpace(row.Text))
        {
            var slug = Word.GenerateSlug(row.Text.Trim());
            if (seenWordSlugs.Contains(slug))
            {
                errors.Add(new ContentImportError(row.RowNumber, nameof(row.Text), "A word with this text already exists."));
            }
        }

        if (!string.IsNullOrWhiteSpace(row.PartOfSpeech) &&
            !Enum.TryParse<PartOfSpeech>(row.PartOfSpeech, ignoreCase: true, out _))
        {
            errors.Add(new ContentImportError(row.RowNumber, nameof(row.PartOfSpeech), "PartOfSpeech is not supported."));
        }

        if (!string.IsNullOrWhiteSpace(row.CefrLevel) &&
            !Enum.TryParse<CefrLevel>(row.CefrLevel, ignoreCase: true, out _))
        {
            errors.Add(new ContentImportError(row.RowNumber, nameof(row.CefrLevel), "CefrLevel is not supported."));
        }

        var categorySlug = NormalizeSlug(row.CategorySlug);
        if (!string.IsNullOrWhiteSpace(categorySlug) && !categories.ContainsKey(categorySlug))
        {
            errors.Add(new ContentImportError(row.RowNumber, nameof(row.CategorySlug), "CategorySlug does not match an existing category."));
        }

        foreach (var tagSlug in ReadTagSlugs(row))
        {
            if (!tags.ContainsKey(tagSlug))
            {
                errors.Add(new ContentImportError(row.RowNumber, nameof(row.Tags), $"Tag '{tagSlug}' does not exist."));
            }
        }

        return errors;
    }

    private static Result<ContentExportResult> Export(
        string entityName,
        IReadOnlyCollection<IReadOnlyDictionary<string, string?>> rows,
        string? format)
    {
        var normalizedFormat = string.IsNullOrWhiteSpace(format)
            ? "csv"
            : format.Trim().ToLowerInvariant();

        return normalizedFormat switch
        {
            "json" => Result<ContentExportResult>.Success(new ContentExportResult(
                $"{entityName}.json",
                "application/json",
                JsonSerializer.SerializeToUtf8Bytes(rows, JsonOptions))),
            "csv" => Result<ContentExportResult>.Success(new ContentExportResult(
                $"{entityName}.csv",
                "text/csv",
                Encoding.UTF8.GetBytes(ToCsv(rows)))),
            _ => Result<ContentExportResult>.Validation(
                new ValidationError("format", "Format must be csv or json."))
        };
    }

    private static string ToCsv(IReadOnlyCollection<IReadOnlyDictionary<string, string?>> rows)
    {
        var headers = rows.FirstOrDefault()?.Keys.ToArray() ?? [];
        if (headers.Length == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        builder.AppendLine(string.Join(",", headers.Select(EscapeCsv)));
        foreach (var row in rows)
        {
            builder.AppendLine(string.Join(",", headers.Select(header =>
                EscapeCsv(row.TryGetValue(header, out var value) ? value : null))));
        }

        return builder.ToString();
    }

    private static List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();
        var builder = new StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < line.Length; index++)
        {
            var character = line[index];
            if (character == '"')
            {
                if (inQuotes && index + 1 < line.Length && line[index + 1] == '"')
                {
                    builder.Append('"');
                    index++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (character == ',' && !inQuotes)
            {
                values.Add(builder.ToString());
                builder.Clear();
                continue;
            }

            builder.Append(character);
        }

        values.Add(builder.ToString());
        return values;
    }

    private static void Required(
        int rowNumber,
        string field,
        string? value,
        ICollection<ContentImportError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new ContentImportError(rowNumber, field, $"{field} is required."));
        }
    }

    private static string? Get(IReadOnlyDictionary<string, string?> fields, string name)
    {
        return fields.TryGetValue(name, out var value) ? value?.Trim() : null;
    }

    private static Guid? ResolveCategoryId(string? categorySlug, IReadOnlyDictionary<string, Guid> categories)
    {
        var normalized = NormalizeSlug(categorySlug);
        return string.IsNullOrWhiteSpace(normalized) ? null : categories[normalized];
    }

    private static IReadOnlyCollection<Guid> ResolveTagIds(WordImportRow row, IReadOnlyDictionary<string, Guid> tags)
    {
        return ReadTagSlugs(row)
            .Select(tagSlug => tags[tagSlug])
            .Distinct()
            .ToArray();
    }

    private static IEnumerable<string> ReadTagSlugs(WordImportRow row)
    {
        var rawTagSlugs = row.TagSlugs is { Count: > 0 }
            ? row.TagSlugs
            : (row.Tags ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return rawTagSlugs
            .Select(NormalizeSlug)
            .Where(tagSlug => !string.IsNullOrWhiteSpace(tagSlug))
            .Select(tagSlug => tagSlug!)
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static string? NormalizeSlug(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToLowerInvariant();
    }

    private static string EscapeCsv(string? value)
    {
        var text = value ?? string.Empty;
        return text.Contains(',') || text.Contains('"') || text.Contains('\n') || text.Contains('\r')
            ? $"\"{text.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : text;
    }

    private static bool IsCsv(string fileName, string contentType)
    {
        return fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) ||
            contentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase) ||
            contentType.Equals("application/vnd.ms-excel", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsJson(string fileName, string contentType)
    {
        return fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
            contentType.Equals("application/json", StringComparison.OrdinalIgnoreCase);
    }

    private sealed class WordImportRow
    {
        public int RowNumber { get; set; } = 1;

        public string? Text { get; init; }

        public string? IpaUk { get; init; }

        public string? IpaUs { get; init; }

        public string? ThaiReading { get; init; }

        public string? MeaningTh { get; init; }

        public string? MeaningEn { get; init; }

        public string? PartOfSpeech { get; init; }

        public string? CefrLevel { get; init; }

        public string? ExampleEn { get; init; }

        public string? ExampleTh { get; init; }

        public string? CategorySlug { get; init; }

        public string? Tags { get; init; }

        public IReadOnlyCollection<string>? TagSlugs { get; init; }
    }
}
