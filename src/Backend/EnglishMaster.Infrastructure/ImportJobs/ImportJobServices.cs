using System.Text.Json;
using EnglishMaster.Application.Features.ImportJobs;
using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Common;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.ImportJobs;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.ImportJobs;

public sealed class ImportParser : IImportParser
{
    public IReadOnlyCollection<string> ParseRows(string format, string content)
    {
        if (format.Equals("JSON", StringComparison.OrdinalIgnoreCase))
        {
            using var document = JsonDocument.Parse(content);
            return document.RootElement.ValueKind == JsonValueKind.Array
                ? document.RootElement.EnumerateArray().Select(element => element.GetRawText()).ToArray()
                : [document.RootElement.GetRawText()];
        }

        var lines = content.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length <= 1)
        {
            return [];
        }

        var headers = SplitCsv(lines[0]);
        return lines.Skip(1)
            .Select(line => SplitCsv(line))
            .Select(values => headers.Select((header, index) => new { header, value = index < values.Length ? values[index] : string.Empty }).ToDictionary(item => item.header, item => item.value))
            .Select(row => JsonSerializer.Serialize(row))
            .ToArray();
    }

    private static string[] SplitCsv(string line) =>
        line.Split(',').Select(value => value.Trim().Trim('"')).ToArray();
}

public sealed class ImportJobService : IImportValidationService, IImportPreviewService, IImportRunService, IImportRollbackService
{
    private readonly EnglishMasterDbContext dbContext;
    private readonly TimeProvider timeProvider;

    public ImportJobService(EnglishMasterDbContext dbContext, TimeProvider timeProvider)
    {
        this.dbContext = dbContext;
        this.timeProvider = timeProvider;
    }

    public async Task ValidateAsync(ImportJob job, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        job.StartValidation(now);
        var seenByImportType = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in job.Rows)
        {
            switch (job.ImportType)
            {
                case "words":
                    await ValidateWordAsync(row, Seen(seenByImportType, "words"), now, cancellationToken);
                    break;
                case "categories":
                    await ValidateCategoryAsync(row, Seen(seenByImportType, "categories"), now, cancellationToken);
                    break;
                case "tags":
                    await ValidateTagAsync(row, Seen(seenByImportType, "tags"), now, cancellationToken);
                    break;
                case "grammartopics":
                    await ValidateGrammarTopicAsync(row, Seen(seenByImportType, "grammartopics"), now, cancellationToken);
                    break;
                case "grammarrules":
                    await ValidateGrammarRuleAsync(row, Seen(seenByImportType, "grammarrules"), now, cancellationToken);
                    break;
                case "lessons":
                    await ValidateLessonAsync(row, Seen(seenByImportType, "lessons"), now, cancellationToken);
                    break;
                case "courses":
                    await ValidateCourseAsync(row, Seen(seenByImportType, "courses"), now, cancellationToken);
                    break;
                case "books":
                    await ValidateBookAsync(row, Seen(seenByImportType, "books"), now, cancellationToken);
                    break;
                case "quizzes":
                    ValidateQuiz(row, now);
                    break;
                default:
                    AddError(row, "ImportType", "IMPORT_TYPE_UNSUPPORTED", "Import type is not supported.", now);
                    break;
            }
        }

