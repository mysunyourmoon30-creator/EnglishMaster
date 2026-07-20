SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @TargetCount int = 1000000;
DECLARE @BatchSize int = 50000;
DECLARE @Now datetimeoffset = SYSDATETIMEOFFSET();
DECLARE @CategoryId uniqueidentifier;
DECLARE @Start int = 1;
DECLARE @End int;
DECLARE @GeneratedWordsActive bit = 0;

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Slug = N'vocabulary')
BEGIN
    INSERT INTO dbo.Categories (Id, [Name], Slug, [Description], SortOrder, IsActive, CreatedAt, UpdatedAt)
    VALUES
    (
        NEWID(),
        N'Vocabulary',
        N'vocabulary',
        N'Page: Words, Search, Dictionary | Use: English vocabulary, meanings, examples, and learner word bank',
        10,
        1,
        @Now,
        @Now
    );
END;

SELECT @CategoryId = Id
FROM dbo.Categories
WHERE Slug = N'vocabulary';

PRINT N'Cleaning generated words from previous runs';

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

PRINT N'Deactivating old starter/demo words';

UPDATE dbo.Words
SET IsActive = 0, UpdatedAt = @Now
WHERE Slug IN (N'hello', N'book', N'learn', N'speak', N'daily')
   OR Slug LIKE N'practice-word-%';

DECLARE @Roots TABLE
(
    RootId int IDENTITY(1,1) PRIMARY KEY,
    RootText nvarchar(60) NOT NULL,
    ThaiReading nvarchar(200) NOT NULL,
    MeaningEn nvarchar(300) NOT NULL,
    ExampleEn nvarchar(300) NOT NULL,
    ExampleTh nvarchar(300) NOT NULL,
    PartOfSpeech nvarchar(50) NOT NULL,
    CefrLevel nvarchar(10) NOT NULL
);

