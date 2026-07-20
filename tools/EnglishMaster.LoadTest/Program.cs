using System.Data;
using EnglishMaster.Infrastructure;
using EnglishMaster.Infrastructure.Persistence;
using EnglishMaster.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var options = LoadTestOptions.Parse(args);
var connectionString = $"Server={options.SqlServer};Database={options.DatabaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False";

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Database:Provider"] = "SqlServer",
        ["Database:Name"] = options.DatabaseName,
        ["ConnectionStrings:DefaultConnection"] = connectionString,
        ["Auth:InitialSuperAdmin:Email"] = options.AdminEmail,
        ["Auth:InitialSuperAdmin:Password"] = options.AdminPassword,
        ["DevelopmentSeed:Enabled"] = "false",
        ["EmailDeliveryWorker:Enabled"] = "false",
        ["SystemHealthWorker:Enabled"] = "false"
    })
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddSingleton(TimeProvider.System);
services.AddInfrastructure(connectionString, configuration);

await using var serviceProvider = services.BuildServiceProvider();
await SecuritySeeder.SeedSecurityAsync(serviceProvider, configuration);

await using var scope = serviceProvider.CreateAsyncScope();
var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
dbContext.Database.SetCommandTimeout(0);

if (options.Reset && !options.DatabaseName.Contains("LoadTest", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException("Refusing to reset a database whose name does not contain LoadTest.");
}

var categoryCount = 100;
var quizCount = 4_900;
var questionCount = 1_000;
var choiceCount = 4_000;
var wordCount = options.TotalRecords - categoryCount - quizCount - questionCount - choiceCount;
if (wordCount < 1)
{
    throw new InvalidOperationException("TotalRecords is too small for the load-test mix.");
}

Console.WriteLine($"Database: {options.DatabaseName}");
Console.WriteLine($"Target records: {options.TotalRecords:N0}");
Console.WriteLine($"Categories: {categoryCount:N0}, Words: {wordCount:N0}, Quizzes: {quizCount:N0}, Questions: {questionCount:N0}, Choices: {choiceCount:N0}");

if (options.Reset)
{
    await ExecuteAsync(dbContext, "Reset load tables", """
        DELETE FROM [QuizChoices];
        DELETE FROM [QuizQuestions];
        DELETE FROM [Quizzes];
        DELETE FROM [Words];
        DELETE FROM [Categories];
        """);
}

await ExecuteAsync(dbContext, "Seed categories", $"""
    WITH n AS (
        SELECT TOP ({categoryCount}) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
        FROM sys.all_objects
    )
    INSERT INTO [Categories] ([Id], [Name], [Slug], [Description], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt])
    SELECT
        NEWID(),
        CONCAT('Load Category ', rn),
        CONCAT('load-category-', FORMAT(rn, '000')),
        'Load-test category',
        rn,
        CAST(1 AS bit),
        SYSDATETIMEOFFSET(),
        SYSDATETIMEOFFSET()
    FROM n;
    """);

await ExecuteAsync(dbContext, "Seed words", $"""
    WITH n AS (
        SELECT TOP ({wordCount}) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
        FROM sys.all_objects a
        CROSS JOIN sys.all_objects b
        CROSS JOIN sys.all_objects c
    ),
    c AS (
        SELECT [Id], ROW_NUMBER() OVER (ORDER BY [Slug]) AS rn
        FROM [Categories]
        WHERE [Slug] LIKE 'load-category-%'
    )
    INSERT INTO [Words] (
        [Id], [Text], [Slug], [IpaUk], [IpaUs], [ThaiReading], [MeaningTh], [MeaningEn],
        [PartOfSpeech], [CefrLevel], [ExampleEn], [ExampleTh], [CategoryId],
        [ImageMediaId], [AudioMediaId], [IsActive], [CreatedAt], [UpdatedAt])
    SELECT
        NEWID(),
        CONCAT('load-word-', FORMAT(n.rn, '0000000')),
        CONCAT('load-word-', FORMAT(n.rn, '0000000')),
        '/load/',
        '/load/',
        CONCAT('reading ', n.rn),
        CONCAT('meaning th ', n.rn),
        CONCAT('meaning en ', n.rn),
        CASE n.rn % 6
            WHEN 0 THEN 'Noun'
            WHEN 1 THEN 'Verb'
            WHEN 2 THEN 'Adjective'
            WHEN 3 THEN 'Adverb'
            WHEN 4 THEN 'Phrase'
            ELSE 'Other'
        END,
        CASE n.rn % 6
            WHEN 0 THEN 'A1'
            WHEN 1 THEN 'A2'
            WHEN 2 THEN 'B1'
            WHEN 3 THEN 'B2'
            WHEN 4 THEN 'C1'
            ELSE 'C2'
        END,
        CONCAT('Example sentence ', n.rn),
        CONCAT('Example translation ', n.rn),
        c.[Id],
        NULL,
        NULL,
        CAST(CASE WHEN n.rn % 20 = 0 THEN 0 ELSE 1 END AS bit),
        SYSDATETIMEOFFSET(),
        SYSDATETIMEOFFSET()
    FROM n
    INNER JOIN c ON c.rn = ((n.rn - 1) % {categoryCount}) + 1;
    """);

await ExecuteAsync(dbContext, "Seed quizzes", $"""
    WITH n AS (
        SELECT TOP ({quizCount}) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
        FROM sys.all_objects a
        CROSS JOIN sys.all_objects b
        CROSS JOIN sys.all_objects c
    ),
    c AS (
        SELECT [Id], ROW_NUMBER() OVER (ORDER BY [Slug]) AS rn
        FROM [Categories]
        WHERE [Slug] LIKE 'load-category-%'
    )
    INSERT INTO [Quizzes] (
        [Id], [Title], [Slug], [Summary], [Description], [CefrLevel], [CategoryId],
        [LessonId], [CourseId], [BookId], [TimeLimitMinutes], [PassingScore],
        [SortOrder], [IsPublished], [IsActive], [CreatedAt], [UpdatedAt])
    SELECT
        NEWID(),
        CONCAT('Load Quiz ', FORMAT(n.rn, '000000')),
        CONCAT('load-quiz-', FORMAT(n.rn, '000000')),
        CONCAT('Load quiz summary ', n.rn),
        CONCAT('Load quiz description ', n.rn),
        CASE n.rn % 6
            WHEN 0 THEN 'A1'
            WHEN 1 THEN 'A2'
            WHEN 2 THEN 'B1'
            WHEN 3 THEN 'B2'
            WHEN 4 THEN 'C1'
            ELSE 'C2'
        END,
        c.[Id],
        NULL,
        NULL,
        NULL,
        10 + (n.rn % 30),
        70,
        n.rn,
        CAST(CASE WHEN n.rn % 3 = 0 THEN 0 ELSE 1 END AS bit),
        CAST(1 AS bit),
        SYSDATETIMEOFFSET(),
        SYSDATETIMEOFFSET()
    FROM n
    INNER JOIN c ON c.rn = ((n.rn - 1) % {categoryCount}) + 1;
    """);

await ExecuteAsync(dbContext, "Seed quiz questions", $"""
    WITH n AS (
        SELECT TOP ({questionCount}) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
        FROM sys.all_objects
    ),
    q AS (
        SELECT [Id], ROW_NUMBER() OVER (ORDER BY [Slug]) AS rn
        FROM [Quizzes]
        WHERE [Slug] LIKE 'load-quiz-%'
    )
    INSERT INTO [QuizQuestions] (
        [Id], [QuizId], [QuestionText], [QuestionType], [ExplanationTh], [ExplanationEn],
        [Points], [SortOrder], [WordId], [GrammarRuleId], [PronunciationId],
        [IsActive], [CreatedAt], [UpdatedAt])
    SELECT
        NEWID(),
        q.[Id],
        CONCAT('Load question ', FORMAT(n.rn, '000000')),
        'SingleChoice',
        CONCAT('Load explanation TH ', n.rn),
        CONCAT('Load explanation EN ', n.rn),
        1,
        n.rn,
        NULL,
        NULL,
        NULL,
        CAST(1 AS bit),
        SYSDATETIMEOFFSET(),
        SYSDATETIMEOFFSET()
    FROM n
    INNER JOIN q ON q.rn = ((n.rn - 1) % {quizCount}) + 1;
    """);

await ExecuteAsync(dbContext, "Seed quiz choices", $"""
    WITH n AS (
        SELECT TOP ({choiceCount}) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
        FROM sys.all_objects a
        CROSS JOIN sys.all_objects b
        CROSS JOIN sys.all_objects c
    ),
    questions AS (
        SELECT [Id], ROW_NUMBER() OVER (ORDER BY [QuestionText]) AS rn
        FROM [QuizQuestions]
        WHERE [QuestionText] LIKE 'Load question %'
    )
    INSERT INTO [QuizChoices] (
        [Id], [QuizQuestionId], [ChoiceText], [IsCorrect], [ExplanationTh], [ExplanationEn],
        [SortOrder], [IsActive], [CreatedAt], [UpdatedAt])
    SELECT
        NEWID(),
        questions.[Id],
        CONCAT('Load choice ', FORMAT(n.rn, '000000')),
        CAST(CASE WHEN ((n.rn - 1) % 4) = 0 THEN 1 ELSE 0 END AS bit),
        CONCAT('Load choice explanation TH ', n.rn),
        CONCAT('Load choice explanation EN ', n.rn),
        ((n.rn - 1) % 4) + 1,
        CAST(1 AS bit),
        SYSDATETIMEOFFSET(),
        SYSDATETIMEOFFSET()
    FROM n
    INNER JOIN questions ON questions.rn = ((n.rn - 1) % {questionCount}) + 1;
    """);

var counts = await CountAsync(dbContext);
var total = counts.Values.Sum();
Console.WriteLine();
foreach (var count in counts)
{
    Console.WriteLine($"{count.Key}: {count.Value:N0}");
}

Console.WriteLine($"Total load-test records: {total:N0}");
if (total < options.TotalRecords)
{
    throw new InvalidOperationException($"Expected at least {options.TotalRecords:N0} load-test records, but found {total:N0}.");
}

static async Task ExecuteAsync(EnglishMasterDbContext dbContext, string name, string sql)
{
    var started = DateTimeOffset.UtcNow;
    Console.Write($"{name}... ");
    var connection = dbContext.Database.GetDbConnection();
    if (connection.State != ConnectionState.Open)
    {
        await connection.OpenAsync();
    }

    await using var command = connection.CreateCommand();
    command.CommandText = sql;
    command.CommandTimeout = 0;
    await command.ExecuteNonQueryAsync();
    Console.WriteLine($"done in {(DateTimeOffset.UtcNow - started).TotalSeconds:N1}s");
}

static async Task<IReadOnlyDictionary<string, long>> CountAsync(EnglishMasterDbContext dbContext)
{
    var sql = """
        SELECT 'Categories', COUNT_BIG(*) FROM [Categories] WHERE [Slug] LIKE 'load-category-%'
        UNION ALL SELECT 'Words', COUNT_BIG(*) FROM [Words] WHERE [Slug] LIKE 'load-word-%'
        UNION ALL SELECT 'Quizzes', COUNT_BIG(*) FROM [Quizzes] WHERE [Slug] LIKE 'load-quiz-%'
        UNION ALL SELECT 'QuizQuestions', COUNT_BIG(*) FROM [QuizQuestions] WHERE [QuestionText] LIKE 'Load question %'
        UNION ALL SELECT 'QuizChoices', COUNT_BIG(*) FROM [QuizChoices] WHERE [ChoiceText] LIKE 'Load choice %';
        """;
    var connection = dbContext.Database.GetDbConnection();
    if (connection.State != ConnectionState.Open)
    {
        await connection.OpenAsync();
    }

    await using var command = connection.CreateCommand();
    command.CommandText = sql;
    command.CommandTimeout = 0;
    await using var reader = await command.ExecuteReaderAsync();
    var counts = new Dictionary<string, long>();
    while (await reader.ReadAsync())
    {
        counts.Add(reader.GetString(0), reader.GetInt64(1));
    }

    return counts;
}

internal sealed record LoadTestOptions(
    string SqlServer,
    string DatabaseName,
    int TotalRecords,
    bool Reset,
    string AdminEmail,
    string AdminPassword)
{
    public static LoadTestOptions Parse(string[] args)
    {
        var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var reset = true;
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg.Equals("--no-reset", StringComparison.OrdinalIgnoreCase))
            {
                reset = false;
                continue;
            }

            if (!arg.StartsWith("--", StringComparison.Ordinal))
            {
                continue;
            }

            var key = arg[2..];
            var value = i + 1 < args.Length ? args[++i] : null;
            values[key] = value;
        }

        var adminPassword = Value("admin-password")
            ?? Environment.GetEnvironmentVariable("ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD")
            ?? "LoadTestPassword1!";

        return new LoadTestOptions(
            Value("sql-server") ?? "localhost",
            Value("database") ?? "EnglishMasterLoadTest",
            int.TryParse(Value("records"), out var records) ? records : 1_000_000,
            reset,
            Value("admin-email") ?? "load.admin@englishmaster.local",
            adminPassword);

        string? Value(string key) => values.TryGetValue(key, out var value) ? value : null;
    }
}
