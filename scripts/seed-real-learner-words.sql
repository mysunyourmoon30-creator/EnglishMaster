SET NOCOUNT ON;
SET XACT_ABORT ON;

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

DECLARE @Words TABLE
(
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
    ExampleTh nvarchar(1000) NOT NULL
);

INSERT INTO @Words ([Text], Slug, IpaUk, IpaUs, ThaiReading, MeaningTh, MeaningEn, PartOfSpeech, CefrLevel, ExampleEn, ExampleTh)
VALUES
(N'apple', N'apple', N'AP-ul', N'AP-ul', N'ap-ple', N'apple; a fruit', N'A round fruit that is often red or green.', N'Noun', N'A1', N'I eat an apple every morning.', N'I eat an apple every morning.'),
(N'banana', N'banana', N'buh-NAN-uh', N'buh-NAN-uh', N'ba-na-na', N'banana; a yellow fruit', N'A long yellow fruit.', N'Noun', N'A1', N'She has a banana for breakfast.', N'She has a banana for breakfast.'),
(N'water', N'water', N'WAW-tuh', N'WAH-ter', N'wa-ter', N'water; drinking liquid', N'A clear liquid that people drink.', N'Noun', N'A1', N'Please drink some water.', N'Please drink some water.'),
(N'food', N'food', N'food', N'food', N'food', N'food; something we eat', N'Things that people or animals eat.', N'Noun', N'A1', N'This food is delicious.', N'This food is delicious.'),
(N'house', N'house', N'hows', N'hows', N'house', N'house; home building', N'A building where people live.', N'Noun', N'A1', N'My house is near the school.', N'My house is near the school.'),
(N'room', N'room', N'room', N'room', N'room', N'room; part of a building', N'A part of a house or building.', N'Noun', N'A1', N'This room is clean.', N'This room is clean.'),
(N'school', N'school', N'skool', N'skool', N'school', N'school; place to learn', N'A place where students learn.', N'Noun', N'A1', N'The children go to school.', N'The children go to school.'),
(N'teacher', N'teacher', N'TEE-chuh', N'TEE-cher', N'tea-cher', N'teacher; person who teaches', N'A person who helps students learn.', N'Noun', N'A1', N'Our teacher speaks slowly.', N'Our teacher speaks slowly.'),
(N'student', N'student', N'STYOO-dunt', N'STOO-dent', N'stu-dent', N'student; learner', N'A person who studies.', N'Noun', N'A1', N'Every student needs practice.', N'Every student needs practice.'),
(N'friend', N'friend', N'frend', N'frend', N'friend', N'friend; person you like', N'A person you know and like.', N'Noun', N'A1', N'My friend studies English with me.', N'My friend studies English with me.'),
(N'family', N'family', N'FAM-uh-lee', N'FAM-uh-lee', N'fam-i-ly', N'family; relatives', N'People related to you, such as parents and children.', N'Noun', N'A1', N'My family eats dinner together.', N'My family eats dinner together.'),
(N'mother', N'mother', N'MUTH-uh', N'MUTH-er', N'moth-er', N'mother; female parent', N'A female parent.', N'Noun', N'A1', N'My mother likes tea.', N'My mother likes tea.'),
(N'father', N'father', N'FAH-thuh', N'FAH-ther', N'fa-ther', N'father; male parent', N'A male parent.', N'Noun', N'A1', N'His father works at home.', N'His father works at home.'),
(N'morning', N'morning', N'MAW-ning', N'MOR-ning', N'mor-ning', N'morning; early part of day', N'The first part of the day.', N'Noun', N'A1', N'I study in the morning.', N'I study in the morning.'),
(N'night', N'night', N'nyt', N'nyt', N'night', N'night; dark time', N'The dark part of the day.', N'Noun', N'A1', N'We sleep at night.', N'We sleep at night.'),
(N'today', N'today', N'tuh-DAY', N'tuh-DAY', N'to-day', N'today; this day', N'This day.', N'Adverb', N'A1', N'Today I will learn five words.', N'Today I will learn five words.'),
(N'tomorrow', N'tomorrow', N'tuh-MOR-oh', N'tuh-MAR-oh', N'to-mor-row', N'tomorrow; next day', N'The day after today.', N'Adverb', N'A1', N'Tomorrow we have a quiz.', N'Tomorrow we have a quiz.'),
(N'yesterday', N'yesterday', N'YES-tuh-day', N'YES-ter-day', N'yes-ter-day', N'yesterday; previous day', N'The day before today.', N'Adverb', N'A1', N'Yesterday I reviewed grammar.', N'Yesterday I reviewed grammar.'),
(N'read', N'read', N'reed', N'reed', N'read', N'read; look at words', N'To look at and understand written words.', N'Verb', N'A1', N'Please read the sentence.', N'Please read the sentence.'),
(N'write', N'write', N'ryt', N'ryt', N'write', N'write; make words', N'To make words on paper or a screen.', N'Verb', N'A1', N'Write your name here.', N'Write your name here.'),
(N'listen', N'listen', N'LIS-un', N'LIS-un', N'lis-ten', N'listen; hear with attention', N'To pay attention to sound.', N'Verb', N'A1', N'Listen to the word again.', N'Listen to the word again.'),
(N'repeat', N'repeat', N'ri-PEET', N'ri-PEET', N're-peat', N'repeat; say again', N'To say or do something again.', N'Verb', N'A1', N'Repeat after the teacher.', N'Repeat after the teacher.'),
(N'open', N'open', N'OH-pun', N'OH-pen', N'o-pen', N'open; not closed', N'To move something so it is not closed.', N'Verb', N'A1', N'Open your book.', N'Open your book.'),
(N'close', N'close', N'klohz', N'klohz', N'close', N'close; shut', N'To shut something.', N'Verb', N'A1', N'Close the door, please.', N'Close the door, please.'),
(N'start', N'start', N'staht', N'start', N'start', N'start; begin', N'To begin doing something.', N'Verb', N'A1', N'Start the lesson now.', N'Start the lesson now.'),
(N'finish', N'finish', N'FIN-ish', N'FIN-ish', N'fin-ish', N'finish; complete', N'To complete something.', N'Verb', N'A1', N'Finish the practice today.', N'Finish the practice today.'),
(N'like', N'like', N'lyk', N'lyk', N'like', N'like; enjoy', N'To enjoy something or someone.', N'Verb', N'A1', N'I like English songs.', N'I like English songs.'),
(N'want', N'want', N'wont', N'wahnt', N'want', N'want; wish to have', N'To wish for something.', N'Verb', N'A1', N'I want to speak English.', N'I want to speak English.'),
(N'need', N'need', N'need', N'need', N'need', N'need; must have', N'To require something.', N'Verb', N'A1', N'You need more practice.', N'You need more practice.'),
(N'help', N'help', N'help', N'help', N'help', N'help; assist', N'To make it easier for someone to do something.', N'Verb', N'A1', N'Can you help me?', N'Can you help me?'),
(N'ask', N'ask', N'ahsk', N'ask', N'ask', N'ask; request information', N'To say a question.', N'Verb', N'A1', N'Ask one question in English.', N'Ask one question in English.'),
(N'answer', N'answer', N'AN-suh', N'AN-ser', N'an-swer', N'answer; reply', N'To reply to a question.', N'Verb', N'A1', N'Answer the question aloud.', N'Answer the question aloud.'),
(N'good', N'good', N'gud', N'gud', N'good', N'good; nice or correct', N'Pleasant, useful, or right.', N'Adjective', N'A1', N'That is a good answer.', N'That is a good answer.'),
(N'bad', N'bad', N'bad', N'bad', N'bad', N'bad; not good', N'Not good or not pleasant.', N'Adjective', N'A1', N'This is a bad habit.', N'This is a bad habit.'),
(N'new', N'new', N'nyoo', N'noo', N'new', N'new; not old', N'Recently made, found, or learned.', N'Adjective', N'A1', N'Learn one new word.', N'Learn one new word.'),
(N'old', N'old', N'ohld', N'ohld', N'old', N'old; not new', N'Having existed for a long time.', N'Adjective', N'A1', N'Review old words first.', N'Review old words first.'),
(N'easy', N'easy', N'EE-zee', N'EE-zee', N'ea-sy', N'easy; not difficult', N'Not difficult.', N'Adjective', N'A1', N'This question is easy.', N'This question is easy.'),
(N'difficult', N'difficult', N'DIF-uh-kult', N'DIF-uh-kult', N'dif-fi-cult', N'difficult; hard', N'Not easy.', N'Adjective', N'A2', N'This grammar rule is difficult.', N'This grammar rule is difficult.'),
(N'important', N'important', N'im-PAW-tunt', N'im-POR-tant', N'im-por-tant', N'important; valuable', N'Something that matters a lot.', N'Adjective', N'A2', N'Practice is important.', N'Practice is important.'),
(N'different', N'different', N'DIF-uh-runt', N'DIF-er-ent', N'dif-fer-ent', N'different; not the same', N'Not the same.', N'Adjective', N'A1', N'These two words are different.', N'These two words are different.'),
(N'because', N'because', N'bi-KOZ', N'bi-KAWZ', N'be-cause', N'because; for the reason that', N'Used to give a reason.', N'Conjunction', N'A2', N'I study because I want to improve.', N'I study because I want to improve.'),
(N'but', N'but', N'but', N'but', N'but', N'but; however', N'Used to connect contrasting ideas.', N'Conjunction', N'A1', N'The word is short but useful.', N'The word is short but useful.'),
(N'and', N'and', N'and', N'and', N'and', N'and; also', N'Used to join words or ideas.', N'Conjunction', N'A1', N'Read and repeat.', N'Read and repeat.'),
(N'before', N'before', N'bi-FAW', N'bi-FOR', N'be-fore', N'before; earlier than', N'Earlier than something.', N'Preposition', N'A2', N'Review before the quiz.', N'Review before the quiz.'),
(N'after', N'after', N'AF-tuh', N'AF-ter', N'af-ter', N'after; later than', N'Later than something.', N'Preposition', N'A2', N'Practice after lunch.', N'Practice after lunch.'),
(N'with', N'with', N'with', N'with', N'with', N'with; together', N'Together with someone or something.', N'Preposition', N'A1', N'Study with a friend.', N'Study with a friend.'),
(N'without', N'without', N'with-OWT', N'with-OWT', N'with-out', N'without; not having', N'Not having someone or something.', N'Preposition', N'A2', N'Try again without help.', N'Try again without help.'),
(N'about', N'about', N'uh-BOWT', N'uh-BOWT', N'a-bout', N'about; connected with', N'Connected with a topic.', N'Preposition', N'A1', N'This lesson is about food.', N'This lesson is about food.'),
(N'always', N'always', N'AWL-wayz', N'AWL-wayz', N'al-ways', N'always; every time', N'Every time.', N'Adverb', N'A1', N'Always check your answer.', N'Always check your answer.'),
(N'usually', N'usually', N'YOO-zhoo-uh-lee', N'YOO-zhoo-uh-lee', N'u-su-al-ly', N'usually; often', N'In most situations.', N'Adverb', N'A1', N'I usually study at night.', N'I usually study at night.'),
(N'sometimes', N'sometimes', N'SUM-tymz', N'SUM-tymz', N'some-times', N'sometimes; not always', N'On some occasions.', N'Adverb', N'A1', N'Sometimes I listen to podcasts.', N'Sometimes I listen to podcasts.'),
(N'never', N'never', N'NEV-uh', N'NEV-er', N'nev-er', N'never; not ever', N'Not at any time.', N'Adverb', N'A1', N'Never stop learning.', N'Never stop learning.'),
(N'quickly', N'quickly', N'KWIK-lee', N'KWIK-lee', N'quick-ly', N'quickly; fast', N'In a fast way.', N'Adverb', N'A2', N'Read the sentence quickly.', N'Read the sentence quickly.'),
(N'slowly', N'slowly', N'SLOH-lee', N'SLOH-lee', N'slow-ly', N'slowly; not fast', N'In a slow way.', N'Adverb', N'A2', N'Speak slowly and clearly.', N'Speak slowly and clearly.');

MERGE dbo.Words AS Target
USING @Words AS Source
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
        Target.UpdatedAt = @Now
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, [Text], Slug, IpaUk, IpaUs, ThaiReading, MeaningTh, MeaningEn, PartOfSpeech, CefrLevel, ExampleEn, ExampleTh, CategoryId, ImageMediaId, AudioMediaId, IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), Source.[Text], Source.Slug, Source.IpaUk, Source.IpaUs, Source.ThaiReading, Source.MeaningTh, Source.MeaningEn, Source.PartOfSpeech, Source.CefrLevel, Source.ExampleEn, Source.ExampleTh, @CategoryId, NULL, NULL, 1, @Now, @Now);

SELECT [Text], Slug, MeaningEn, CefrLevel, PartOfSpeech, IsActive
FROM dbo.Words
WHERE Slug IN (SELECT Slug FROM @Words)
ORDER BY [Text];
