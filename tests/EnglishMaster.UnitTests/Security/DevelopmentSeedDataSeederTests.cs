using EnglishMaster.Domain.Common;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using EnglishMaster.Infrastructure.Security;

using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.UnitTests.Security;

public sealed class DevelopmentSeedDataSeederTests
{
    [Fact]
    public async Task GrammarCurriculum_IsIdempotentAndRestoresExpectedWordLinks()
    {
        await using var dbContext = CreateDbContext();
        var now = DateTimeOffset.Parse("2026-07-28T00:00:00+00:00");
        var learn = CreateWord("learn", now);
        var daily = CreateWord("daily", now);
        var book = CreateWord("book", now);
        var unexpected = CreateWord("unexpected", now);
        dbContext.Words.AddRange(learn, daily, book, unexpected);
        await dbContext.SaveChangesAsync();
        var seeder = new DevelopmentSeedDataSeeder(dbContext, TimeProvider.System);

        var first = await seeder.SeedGrammarCurriculumAsync(now, CancellationToken.None);
        var firstTopicIds = await dbContext.GrammarTopics
            .AsNoTracking()
            .OrderBy(item => item.Slug)
            .Select(item => item.Id)
            .ToArrayAsync();
        first.HabitRule.RemoveRelatedWord(learn.Id, now.AddMinutes(1));
        first.HabitRule.AddRelatedWord(unexpected.Id, now.AddMinutes(1));
        await dbContext.SaveChangesAsync();

        await seeder.SeedGrammarCurriculumAsync(now.AddMinutes(2), CancellationToken.None);
        var secondTopicIds = await dbContext.GrammarTopics
            .AsNoTracking()
            .OrderBy(item => item.Slug)
            .Select(item => item.Id)
            .ToArrayAsync();
        var habitWordIds = await dbContext.GrammarRuleWords
            .AsNoTracking()
            .Where(item => item.GrammarRuleId == first.HabitRule.Id)
            .Select(item => item.WordId)
            .ToArrayAsync();

        Assert.Equal(13, await dbContext.GrammarTopics.CountAsync());
        Assert.Equal(13, await dbContext.GrammarRules.CountAsync());
        Assert.Equal(39, await dbContext.GrammarExamples.CountAsync());
        Assert.Equal(firstTopicIds, secondTopicIds);
        Assert.Contains(learn.Id, habitWordIds);
        Assert.Contains(daily.Id, habitWordIds);
        Assert.DoesNotContain(unexpected.Id, habitWordIds);
        Assert.Contains(book.Id, await dbContext.GrammarRuleWords
            .Where(item => item.GrammarRuleId == first.ArticleRule.Id)
            .Select(item => item.WordId)
            .ToArrayAsync());
    }

    [Fact]
    public async Task GrammarCurriculum_UsesStableIdsAcrossFreshDatabases()
    {
        var first = await SeedAndReadIdsAsync();
        var second = await SeedAndReadIdsAsync();

        Assert.Equal(first.TopicIds, second.TopicIds);
        Assert.Equal(first.RuleIds, second.RuleIds);
        Assert.Equal(first.ExampleIds, second.ExampleIds);
    }

    private static async Task<SeedIds> SeedAndReadIdsAsync()
    {
        await using var dbContext = CreateDbContext();
        var seeder = new DevelopmentSeedDataSeeder(dbContext, TimeProvider.System);
        await seeder.SeedGrammarCurriculumAsync(
            DateTimeOffset.Parse("2026-07-28T00:00:00+00:00"),
            CancellationToken.None);

        return new SeedIds(
            await dbContext.GrammarTopics.AsNoTracking().OrderBy(item => item.Slug).Select(item => item.Id).ToArrayAsync(),
            await dbContext.GrammarRules.AsNoTracking().OrderBy(item => item.Slug).Select(item => item.Id).ToArrayAsync(),
            await dbContext.GrammarExamples.AsNoTracking()
                .OrderBy(item => item.GrammarRuleId)
                .ThenBy(item => item.SortOrder)
                .Select(item => item.Id)
                .ToArrayAsync());
    }

    private static EnglishMasterDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<EnglishMasterDbContext>()
            .UseInMemoryDatabase($"EnglishMaster-Seed-{Guid.NewGuid():N}")
            .Options;
        return new EnglishMasterDbContext(options);
    }

    private static Word CreateWord(string text, DateTimeOffset now) =>
        Word.Create(
            text,
            string.Empty,
            string.Empty,
            string.Empty,
            "ความหมาย",
            "Meaning",
            PartOfSpeech.Verb,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now);

    private sealed record SeedIds(
        IReadOnlyCollection<Guid> TopicIds,
        IReadOnlyCollection<Guid> RuleIds,
        IReadOnlyCollection<Guid> ExampleIds);
}