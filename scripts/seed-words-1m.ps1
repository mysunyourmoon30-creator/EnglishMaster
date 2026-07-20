param(
    [string]$ConnectionString = "Server=(localdb)\mssqllocaldb;Database=EnglishMaster;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
    [int]$Count = 1000000,
    [int]$BatchSize = 10000,
    [switch]$DeactivateOldSeedWords,
    [switch]$GeneratedWordsActive,
    [switch]$PreviewOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($Count -lt 1) {
    throw "Count must be greater than zero."
}

if ($BatchSize -lt 100 -or $BatchSize -gt 50000) {
    throw "BatchSize must be between 100 and 50000."
}

$roots = @(
    @("ability", "khwam-samat", "the skill to do something", "I can improve my ability with daily practice.", "I can improve my ability with daily practice.", "Noun", "B1"),
    @("accept", "yom-rap", "to agree to receive or allow something", "Please accept this answer.", "Please accept this answer.", "Verb", "A2"),
    @("action", "kan-kratham", "something that a person does", "Small action creates progress.", "Small action creates progress.", "Noun", "A2"),
    @("active", "kratu-rue-ron", "doing things with energy", "Active learners practice every day.", "Active learners practice every day.", "Adjective", "A2"),
    @("advice", "kham-nae-nam", "an opinion that helps someone decide", "Good advice helps me learn faster.", "Good advice helps me learn faster.", "Noun", "A2"),
    @("answer", "kham-top", "something you say or write to reply", "Write your answer clearly.", "Write your answer clearly.", "Noun", "A1"),
    @("arrive", "ma-thueng", "to reach a place", "We arrive at school early.", "We arrive at school early.", "Verb", "A1"),
    @("balance", "khwam-som-dun", "a steady condition where things are equal", "Learning needs balance and rest.", "Learning needs balance and rest.", "Noun", "B1"),
    @("basic", "phuen-than", "simple and important", "This is a basic English lesson.", "This is a basic English lesson.", "Adjective", "A1"),
    @("believe", "chuea", "to think something is true", "I believe practice helps.", "I believe practice helps.", "Verb", "A2"),
    @("careful", "ra-mat-ra-wang", "paying attention to avoid mistakes", "Be careful with spelling.", "Be careful with spelling.", "Adjective", "A2"),
    @("change", "plian-plaeng", "to become different", "Change your plan when you need to.", "Change your plan when you need to.", "Verb", "A1"),
    @("choose", "lueak", "to pick one thing from several things", "Choose the best answer.", "Choose the best answer.", "Verb", "A1"),
    @("clear", "chat-jen", "easy to understand", "Give a clear example.", "Give a clear example.", "Adjective", "A2"),
    @("common", "thua-pai", "happening often", "This is a common mistake.", "This is a common mistake.", "Adjective", "A2"),
    @("compare", "priap-thiap", "to look at two things and see differences", "Compare these two sentences.", "Compare these two sentences.", "Verb", "B1"),
    @("complete", "tham-hai-set", "to finish something", "Complete today's practice.", "Complete today's practice.", "Verb", "A2"),
    @("connect", "chueam-to", "to join things together", "Connect the word with its meaning.", "Connect the word with its meaning.", "Verb", "B1"),
    @("correct", "thuk-tong", "right or without mistakes", "Choose the correct phrase.", "Choose the correct phrase.", "Adjective", "A1"),
    @("daily", "pra-jam-wan", "happening every day", "Daily practice helps you improve.", "Daily practice helps you improve.", "Adjective", "A1"),
    @("describe", "athibai", "to say what something is like", "Describe your morning routine.", "Describe your morning routine.", "Verb", "A2"),
    @("detail", "rai-la-iat", "a small piece of information", "Read the detail before answering.", "Read the detail before answering.", "Noun", "B1"),
    @("develop", "phatthana", "to grow or improve", "Develop your speaking skill.", "Develop your speaking skill.", "Verb", "B1"),
    @("different", "taek-tang", "not the same", "These words have different meanings.", "These words have different meanings.", "Adjective", "A1"),
    @("easy", "ngai", "not difficult", "This lesson is easy to start.", "This lesson is easy to start.", "Adjective", "A1"),
    @("example", "tua-yang", "something that shows how a rule works", "Read the example sentence.", "Read the example sentence.", "Noun", "A1"),
    @("explain", "athibai", "to make something clear", "Explain your answer in English.", "Explain your answer in English.", "Verb", "A2"),
    @("focus", "jot-jo", "to give attention to one thing", "Focus on one skill today.", "Focus on one skill today.", "Verb", "B1"),
    @("grammar", "waiyakon", "rules for making sentences", "Grammar helps us make clear sentences.", "Grammar helps us make clear sentences.", "Noun", "A2"),
    @("habit", "nisai", "something you do often", "A study habit makes learning easier.", "A study habit makes learning easier.", "Noun", "A2"),
    @("improve", "phatthana-hai-di-khuen", "to become better", "Practice to improve your English.", "Practice to improve your English.", "Verb", "A2"),
    @("listen", "fang", "to pay attention to sound", "Listen to the pronunciation.", "Listen to the pronunciation.", "Verb", "A1"),
    @("meaning", "khwam-mai", "what a word or sentence expresses", "Check the meaning before you speak.", "Check the meaning before you speak.", "Noun", "A1"),
    @("practice", "fuek-fon", "to do something many times to improve", "Practice speaking for ten minutes.", "Practice speaking for ten minutes.", "Verb", "A1"),
    @("progress", "khwam-kao-na", "improvement over time", "Your progress is visible this week.", "Your progress is visible this week.", "Noun", "B1"),
    @("question", "kham-tham", "a sentence that asks for information", "Read the question carefully.", "Read the question carefully.", "Noun", "A1"),
    @("review", "thop-thuan", "to study something again", "Review yesterday's words.", "Review yesterday's words.", "Verb", "A2"),
    @("sentence", "prayok", "a group of words with complete meaning", "Make one sentence with this word.", "Make one sentence with this word.", "Noun", "A1"),
    @("speak", "phut", "to say words with your voice", "Speak slowly and clearly.", "Speak slowly and clearly.", "Verb", "A1"),
    @("study", "rian", "to learn about a subject", "Study English every day.", "Study English every day.", "Verb", "A1"),
    @("target", "pao-mai", "something you want to achieve", "Set a small target for today.", "Set a small target for today.", "Noun", "B1"),
    @("useful", "mi-prayoj", "helpful for a purpose", "This phrase is useful in class.", "This phrase is useful in class.", "Adjective", "A2"),
    @("vocabulary", "kham-sap", "words used in a language", "Build your vocabulary step by step.", "Build your vocabulary step by step.", "Noun", "A2"),
    @("write", "khian", "to make words on paper or screen", "Write three example sentences.", "Write three example sentences.", "Verb", "A1")
)

function New-WordsTable {
    $table = New-Object System.Data.DataTable
    [void]$table.Columns.Add("Id", [Guid])
    [void]$table.Columns.Add("Text", [string])
    [void]$table.Columns.Add("Slug", [string])
    [void]$table.Columns.Add("IpaUk", [string])
    [void]$table.Columns.Add("IpaUs", [string])
    [void]$table.Columns.Add("ThaiReading", [string])
    [void]$table.Columns.Add("MeaningTh", [string])
    [void]$table.Columns.Add("MeaningEn", [string])
    [void]$table.Columns.Add("PartOfSpeech", [string])
    [void]$table.Columns.Add("CefrLevel", [string])
    [void]$table.Columns.Add("ExampleEn", [string])
    [void]$table.Columns.Add("ExampleTh", [string])
    [void]$table.Columns.Add("CategoryId", [Guid])
    [void]$table.Columns.Add("ImageMediaId", [Guid])
    [void]$table.Columns.Add("AudioMediaId", [Guid])
    [void]$table.Columns.Add("IsActive", [bool])
    [void]$table.Columns.Add("CreatedAt", [DateTimeOffset])
    [void]$table.Columns.Add("UpdatedAt", [DateTimeOffset])
    return $table
}

function Add-ColumnMappings {
    param([System.Data.SqlClient.SqlBulkCopy]$BulkCopy, [System.Data.DataTable]$Table)

    foreach ($column in $Table.Columns) {
        [void]$BulkCopy.ColumnMappings.Add($column.ColumnName, $column.ColumnName)
    }
}

if ($PreviewOnly) {
    Write-Host "Preview only. No database changes will be made."
    Write-Host "Rows to generate: $Count"
    Write-Host "Batch size: $BatchSize"
    Write-Host "Old seed words will be deactivated: $DeactivateOldSeedWords"
    Write-Host "Generated performance words will be active: $GeneratedWordsActive"
    Write-Host "Sample generated rows:"
    for ($i = 1; $i -le [Math]::Min(5, $Count); $i++) {
        $root = $roots[($i - 1) % $roots.Count]
        $text = "perf {0} {1:D7}" -f $root[0], $i
        $slug = "perf-word-{0:D7}-{1}" -f $i, $root[0]
        Write-Host "  $slug | $text | $($root[1]) | $($root[6])"
    }
    exit 0
}

Add-Type -AssemblyName System.Data

$connection = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
$connection.Open()

try {
    $command = $connection.CreateCommand()
    $command.CommandTimeout = 0

    $command.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Slug = N'vocabulary')
BEGIN
    INSERT INTO dbo.Categories (Id, [Name], Slug, [Description], SortOrder, IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), N'Vocabulary', N'vocabulary', N'Page: Words, Search, Dictionary | Use: English vocabulary, meanings, examples, and learner word bank', 10, 1, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET());
