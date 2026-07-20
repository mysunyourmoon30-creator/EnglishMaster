SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @Now datetimeoffset = SYSDATETIMEOFFSET();
DECLARE @VocabularyCategoryId uniqueidentifier;
DECLARE @LessonCategoryId uniqueidentifier;

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Slug = N'vocabulary')
BEGIN
    INSERT INTO dbo.Categories (Id, [Name], Slug, [Description], SortOrder, IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), N'Vocabulary', N'vocabulary', N'Page: Words, Search, Dictionary | Use: 2,500 most common English words ranked by frequency', 10, 1, @Now, @Now);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Slug = N'lessons')
BEGIN
    INSERT INTO dbo.Categories (Id, [Name], Slug, [Description], SortOrder, IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), N'Lessons', N'lessons', N'Page: Lessons, Study Plan | Use: lessons built from ranked common English words', 40, 1, @Now, @Now);
END;

UPDATE dbo.Categories
SET [Name] = N'Vocabulary',
    [Description] = N'Page: Words, Search, Dictionary | Use: 2,500 most common English words ranked by frequency',
    SortOrder = 10,
    IsActive = 1,
    UpdatedAt = @Now
WHERE Slug = N'vocabulary';

UPDATE dbo.Categories
SET [Name] = N'Lessons',
    [Description] = N'Page: Lessons, Study Plan | Use: lessons built from ranked common English words',
    SortOrder = 40,
    IsActive = 1,
    UpdatedAt = @Now
WHERE Slug = N'lessons';

SELECT @VocabularyCategoryId = Id FROM dbo.Categories WHERE Slug = N'vocabulary';
SELECT @LessonCategoryId = Id FROM dbo.Categories WHERE Slug = N'lessons';

UPDATE dbo.Categories
SET IsActive = 0, UpdatedAt = @Now
WHERE Slug = N'demo-vocabulary'
   OR Slug LIKE N'practice-category-%';

DECLARE @Tags TABLE
(
    [Name] nvarchar(120) NOT NULL,
    Slug nvarchar(140) NOT NULL,
    [Description] nvarchar(500) NOT NULL
);

INSERT INTO @Tags ([Name], Slug, [Description])
VALUES
(N'Common Rank 1-500', N'common-rank-001-500', N'Most common English words ranked 1-500.'),
(N'Common Rank 501-1000', N'common-rank-501-1000', N'Common English words ranked 501-1000.'),
(N'Common Rank 1001-1500', N'common-rank-1001-1500', N'Common English words ranked 1001-1500.'),
(N'Common Rank 1501-2000', N'common-rank-1501-2000', N'Common English words ranked 1501-2000.'),
(N'Common Rank 2001-2500', N'common-rank-2001-2500', N'Common English words ranked 2001-2500.'),
(N'A1', N'a1', N'CEFR A1 common words.'),
(N'A2', N'a2', N'CEFR A2 common words.'),
(N'B1', N'b1', N'CEFR B1 common words.'),
(N'B2', N'b2', N'CEFR B2 common words.');

MERGE dbo.Tags AS Target
USING @Tags AS Source
    ON Target.Slug = Source.Slug
WHEN MATCHED THEN
    UPDATE SET
        Target.[Name] = Source.[Name],
        Target.[Description] = Source.[Description],
        Target.IsActive = 1,
        Target.UpdatedAt = @Now
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, [Name], Slug, [Description], IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), Source.[Name], Source.Slug, Source.[Description], 1, @Now, @Now);

;WITH RankedWords AS
(
    SELECT
        Id,
        [Text],
        Slug,
        CefrLevel,
        ROW_NUMBER() OVER (ORDER BY CreatedAt ASC, [Text] ASC) AS RankNumber
    FROM dbo.Words
    WHERE IsActive = 1
      AND Slug NOT LIKE N'perf-word-%'
      AND Slug NOT LIKE N'word-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]-%'
),
WordTagTargets AS
(
    SELECT Word.Id AS WordId, Tag.Id AS TagId
    FROM RankedWords AS Word
    JOIN dbo.Tags AS Tag
        ON Tag.Slug =
            CASE
                WHEN Word.RankNumber BETWEEN 1 AND 500 THEN N'common-rank-001-500'
                WHEN Word.RankNumber BETWEEN 501 AND 1000 THEN N'common-rank-501-1000'
                WHEN Word.RankNumber BETWEEN 1001 AND 1500 THEN N'common-rank-1001-1500'
                WHEN Word.RankNumber BETWEEN 1501 AND 2000 THEN N'common-rank-1501-2000'
                ELSE N'common-rank-2001-2500'
            END
    WHERE Word.RankNumber <= 2500
    UNION ALL
    SELECT Word.Id AS WordId, Tag.Id AS TagId
    FROM RankedWords AS Word
    JOIN dbo.Tags AS Tag ON Tag.Slug = LOWER(Word.CefrLevel)
    WHERE Word.RankNumber <= 2500
)
INSERT INTO dbo.WordTags (WordId, TagId)
SELECT Target.WordId, Target.TagId
FROM WordTagTargets AS Target
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.WordTags AS Existing
    WHERE Existing.WordId = Target.WordId
      AND Existing.TagId = Target.TagId
);

DECLARE @LessonRows TABLE
(
    LessonNumber int NOT NULL,
    StartRank int NOT NULL,
    EndRank int NOT NULL,
    Title nvarchar(200) NOT NULL,
    Slug nvarchar(220) NOT NULL,
    CefrLevel nvarchar(10) NOT NULL
);