INSERT INTO @Roots (RootText, ThaiReading, MeaningEn, ExampleEn, ExampleTh, PartOfSpeech, CefrLevel)
VALUES
(N'ability', N'khwam-samat', N'the skill to do something', N'I can improve my ability with daily practice.', N'I can improve my ability with daily practice.', N'Noun', N'B1'),
(N'accept', N'yom-rap', N'to agree to receive or allow something', N'Please accept this answer.', N'Please accept this answer.', N'Verb', N'A2'),
(N'action', N'kan-kratham', N'something that a person does', N'Small action creates progress.', N'Small action creates progress.', N'Noun', N'A2'),
(N'active', N'kratu-rue-ron', N'doing things with energy', N'Active learners practice every day.', N'Active learners practice every day.', N'Adjective', N'A2'),
(N'advice', N'kham-nae-nam', N'an opinion that helps someone decide', N'Good advice helps me learn faster.', N'Good advice helps me learn faster.', N'Noun', N'A2'),
(N'answer', N'kham-top', N'something you say or write to reply', N'Write your answer clearly.', N'Write your answer clearly.', N'Noun', N'A1'),
(N'arrive', N'ma-thueng', N'to reach a place', N'We arrive at school early.', N'We arrive at school early.', N'Verb', N'A1'),
(N'balance', N'khwam-som-dun', N'a steady condition where things are equal', N'Learning needs balance and rest.', N'Learning needs balance and rest.', N'Noun', N'B1'),
(N'basic', N'phuen-than', N'simple and important', N'This is a basic English lesson.', N'This is a basic English lesson.', N'Adjective', N'A1'),
(N'believe', N'chuea', N'to think something is true', N'I believe practice helps.', N'I believe practice helps.', N'Verb', N'A2'),
(N'careful', N'ra-mat-ra-wang', N'paying attention to avoid mistakes', N'Be careful with spelling.', N'Be careful with spelling.', N'Adjective', N'A2'),
(N'change', N'plian-plaeng', N'to become different', N'Change your plan when you need to.', N'Change your plan when you need to.', N'Verb', N'A1'),
(N'choose', N'lueak', N'to pick one thing from several things', N'Choose the best answer.', N'Choose the best answer.', N'Verb', N'A1'),
(N'clear', N'chat-jen', N'easy to understand', N'Give a clear example.', N'Give a clear example.', N'Adjective', N'A2'),
(N'common', N'thua-pai', N'happening often', N'This is a common mistake.', N'This is a common mistake.', N'Adjective', N'A2'),
(N'compare', N'priap-thiap', N'to look at two things and see differences', N'Compare these two sentences.', N'Compare these two sentences.', N'Verb', N'B1'),
(N'complete', N'tham-hai-set', N'to finish something', N'Complete today''s practice.', N'Complete today''s practice.', N'Verb', N'A2'),
(N'connect', N'chueam-to', N'to join things together', N'Connect the word with its meaning.', N'Connect the word with its meaning.', N'Verb', N'B1'),
(N'correct', N'thuk-tong', N'right or without mistakes', N'Choose the correct phrase.', N'Choose the correct phrase.', N'Adjective', N'A1'),
(N'daily', N'pra-jam-wan', N'happening every day', N'Daily practice helps you improve.', N'Daily practice helps you improve.', N'Adjective', N'A1'),
(N'describe', N'athibai', N'to say what something is like', N'Describe your morning routine.', N'Describe your morning routine.', N'Verb', N'A2'),
(N'detail', N'rai-la-iat', N'a small piece of information', N'Read the detail before answering.', N'Read the detail before answering.', N'Noun', N'B1'),
(N'develop', N'phatthana', N'to grow or improve', N'Develop your speaking skill.', N'Develop your speaking skill.', N'Verb', N'B1'),
(N'different', N'taek-tang', N'not the same', N'These words have different meanings.', N'These words have different meanings.', N'Adjective', N'A1'),
(N'easy', N'ngai', N'not difficult', N'This lesson is easy to start.', N'This lesson is easy to start.', N'Adjective', N'A1'),
(N'example', N'tua-yang', N'something that shows how a rule works', N'Read the example sentence.', N'Read the example sentence.', N'Noun', N'A1'),
(N'explain', N'athibai', N'to make something clear', N'Explain your answer in English.', N'Explain your answer in English.', N'Verb', N'A2'),
(N'focus', N'jot-jo', N'to give attention to one thing', N'Focus on one skill today.', N'Focus on one skill today.', N'Verb', N'B1'),
(N'grammar', N'waiyakon', N'rules for making sentences', N'Grammar helps us make clear sentences.', N'Grammar helps us make clear sentences.', N'Noun', N'A2'),
(N'habit', N'nisai', N'something you do often', N'A study habit makes learning easier.', N'A study habit makes learning easier.', N'Noun', N'A2'),
(N'improve', N'phatthana-hai-di-khuen', N'to become better', N'Practice to improve your English.', N'Practice to improve your English.', N'Verb', N'A2'),
(N'listen', N'fang', N'to pay attention to sound', N'Listen to the pronunciation.', N'Listen to the pronunciation.', N'Verb', N'A1'),
(N'meaning', N'khwam-mai', N'what a word or sentence expresses', N'Check the meaning before you speak.', N'Check the meaning before you speak.', N'Noun', N'A1'),
(N'practice', N'fuek-fon', N'to do something many times to improve', N'Practice speaking for ten minutes.', N'Practice speaking for ten minutes.', N'Verb', N'A1'),
(N'progress', N'khwam-kao-na', N'improvement over time', N'Your progress is visible this week.', N'Your progress is visible this week.', N'Noun', N'B1'),
(N'question', N'kham-tham', N'a sentence that asks for information', N'Read the question carefully.', N'Read the question carefully.', N'Noun', N'A1'),
(N'review', N'thop-thuan', N'to study something again', N'Review yesterday''s words.', N'Review yesterday''s words.', N'Verb', N'A2'),
(N'sentence', N'prayok', N'a group of words with complete meaning', N'Make one sentence with this word.', N'Make one sentence with this word.', N'Noun', N'A1'),
(N'speak', N'phut', N'to say words with your voice', N'Speak slowly and clearly.', N'Speak slowly and clearly.', N'Verb', N'A1'),
(N'study', N'rian', N'to learn about a subject', N'Study English every day.', N'Study English every day.', N'Verb', N'A1'),
(N'target', N'pao-mai', N'something you want to achieve', N'Set a small target for today.', N'Set a small target for today.', N'Noun', N'B1'),
(N'useful', N'mi-prayoj', N'helpful for a purpose', N'This phrase is useful in class.', N'This phrase is useful in class.', N'Adjective', N'A2'),
(N'vocabulary', N'kham-sap', N'words used in a language', N'Build your vocabulary step by step.', N'Build your vocabulary step by step.', N'Noun', N'A2'),
(N'write', N'khian', N'to make words on paper or screen', N'Write three example sentences.', N'Write three example sentences.', N'Verb', N'A1');

