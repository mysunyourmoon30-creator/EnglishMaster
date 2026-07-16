using EnglishMaster.Application.Features.LessonSections.Commands;
using EnglishMaster.Application.Features.Lessons.Commands;
using EnglishMaster.Application.Features.Lessons.Queries;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;

namespace EnglishMaster.UnitTests.Lessons;

public sealed class LessonUseCaseTests
{
    [Fact]
    public async Task CreateLessonCreatesLessonWhenInputIsValid()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lessons = new FakeLessonRepository();
        var handler = new CreateLessonCommandHandler(
            lessons,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeWordRepository(),
            new FakeGrammarRuleRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateLessonCommand("Daily Routines", "Learn to talk about your day", null, "A1", null, null, 15, 0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(lessons.Lessons);
        Assert.Equal("daily-routines", result.Value!.Slug);
    }

    [Fact]
    public async Task CreateLessonReturnsValidationErrorWhenTitleSlugAlreadyExists()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lessons = new FakeLessonRepository();
        lessons.Lessons.Add(CreateLesson("Daily Routines", now));
        var handler = new CreateLessonCommandHandler(
            lessons,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeWordRepository(),
            new FakeGrammarRuleRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateLessonCommand("Daily Routines", null, null, null, null, null, 0, 0),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "Title");
        Assert.Single(lessons.Lessons);
    }

    [Fact]
    public async Task AddWordToLessonAddsWordWhenWordIsActive()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lessons = new FakeLessonRepository();
        var lesson = CreateLesson("Daily Routines", now);
        lessons.Lessons.Add(lesson);
        var words = new FakeWordRepository();
        var word = CreateWord("wake up", now);
        words.Words.Add(word);
        var handler = new AddWordToLessonCommandHandler(
            lessons,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            words,
            new FakeGrammarRuleRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new AddWordToLessonCommand(lesson.Id, word.Id, 0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(lesson.Words);
        Assert.Contains(result.Value!.Words, item => item.Id == word.Id);
    }

    [Fact]
    public async Task AddWordToLessonReturnsValidationErrorWhenSortOrderIsNegative()
    {
        var handler = new AddWordToLessonCommandHandler(
            new FakeLessonRepository(),
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeWordRepository(),
            new FakeGrammarRuleRepository(),
            new FixedTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.HandleAsync(
            new AddWordToLessonCommand(Guid.NewGuid(), Guid.NewGuid(), -1),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "SortOrder");
    }

    [Fact]
    public async Task AddGrammarRuleToLessonAddsRuleWhenRuleIsActive()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lessons = new FakeLessonRepository();
        var lesson = CreateLesson("Daily Routines", now);
        lessons.Lessons.Add(lesson);
        var grammarRules = new FakeGrammarRuleRepository();
        var topic = GrammarTopic.Create("Present Simple", null, CefrLevel.A1, 0, now);
        var rule = GrammarRule.Create(topic.Id, "Positive Sentences", "Subject + base verb", null, null, null, null, null, 0, now);
        grammarRules.GrammarRules.Add(rule);
        var handler = new AddGrammarRuleToLessonCommandHandler(
            lessons,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeWordRepository(),
            grammarRules,
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new AddGrammarRuleToLessonCommand(lesson.Id, rule.Id, 0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(lesson.GrammarRules);
        Assert.Contains(result.Value!.GrammarRules, item => item.Id == rule.Id);
    }

    [Fact]
    public async Task AddGrammarRuleToLessonReturnsValidationErrorWhenSortOrderIsNegative()
    {
        var handler = new AddGrammarRuleToLessonCommandHandler(
            new FakeLessonRepository(),
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeWordRepository(),
            new FakeGrammarRuleRepository(),
            new FixedTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.HandleAsync(
            new AddGrammarRuleToLessonCommand(Guid.NewGuid(), Guid.NewGuid(), -1),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "SortOrder");
    }

    [Fact]
    public async Task SearchLessonsFiltersByCefr()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lessons = new FakeLessonRepository();
        lessons.Lessons.Add(CreateLesson("Daily Routines", now, CefrLevel.A1));
        lessons.Lessons.Add(CreateLesson("Conditionals In Practice", now, CefrLevel.B1));
        var handler = new SearchLessonsQueryHandler(
            lessons,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeWordRepository(),
            new FakeGrammarRuleRepository());

        var result = await handler.HandleAsync(
            new SearchLessonsQuery(null, "B1", null, null, true, 1, 20),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Conditionals In Practice", result.Value.Items.Single().Title);
    }

    [Fact]
    public async Task SearchLessonsFiltersByPublishedStatus()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lessons = new FakeLessonRepository();
        var draft = CreateLesson("Draft Lesson", now);
        var published = CreateLesson("Published Lesson", now);
        published.Publish(now);
        lessons.Lessons.Add(draft);
        lessons.Lessons.Add(published);
        var handler = new SearchLessonsQueryHandler(
            lessons,
            new FakeCategoryRepository(),
            new FakeMediaRepository(),
            new FakeWordRepository(),
            new FakeGrammarRuleRepository());

        var result = await handler.HandleAsync(
            new SearchLessonsQuery(null, null, null, true, true, 1, 20),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Published Lesson", result.Value.Items.Single().Title);
    }

    [Fact]
    public async Task AddLessonSectionCreatesSectionWhenLessonIsActive()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lessons = new FakeLessonRepository();
        var lesson = CreateLesson("Daily Routines", now);
        lessons.Lessons.Add(lesson);
        var sections = new FakeLessonSectionRepository();
        var handler = new AddLessonSectionCommandHandler(
            sections,
            lessons,
            new FakeMediaRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new AddLessonSectionCommand(lesson.Id, "Vocabulary Warm-Up", "## Words", "Vocabulary", null, 0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(sections.LessonSections);
        Assert.Equal(lesson.Id, result.Value!.LessonId);
    }

    [Fact]
    public async Task ReorderLessonSectionsRejectsDuplicateSectionIds()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lessons = new FakeLessonRepository();
        var lesson = CreateLesson("Daily Routines", now);
        lessons.Lessons.Add(lesson);
        var sections = new FakeLessonSectionRepository();
        var firstSection = LessonSection.Create(
            lesson.Id,
            "Vocabulary Warm-Up",
            null,
            SectionType.Vocabulary,
            null,
            0,
            now);
        var secondSection = LessonSection.Create(
            lesson.Id,
            "Grammar Focus",
            null,
            SectionType.Grammar,
            null,
            1,
            now);
        sections.LessonSections.Add(firstSection);
        sections.LessonSections.Add(secondSection);
        var handler = new ReorderLessonSectionsCommandHandler(
            sections,
            lessons,
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new ReorderLessonSectionsCommand(lesson.Id, [firstSection.Id, firstSection.Id, secondSection.Id]),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "OrderedSectionIds");
        Assert.Equal(0, sections.SaveChangesCount);
    }

    private static Lesson CreateLesson(string title, DateTimeOffset now, CefrLevel? cefrLevel = null)
    {
        return Lesson.Create(title, null, null, cefrLevel, null, null, 0, 0, now);
    }

    private static Word CreateWord(string text, DateTimeOffset now)
    {
        return Word.Create(
            text,
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            text,
            PartOfSpeech.Verb,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now);
    }
}