        job.FinishValidation(now);
    }

    public Task ConfirmAsync(ImportJob job, CancellationToken cancellationToken)
    {
        job.Confirm(timeProvider.GetUtcNow());
        return Task.CompletedTask;
    }

    public async Task RunAsync(ImportJob job, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        job.StartImport(now);
        foreach (var row in job.Rows.Where(row => row.Status == ImportJobRowStatus.Valid))
        {
            if (job.ImportType == "words")
            {
                await ImportWordAsync(row, now, cancellationToken);
            }
            else
            {
                row.MarkFailed("Import type is not implemented yet.", now);
            }
        }

        job.FinishImport(timeProvider.GetUtcNow());
    }

    public async Task RollbackAsync(ImportJob job, CancellationToken cancellationToken)
    {
        foreach (var row in job.Rows.Where(row => row.Status == ImportJobRowStatus.Imported && row.CreatedEntityType == "Word" && row.CreatedEntityId.HasValue))
        {
            var createdEntityId = row.CreatedEntityId.GetValueOrDefault();
            var word = await dbContext.Words.SingleOrDefaultAsync(item => item.Id == createdEntityId, cancellationToken);
            if (word is not null)
            {
                dbContext.Words.Remove(word);
            }
        }

        job.MarkRolledBack(timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateWordAsync(ImportJobRow row, HashSet<string> seenWords, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var values = ReadValues(row.RawDataJson);
        var text = Value(values, "Text");
        var meaningTh = Value(values, "MeaningTh");
        var cefr = Value(values, "CefrLevel");

        if (string.IsNullOrWhiteSpace(text))
        {
            AddError(row, "Text", "WORD_TEXT_REQUIRED", "Text is required.", now);
        }
        else if (!seenWords.Add(text) || await dbContext.Words.AnyAsync(word => word.Text == text, cancellationToken))
        {
            AddError(row, "Text", "WORD_TEXT_DUPLICATE", "Word text is duplicated.", now);
        }

        if (string.IsNullOrWhiteSpace(meaningTh))
        {
            AddError(row, "MeaningTh", "WORD_MEANING_TH_REQUIRED", "MeaningTh is required.", now);
        }

        if (!string.IsNullOrWhiteSpace(cefr) && !Enum.TryParse<CefrLevel>(cefr, ignoreCase: true, out _))
        {
            AddError(row, "CefrLevel", "WORD_CEFR_INVALID", "CEFR value is invalid.", now);
        }

        MarkValidWhenClean(row, values, now);
    }

    private async Task ValidateCategoryAsync(ImportJobRow row, HashSet<string> seen, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var values = ReadValues(row.RawDataJson);
        var name = Value(values, "Name");
        var slug = Value(values, "Slug");
        await ValidateNameSlugAsync<Category>(row, values, seen, name, slug, "CATEGORY", now, cancellationToken);
    }

    private async Task ValidateTagAsync(ImportJobRow row, HashSet<string> seen, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var values = ReadValues(row.RawDataJson);
        var name = Value(values, "Name");
        var slug = Value(values, "Slug");
        await ValidateNameSlugAsync<Tag>(row, values, seen, name, slug, "TAG", now, cancellationToken);
    }

    private async Task ValidateGrammarTopicAsync(ImportJobRow row, HashSet<string> seen, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var values = ReadValues(row.RawDataJson);
        var title = Value(values, "Title");
        if (string.IsNullOrWhiteSpace(title))
        {
            AddError(row, "Title", "GRAMMAR_TOPIC_TITLE_REQUIRED", "Grammar topic title is required.", now);
        }
        else if (!seen.Add(title) || await dbContext.GrammarTopics.AnyAsync(topic => topic.Title == title, cancellationToken))
        {
            AddError(row, "Title", "GRAMMAR_TOPIC_TITLE_DUPLICATE", "Grammar topic title is duplicated.", now);
        }

        MarkValidWhenClean(row, values, now);
    }

    private async Task ValidateGrammarRuleAsync(ImportJobRow row, HashSet<string> seen, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var values = ReadValues(row.RawDataJson);
        var title = Value(values, "Title");
        var ruleText = Value(values, "RuleText");
        var exampleEn = Value(values, "ExampleEn");
        if (string.IsNullOrWhiteSpace(title))
        {
            AddError(row, "Title", "GRAMMAR_RULE_TITLE_REQUIRED", "Grammar rule title is required.", now);
        }
        else if (!seen.Add(title) || await dbContext.GrammarRules.AnyAsync(rule => rule.Title == title, cancellationToken))
        {
            AddError(row, "Title", "GRAMMAR_RULE_TITLE_DUPLICATE", "Grammar rule title is duplicated.", now);
        }

        if (string.IsNullOrWhiteSpace(ruleText))
        {
            AddError(row, "RuleText", "GRAMMAR_RULE_TEXT_REQUIRED", "Grammar rule text is required.", now);
        }

        if (values.ContainsKey("ExampleEn") && string.IsNullOrWhiteSpace(exampleEn))
        {
            AddError(row, "ExampleEn", "GRAMMAR_EXAMPLE_EN_REQUIRED", "Grammar example English text is required when provided.", now);
        }

        MarkValidWhenClean(row, values, now);
    }

    private async Task ValidateLessonAsync(ImportJobRow row, HashSet<string> seen, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var values = ReadValues(row.RawDataJson);
        var title = Value(values, "Title");
        var slug = Value(values, "Slug");
        if (string.IsNullOrWhiteSpace(title))
        {
            AddError(row, "Title", "LESSON_TITLE_REQUIRED", "Lesson title is required.", now);
        }
        else if (!seen.Add(string.IsNullOrWhiteSpace(slug) ? title : slug) || await dbContext.Lessons.AnyAsync(lesson => lesson.Title == title || (!string.IsNullOrWhiteSpace(slug) && lesson.Slug == slug), cancellationToken))
        {
            AddError(row, "Slug", "LESSON_DUPLICATE", "Lesson title or slug is duplicated.", now);
        }

        ValidateNonNegative(row, values, "EstimatedMinutes", "LESSON_ESTIMATED_MINUTES_NEGATIVE", now);
        MarkValidWhenClean(row, values, now);
    }

    private async Task ValidateCourseAsync(ImportJobRow row, HashSet<string> seen, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var values = ReadValues(row.RawDataJson);
        var title = Value(values, "Title");
        var slug = Value(values, "Slug");
        if (string.IsNullOrWhiteSpace(title))
        {
            AddError(row, "Title", "COURSE_TITLE_REQUIRED", "Course title is required.", now);
        }
        else if (!seen.Add(string.IsNullOrWhiteSpace(slug) ? title : slug) || await dbContext.Courses.AnyAsync(course => course.Title == title || (!string.IsNullOrWhiteSpace(slug) && course.Slug == slug), cancellationToken))
        {
            AddError(row, "Slug", "COURSE_DUPLICATE", "Course title or slug is duplicated.", now);
        }

        ValidateNonNegative(row, values, "EstimatedMinutes", "COURSE_ESTIMATED_MINUTES_NEGATIVE", now);
        MarkValidWhenClean(row, values, now);
    }

    private async Task ValidateBookAsync(ImportJobRow row, HashSet<string> seen, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var values = ReadValues(row.RawDataJson);
        var title = Value(values, "Title");
        var slug = Value(values, "Slug");
        if (string.IsNullOrWhiteSpace(title))
        {
            AddError(row, "Title", "BOOK_TITLE_REQUIRED", "Book title is required.", now);
        }
        else if (!seen.Add(string.IsNullOrWhiteSpace(slug) ? title : slug) || await dbContext.Books.AnyAsync(book => book.Title == title || (!string.IsNullOrWhiteSpace(slug) && book.Slug == slug), cancellationToken))
        {
            AddError(row, "Slug", "BOOK_DUPLICATE", "Book title or slug is duplicated.", now);
        }

        ValidateNonNegative(row, values, "EstimatedPages", "BOOK_ESTIMATED_PAGES_NEGATIVE", now);
        MarkValidWhenClean(row, values, now);
    }

    private void ValidateQuiz(ImportJobRow row, DateTimeOffset now)
    {
        var values = ReadValues(row.RawDataJson);
        if (string.IsNullOrWhiteSpace(Value(values, "Title")))
        {
            AddError(row, "Title", "QUIZ_TITLE_REQUIRED", "Quiz title is required.", now);
        }

        var passingScore = Value(values, "PassingScore");
        if (!string.IsNullOrWhiteSpace(passingScore) && (!int.TryParse(passingScore, out var score) || score is < 0 or > 100))
        {
            AddError(row, "PassingScore", "QUIZ_PASSING_SCORE_INVALID", "Quiz passing score must be between 0 and 100.", now);
        }

        MarkValidWhenClean(row, values, now);
    }

    private async Task ValidateNameSlugAsync<TEntity>(ImportJobRow row, IReadOnlyDictionary<string, string> values, HashSet<string> seen, string name, string slug, string prefix, DateTimeOffset now, CancellationToken cancellationToken)
        where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            AddError(row, "Name", $"{prefix}_NAME_REQUIRED", "Name is required.", now);
            return;
        }

        var duplicateKey = string.IsNullOrWhiteSpace(slug) ? name : slug;
        var duplicateInFile = !seen.Add(duplicateKey);
        var duplicateInDatabase = typeof(TEntity) == typeof(Category)
            ? await dbContext.Categories.AnyAsync(category => category.Name == name || (!string.IsNullOrWhiteSpace(slug) && category.Slug == slug), cancellationToken)
            : await dbContext.Tags.AnyAsync(tag => tag.Name == name || (!string.IsNullOrWhiteSpace(slug) && tag.Slug == slug), cancellationToken);

        if (duplicateInFile || duplicateInDatabase)
        {
            AddError(row, string.IsNullOrWhiteSpace(slug) ? "Name" : "Slug", $"{prefix}_DUPLICATE", "Name or slug is duplicated.", now);
        }

        MarkValidWhenClean(row, values, now);
    }

    private async Task ImportWordAsync(ImportJobRow row, DateTimeOffset now, CancellationToken cancellationToken)
    {
        try
        {
            var values = ReadValues(row.ParsedDataJson);
            var word = Word.Create(
                Value(values, "Text"),
                Value(values, "IpaUk"),
                Value(values, "IpaUs"),
                Value(values, "ThaiReading"),
                Value(values, "MeaningTh"),
                Value(values, "MeaningEn"),
                Enum.TryParse<PartOfSpeech>(Value(values, "PartOfSpeech"), ignoreCase: true, out var partOfSpeech) ? partOfSpeech : PartOfSpeech.Noun,
                Enum.TryParse<CefrLevel>(Value(values, "CefrLevel"), ignoreCase: true, out var cefr) ? cefr : CefrLevel.A1,
                Value(values, "ExampleEn"),
                Value(values, "ExampleTh"),
                now);
            dbContext.Words.Add(word);
            await dbContext.SaveChangesAsync(cancellationToken);
            row.MarkImported("Word", word.Id, now);
        }
        catch (ArgumentException exception)
        {
            row.MarkFailed(exception.Message, now);
        }
    }

    private static string Value(IReadOnlyDictionary<string, string> values, string key) =>
        values.TryGetValue(key, out var value) ? value : string.Empty;

    private static HashSet<string> Seen(IDictionary<string, HashSet<string>> seenByImportType, string key)
    {
        if (!seenByImportType.TryGetValue(key, out var seen))
        {
            seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            seenByImportType[key] = seen;
        }

        return seen;
    }

    private static Dictionary<string, string> ReadValues(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.ValueKind == JsonValueKind.Object
            ? document.RootElement.EnumerateObject().ToDictionary(property => property.Name, property => property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() ?? string.Empty : property.Value.ToString(), StringComparer.OrdinalIgnoreCase)
            : [];
    }

    private void AddError(ImportJobRow row, string fieldName, string errorCode, string errorMessage, DateTimeOffset now) =>
        dbContext.ImportValidationErrors.Add(row.AddError(fieldName, errorCode, errorMessage, ImportValidationSeverity.Error, now));

    private static void MarkValidWhenClean(ImportJobRow row, IReadOnlyDictionary<string, string> values, DateTimeOffset now)
    {
        if (row.Status != ImportJobRowStatus.Invalid)
        {
            row.MarkValid(JsonSerializer.Serialize(values), now);
        }
    }

    private void ValidateNonNegative(ImportJobRow row, IReadOnlyDictionary<string, string> values, string fieldName, string errorCode, DateTimeOffset now)
    {
        var value = Value(values, fieldName);
        if (!string.IsNullOrWhiteSpace(value) && (!int.TryParse(value, out var parsed) || parsed < 0))
        {
            AddError(row, fieldName, errorCode, $"{fieldName} must not be negative.", now);
        }
    }
}