;WITH Numbers AS
(
    SELECT TOP (50)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS LessonNumber
    FROM sys.all_objects
)
INSERT INTO @LessonRows (LessonNumber, StartRank, EndRank, Title, Slug, CefrLevel)
SELECT
    LessonNumber,
    ((LessonNumber - 1) * 50) + 1,
    LessonNumber * 50,
    CONCAT(N'Common Words ', FORMAT(((LessonNumber - 1) * 50) + 1, '0000'), N'-', FORMAT(LessonNumber * 50, '0000')),
    CONCAT(N'common-words-', FORMAT(((LessonNumber - 1) * 50) + 1, '0000'), N'-', FORMAT(LessonNumber * 50, '0000')),
    CASE
        WHEN LessonNumber <= 14 THEN N'A1'
        WHEN LessonNumber <= 30 THEN N'A2'
        WHEN LessonNumber <= 44 THEN N'B1'
        ELSE N'B2'
    END
FROM Numbers;

MERGE dbo.Lessons AS Target
USING @LessonRows AS Source
    ON Target.Slug = Source.Slug
WHEN MATCHED THEN
    UPDATE SET
        Target.Title = Source.Title,
        Target.Summary = CONCAT(N'Learn common English words ranked ', Source.StartRank, N'-', Source.EndRank, N'.'),
        Target.[Description] = CONCAT(N'Frequency-based vocabulary lesson covering common English words ranked ', Source.StartRank, N'-', Source.EndRank, N'. Review meaning, pronunciation, and example usage.'),
        Target.CefrLevel = Source.CefrLevel,
        Target.CategoryId = @LessonCategoryId,
        Target.EstimatedMinutes = 15,
        Target.SortOrder = Source.LessonNumber * 10,
        Target.IsPublished = 1,
        Target.IsActive = 1,
        Target.UpdatedAt = @Now
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, Title, Slug, Summary, [Description], CefrLevel, CategoryId, ThumbnailMediaId, EstimatedMinutes, SortOrder, IsPublished, IsActive, CreatedAt, UpdatedAt)
    VALUES
    (
        NEWID(),
        Source.Title,
        Source.Slug,
        CONCAT(N'Learn common English words ranked ', Source.StartRank, N'-', Source.EndRank, N'.'),
        CONCAT(N'Frequency-based vocabulary lesson covering common English words ranked ', Source.StartRank, N'-', Source.EndRank, N'. Review meaning, pronunciation, and example usage.'),
        Source.CefrLevel,
        @LessonCategoryId,
        NULL,
        15,
        Source.LessonNumber * 10,
        1,
        1,
        @Now,
        @Now
    );

DELETE LessonWord
FROM dbo.LessonWords AS LessonWord
JOIN dbo.Lessons AS Lesson ON Lesson.Id = LessonWord.LessonId
WHERE Lesson.Slug LIKE N'common-words-[0-9][0-9][0-9][0-9]-%';

;WITH RankedWords AS
(
    SELECT
        Id,
        ROW_NUMBER() OVER (ORDER BY CreatedAt ASC, [Text] ASC) AS RankNumber
    FROM dbo.Words
    WHERE IsActive = 1
      AND Slug NOT LIKE N'perf-word-%'
      AND Slug NOT LIKE N'word-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]-%'
),
Targets AS
(
    SELECT
        Lesson.Id AS LessonId,
        Word.Id AS WordId,
        ((Word.RankNumber - LessonRows.StartRank) + 1) * 10 AS SortOrder
    FROM @LessonRows AS LessonRows
    JOIN dbo.Lessons AS Lesson ON Lesson.Slug = LessonRows.Slug
    JOIN RankedWords AS Word ON Word.RankNumber BETWEEN LessonRows.StartRank AND LessonRows.EndRank
)
INSERT INTO dbo.LessonWords (LessonId, WordId, SortOrder)
SELECT LessonId, WordId, SortOrder
FROM Targets;

DELETE SectionRow
FROM dbo.LessonSections AS SectionRow
JOIN dbo.Lessons AS Lesson ON Lesson.Id = SectionRow.LessonId
WHERE Lesson.Slug LIKE N'common-words-[0-9][0-9][0-9][0-9]-%';

INSERT INTO dbo.LessonSections (Id, LessonId, Title, ContentMarkdown, SectionType, MediaId, SortOrder, IsActive, CreatedAt, UpdatedAt)
SELECT
    NEWID(),
    Lesson.Id,
    N'Vocabulary Focus',
    CONCAT(N'Review words ranked ', LessonRows.StartRank, N'-', LessonRows.EndRank, N'. Read each word, check the meaning, then make one short sentence.'),
    N'Vocabulary',
    NULL,
    10,
    1,
    @Now,
    @Now
FROM @LessonRows AS LessonRows
JOIN dbo.Lessons AS Lesson ON Lesson.Slug = LessonRows.Slug;

COMMIT TRANSACTION;

SELECT COUNT_BIG(*) AS ActiveCategories FROM dbo.Categories WHERE IsActive = 1;
SELECT COUNT_BIG(*) AS ActiveTags FROM dbo.Tags WHERE IsActive = 1;
SELECT COUNT_BIG(*) AS ActivePublishedLessons FROM dbo.Lessons WHERE IsActive = 1 AND IsPublished = 1;
SELECT COUNT_BIG(*) AS LessonWordLinks FROM dbo.LessonWords;
SELECT TOP 20 [Name], Slug, IsActive FROM dbo.Categories ORDER BY SortOrder, [Name];
SELECT TOP 10 Title, Slug, CefrLevel, IsPublished, IsActive FROM dbo.Lessons WHERE Slug LIKE N'common-words-%' ORDER BY SortOrder;