END;

SELECT Id FROM dbo.Categories WHERE Slug = N'vocabulary';
"@
    $categoryId = [Guid]$command.ExecuteScalar()

    if ($DeactivateOldSeedWords) {
        $command.CommandText = @"
DELETE FROM dbo.WordTags
WHERE WordId IN
(
    SELECT Id
    FROM dbo.Words
    WHERE Slug LIKE N'word-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]-%'
       OR Slug LIKE N'perf-word-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]-%'
);

DELETE FROM dbo.Words
WHERE Slug LIKE N'word-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]-%'
   OR Slug LIKE N'perf-word-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]-%';

UPDATE dbo.Words
SET IsActive = 0, UpdatedAt = SYSDATETIMEOFFSET()
WHERE Slug IN (N'hello', N'book', N'learn', N'speak', N'daily')
   OR Slug LIKE N'practice-word-%';
"@
        $oldRows = $command.ExecuteNonQuery()
        Write-Host "Cleaned old generated words and deactivated seed words. Affected rows: $oldRows"
    }

    $now = [DateTimeOffset]::UtcNow
    $inserted = 0
    $isActive = [bool]$GeneratedWordsActive

    while ($inserted -lt $Count) {
        $table = New-WordsTable
        $take = [Math]::Min($BatchSize, $Count - $inserted)

        for ($offset = 1; $offset -le $take; $offset++) {
            $number = $inserted + $offset
            $root = $roots[($number - 1) % $roots.Count]
            $level = $root[6]
            $text = "perf {0} {1:D7}" -f $root[0], $number
            $slug = "perf-word-{0:D7}-{1}" -f $number, $root[0]
            $meaningTh = "{0} - learning vocabulary item {1:N0}" -f $root[1], $number
            $meaningEn = "{0}. Learning vocabulary item #{1:N0} for search, paging, and practice performance." -f $root[2], $number

            $row = $table.NewRow()
            $row["Id"] = [Guid]::NewGuid()
            $row["Text"] = $text
            $row["Slug"] = $slug
            $row["IpaUk"] = "/$($root[0])/"
            $row["IpaUs"] = "/$($root[0])/"
            $row["ThaiReading"] = $root[1]
            $row["MeaningTh"] = $meaningTh
            $row["MeaningEn"] = $meaningEn
            $row["PartOfSpeech"] = $root[5]
            $row["CefrLevel"] = $level
            $row["ExampleEn"] = $root[3]
            $row["ExampleTh"] = $root[4]
            $row["CategoryId"] = $categoryId
            $row["ImageMediaId"] = [DBNull]::Value
            $row["AudioMediaId"] = [DBNull]::Value
            $row["IsActive"] = $isActive
            $row["CreatedAt"] = $now
            $row["UpdatedAt"] = $now
            [void]$table.Rows.Add($row)
        }

        $bulkCopy = New-Object System.Data.SqlClient.SqlBulkCopy($connection)
        $bulkCopy.DestinationTableName = "dbo.Words"
        $bulkCopy.BatchSize = $BatchSize
        $bulkCopy.BulkCopyTimeout = 0
        Add-ColumnMappings -BulkCopy $bulkCopy -Table $table
        $bulkCopy.WriteToServer($table)
        $bulkCopy.Close()

        $inserted += $take
        Write-Host ("Inserted {0:N0}/{1:N0} words" -f $inserted, $Count)
    }
}
finally {
    $connection.Close()
}

Write-Host "Done. Inserted $Count words."