DECLARE @RootCount int = (SELECT COUNT(*) FROM @Roots);

WHILE @Start <= @TargetCount
BEGIN
    SET @End = IIF(@Start + @BatchSize - 1 > @TargetCount, @TargetCount, @Start + @BatchSize - 1);

    ;WITH
    E1(N) AS
    (
        SELECT 1 FROM (VALUES(0),(0),(0),(0),(0),(0),(0),(0),(0),(0)) D(N)
    ),
    E2(N) AS (SELECT 1 FROM E1 A CROSS JOIN E1 B),
    E4(N) AS (SELECT 1 FROM E2 A CROSS JOIN E2 B),
    E8(N) AS (SELECT 1 FROM E4 A CROSS JOIN E4 B),
    Numbers(N) AS
    (
        SELECT TOP (@End - @Start + 1)
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) + @Start - 1
        FROM E8
    )
    INSERT INTO dbo.Words WITH (TABLOCK)
    (
        Id,
        [Text],
        Slug,
        IpaUk,
        IpaUs,
        ThaiReading,
        MeaningTh,
        MeaningEn,
        PartOfSpeech,
        CefrLevel,
        ExampleEn,
        ExampleTh,
        CategoryId,
        ImageMediaId,
        AudioMediaId,
        IsActive,
        CreatedAt,
        UpdatedAt
    )
    SELECT
        NEWID(),
        CONCAT(N'perf ', Root.RootText, N' ', FORMAT(Number.N, '0000000')),
        CONCAT(N'perf-word-', FORMAT(Number.N, '0000000'), N'-', Root.RootText),
        CONCAT(N'/', Root.RootText, N'/'),
        CONCAT(N'/', Root.RootText, N'/'),
        Root.ThaiReading,
        CONCAT(Root.ThaiReading, N' - learning vocabulary item ', FORMAT(Number.N, 'N0')),
        CONCAT(Root.MeaningEn, N'. Learning vocabulary item #', FORMAT(Number.N, 'N0'), N' for search, paging, and practice performance.'),
        Root.PartOfSpeech,
        Root.CefrLevel,
        Root.ExampleEn,
        Root.ExampleTh,
        @CategoryId,
        NULL,
        NULL,
        @GeneratedWordsActive,
        @Now,
        @Now
    FROM Numbers AS Number
    JOIN @Roots AS Root
        ON Root.RootId = ((Number.N - 1) % @RootCount) + 1;

    PRINT CONCAT(N'Inserted words ', FORMAT(@Start, 'N0'), N' - ', FORMAT(@End, 'N0'));
    SET @Start = @End + 1;
END;

SELECT COUNT_BIG(*) AS ActiveWords
FROM dbo.Words
WHERE IsActive = 1;

SELECT TOP (20) [Text], Slug, MeaningEn, CefrLevel, PartOfSpeech
FROM dbo.Words
WHERE Slug LIKE N'word-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]-%'
   OR Slug LIKE N'perf-word-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]-%'
ORDER BY Slug;
