param(
    [string]$ConnectionString = "Server=.;Database=EnglishMasterInternal;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False",
    [int]$Count = 2500,
    [string]$SourceUrl = "https://raw.githubusercontent.com/first20hours/google-10000-english/master/google-10000-english-usa-no-swears.txt",
    [string]$LocalWordListPath = "",
    [switch]$DeactivateGeneratedWords,
    [switch]$PreviewOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($Count -lt 1 -or $Count -gt 10000) {
    throw "Count must be between 1 and 10000."
}

function ConvertTo-Slug {
    param([string]$Value)

    $slug = ($Value.Trim().ToLowerInvariant() -replace "[^a-z0-9]+", "-").Trim("-")
    if ([string]::IsNullOrWhiteSpace($slug)) {
        throw "Cannot create slug for value '$Value'."
    }

    return $slug
}

function Get-SourceWords {
    if (-not [string]::IsNullOrWhiteSpace($LocalWordListPath)) {
        if (-not (Test-Path -LiteralPath $LocalWordListPath)) {
            throw "Local word list was not found: $LocalWordListPath"
        }

        return Get-Content -LiteralPath $LocalWordListPath |
            Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    }

    $tempPath = Join-Path $env:TEMP "englishmaster-google-10000-english-usa-no-swears.txt"
    Invoke-WebRequest -Uri $SourceUrl -OutFile $tempPath -UseBasicParsing
    return Get-Content -LiteralPath $tempPath |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
}

function New-WordsTable {
    $table = New-Object System.Data.DataTable
    [void]$table.Columns.Add("Rank", [int])
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
    [void]$table.Columns.Add("CreatedAt", [DateTimeOffset])
    [void]$table.Columns.Add("UpdatedAt", [DateTimeOffset])
    return ,$table
}

function Add-ColumnMappings {
    param([System.Data.SqlClient.SqlBulkCopy]$BulkCopy, [System.Data.DataTable]$Table)

    foreach ($column in $Table.Columns) {
        [void]$BulkCopy.ColumnMappings.Add($column.ColumnName, $column.ColumnName)
    }
}

$sourceWords = @(Get-SourceWords |
    ForEach-Object { $_.Trim().ToLowerInvariant() } |
    Where-Object { $_ -match "^[a-z]+$" } |
    Select-Object -Unique |
    Select-Object -First $Count)

if ($sourceWords.Count -lt $Count) {
    throw "Only found $($sourceWords.Count) source words, but Count is $Count."
}

$rankBase = [DateTimeOffset]::Parse("2026-01-01T00:00:00+00:00")
$table = New-WordsTable

for ($index = 0; $index -lt $sourceWords.Count; $index++) {
    $rank = $index + 1
    $word = $sourceWords[$index]
    $rankTime = $rankBase.AddSeconds($rank)
    $cefr = if ($rank -le 700) { "A1" } elseif ($rank -le 1500) { "A2" } elseif ($rank -le 2200) { "B1" } else { "B2" }
    $partOfSpeech = if ($word.Length -le 2) { "Other" } else { "Other" }

    $row = $table.NewRow()
    $row["Rank"] = $rank
    $row["Text"] = $word
    $row["Slug"] = ConvertTo-Slug $word
    $row["IpaUk"] = "/$word/"
    $row["IpaUs"] = "/$word/"
    $row["ThaiReading"] = $word
    $row["MeaningTh"] = "Common English word rank #$rank"
    $row["MeaningEn"] = "Common English word rank #$rank. Add a reviewed learner definition here."
    $row["PartOfSpeech"] = $partOfSpeech
    $row["CefrLevel"] = $cefr
    $row["ExampleEn"] = "Practice the word '$word' in a short sentence."
    $row["ExampleTh"] = "Practice the word '$word' in a short sentence."
    $row["CreatedAt"] = $rankTime
    $row["UpdatedAt"] = $rankTime
    [void]$table.Rows.Add($row)
}

if ($PreviewOnly) {
    Write-Host "Preview only. No database changes will be made."
    Write-Host "Source: $SourceUrl"
    Write-Host "Rows: $($table.Rows.Count)"
    Write-Host "Top 20 words:"
    $table.Rows | Select-Object -First 20 | ForEach-Object {
        Write-Host ("  #{0}: {1}" -f $_["Rank"], $_["Text"])
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
IF OBJECT_ID('tempdb..#CommonWords') IS NOT NULL DROP TABLE #CommonWords;

CREATE TABLE #CommonWords
(
    [Rank] int NOT NULL,
    [Text] nvarchar(200) NOT NULL,
    Slug nvarchar(220) NOT NULL,
    IpaUk nvarchar(100) NOT NULL,
    IpaUs nvarchar(100) NOT NULL,
    ThaiReading nvarchar(200) NOT NULL,
    MeaningTh nvarchar(1000) NOT NULL,
    MeaningEn nvarchar(1000) NOT NULL,
    PartOfSpeech nvarchar(50) NOT NULL,
    CefrLevel nvarchar(10) NOT NULL,
    ExampleEn nvarchar(1000) NOT NULL,
    ExampleTh nvarchar(1000) NOT NULL,
    CreatedAt datetimeoffset NOT NULL,
    UpdatedAt datetimeoffset NOT NULL
);
"@
    [void]$command.ExecuteNonQuery()

    $bulkCopy = New-Object System.Data.SqlClient.SqlBulkCopy($connection)
    $bulkCopy.DestinationTableName = "#CommonWords"
    $bulkCopy.BatchSize = 2500
    $bulkCopy.BulkCopyTimeout = 0
    Add-ColumnMappings -BulkCopy $bulkCopy -Table $table
    $bulkCopy.WriteToServer($table)
    $bulkCopy.Close()

    $command.CommandText = @"
DECLARE @Now datetimeoffset = SYSDATETIMEOFFSET();
DECLARE @CategoryId uniqueidentifier;

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Slug = N'vocabulary')
BEGIN
    INSERT INTO dbo.Categories (Id, [Name], Slug, [Description], SortOrder, IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), N'Vocabulary', N'vocabulary', N'Page: Words, Search, Dictionary | Use: English vocabulary, meanings, examples, and learner word bank', 10, 1, @Now, @Now);
END;

SELECT @CategoryId = Id FROM dbo.Categories WHERE Slug = N'vocabulary';

UPDATE dbo.Words
SET IsActive = 0, UpdatedAt = @Now
WHERE Slug IN (N'hello', N'book', N'learn', N'speak', N'daily')
   OR Slug LIKE N'practice-word-%'
   OR Slug LIKE N'word-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]-%'
   OR Slug LIKE N'perf-word-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]-%';

MERGE dbo.Words AS Target
USING #CommonWords AS Source
    ON Target.Slug = Source.Slug
WHEN MATCHED THEN
    UPDATE SET
        Target.[Text] = Source.[Text],
        Target.IpaUk = Source.IpaUk,
        Target.IpaUs = Source.IpaUs,
        Target.ThaiReading = Source.ThaiReading,
        Target.MeaningTh = Source.MeaningTh,
        Target.MeaningEn = Source.MeaningEn,
        Target.PartOfSpeech = Source.PartOfSpeech,
        Target.CefrLevel = Source.CefrLevel,
        Target.ExampleEn = Source.ExampleEn,
        Target.ExampleTh = Source.ExampleTh,
        Target.CategoryId = @CategoryId,
        Target.IsActive = 1,
        Target.CreatedAt = Source.CreatedAt,
        Target.UpdatedAt = Source.UpdatedAt
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, [Text], Slug, IpaUk, IpaUs, ThaiReading, MeaningTh, MeaningEn, PartOfSpeech, CefrLevel, ExampleEn, ExampleTh, CategoryId, ImageMediaId, AudioMediaId, IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), Source.[Text], Source.Slug, Source.IpaUk, Source.IpaUs, Source.ThaiReading, Source.MeaningTh, Source.MeaningEn, Source.PartOfSpeech, Source.CefrLevel, Source.ExampleEn, Source.ExampleTh, @CategoryId, NULL, NULL, 1, Source.CreatedAt, Source.UpdatedAt);

SELECT COUNT_BIG(*) FROM dbo.Words WHERE IsActive = 1;
"@
    $activeCount = $command.ExecuteScalar()
    Write-Host "Done. Active words: $activeCount"
}
finally {
    $connection.Close()
}
