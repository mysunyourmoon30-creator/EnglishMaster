using EnglishMaster.Application.Features.GrammarExamples.Commands;
using EnglishMaster.Application.Features.GrammarRules.Commands;
using EnglishMaster.Application.Features.GrammarRules.Queries;
using EnglishMaster.Application.Features.GrammarTopics.Commands;
using EnglishMaster.Application.Features.GrammarTopics.Queries;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;

namespace EnglishMaster.UnitTests.Grammar;

public sealed class GrammarUseCaseTests
{
    [Fact]
    public async Task CreateGrammarTopicCreatesTopicWhenInputIsValid()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var repository = new FakeGrammarTopicRepository();
        var handler = new CreateGrammarTopicCommandHandler(repository, new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateGrammarTopicCommand("Present Simple", "Daily routines", "A1", 0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(repository.GrammarTopics);
        Assert.Equal("present-simple", result.Value!.Slug);
    }

    [Fact]
    public async Task CreateGrammarRuleCreatesRuleWhenTopicIsActive()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var topics = new FakeGrammarTopicRepository();
        var topic = CreateTopic("Present Simple", CefrLevel.A1, now);
        topics.GrammarTopics.Add(topic);
        var rules = new FakeGrammarRuleRepository();
        var handler = new CreateGrammarRuleCommandHandler(
            rules,
            topics,
            new FakeWordRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateGrammarRuleCommand(
                topic.Id,
                "Positive Sentences",
                "Subject + base verb",
                null,
                null,
                "S + V1",
                null,
                null,
                0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(rules.GrammarRules);
        Assert.Equal(topic.Id, result.Value!.GrammarTopicId);
    }

    [Fact]
    public async Task AddRelatedWordAddsWordToGrammarRule()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var topics = new FakeGrammarTopicRepository();
        var topic = CreateTopic("Present Simple", CefrLevel.A1, now);
        topics.GrammarTopics.Add(topic);
        var rules = new FakeGrammarRuleRepository();
        var rule = CreateRule(topic.Id, "Positive Sentences", now);
        rules.GrammarRules.Add(rule);
        var words = new FakeWordRepository();
        var word = CreateWord("walk", now);
        words.Words.Add(word);
        var handler = new AddRelatedWordToGrammarRuleCommandHandler(
            rules,
            topics,
            words,
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new AddRelatedWordToGrammarRuleCommand(rule.Id, word.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(rule.RelatedWords);
        Assert.Contains(result.Value!.RelatedWords, relatedWord => relatedWord.Id == word.Id);
    }

    [Fact]
    public async Task AddGrammarExampleCreatesExampleWhenRuleIsActive()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var topicId = Guid.NewGuid();
        var rule = CreateRule(topicId, "Positive Sentences", now);
        var rules = new FakeGrammarRuleRepository();
        rules.GrammarRules.Add(rule);
        var examples = new FakeGrammarExampleRepository();
        var handler = new AddGrammarExampleCommandHandler(
            examples,
            rules,
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new AddGrammarExampleCommand(
                rule.Id,
                "She walks to school.",
                "เธอเดินไปโรงเรียน",
                "Add s after she.",
                true,
                0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(examples.GrammarExamples);
        Assert.Equal(rule.Id, result.Value!.GrammarRuleId);
    }

    [Fact]
    public async Task SearchGrammarTopicsFiltersByCefr()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var repository = new FakeGrammarTopicRepository();
        repository.GrammarTopics.Add(CreateTopic("Present Simple", CefrLevel.A1, now));
        repository.GrammarTopics.Add(CreateTopic("Conditionals", CefrLevel.B1, now));
        var handler = new SearchGrammarTopicsQueryHandler(repository);

        var result = await handler.HandleAsync(
            new SearchGrammarTopicsQuery(null, "B1", true, 1, 20),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Conditionals", result.Value.Items.Single().Title);
    }

    [Fact]
    public async Task SearchGrammarRulesFiltersByTopic()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var topics = new FakeGrammarTopicRepository();
        var present = CreateTopic("Present Simple", CefrLevel.A1, now);
        var past = CreateTopic("Past Simple", CefrLevel.A2, now);
        topics.GrammarTopics.AddRange([present, past]);
        var rules = new FakeGrammarRuleRepository();
        rules.GrammarRules.Add(CreateRule(present.Id, "Positive Sentences", now));
        rules.GrammarRules.Add(CreateRule(past.Id, "Past Actions", now));
        var handler = new SearchGrammarRulesQueryHandler(rules, topics, new FakeWordRepository());

        var result = await handler.HandleAsync(
            new SearchGrammarRulesQuery(null, present.Id, true, 1, 20),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Positive Sentences", result.Value.Items.Single().Title);
    }

    [Fact]
    public async Task CreateGrammarRuleReturnsValidationErrorWhenRuleTextIsMissing()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var topics = new FakeGrammarTopicRepository();
        var topic = CreateTopic("Present Simple", CefrLevel.A1, now);
        topics.GrammarTopics.Add(topic);
        var handler = new CreateGrammarRuleCommandHandler(
            new FakeGrammarRuleRepository(),
            topics,
            new FakeWordRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateGrammarRuleCommand(topic.Id, "Positive Sentences", " ", null, null, null, null, null, 0),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "ruleText");
    }

    [Fact]
    public async Task CreateGrammarRuleReturnsValidationErrorWhenTitleSlugAlreadyExists()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var topics = new FakeGrammarTopicRepository();
        var topic = CreateTopic("Present Simple", CefrLevel.A1, now);
        topics.GrammarTopics.Add(topic);
        var rules = new FakeGrammarRuleRepository();
        rules.GrammarRules.Add(CreateRule(topic.Id, "Positive Sentences", now));
        var handler = new CreateGrammarRuleCommandHandler(
            rules,
            topics,
            new FakeWordRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateGrammarRuleCommand(
                topic.Id,
                "Positive Sentences",
                "Subject + base verb",
                null,
                null,
                null,
                null,
                null,
                0),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "Title");
        Assert.Single(rules.GrammarRules);
    }

    [Fact]
    public async Task CreateGrammarTopicReturnsValidationErrorWhenTitleSlugAlreadyExists()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var repository = new FakeGrammarTopicRepository();
        repository.GrammarTopics.Add(CreateTopic("Present Simple", CefrLevel.A1, now));
        var handler = new CreateGrammarTopicCommandHandler(repository, new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateGrammarTopicCommand("Present Simple", "Another summary", "A2", 0),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "Title");
        Assert.Single(repository.GrammarTopics);
    }

    private static GrammarTopic CreateTopic(
        string title,
        CefrLevel cefrLevel,
        DateTimeOffset now)
    {
        return GrammarTopic.Create(title, null, cefrLevel, 0, now);
    }

    private static GrammarRule CreateRule(
        Guid topicId,
        string title,
        DateTimeOffset now)
    {
        return GrammarRule.Create(
            topicId,
            title,
            "Subject + base verb",
            null,
            null,
            null,
            null,
            null,
            0,
            now);
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
