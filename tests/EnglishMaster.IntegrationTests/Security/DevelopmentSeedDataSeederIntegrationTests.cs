using EnglishMaster.Domain.Common;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using EnglishMaster.Infrastructure.Security;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.Security;

public sealed class DevelopmentSeedDataSeederIntegrationTests(EnglishMasterApiFactory factory)
    : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task GrammarCurriculum_RepeatedSeedIsIdempotentAndReconcilesRelatedWords()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        var now = DateTimeOffset.Parse("2026-07-28T00:00:00+00:00");
        var learn = CreateWord("learn", now);
        var daily = CreateWord("daily", now);
        var book = CreateWord("book", now);
        var unexpected = CreateWord("unexpected", now);
        dbContext.Words.AddRange(learn, daily, book, unexpected);
        await dbContext.SaveChangesAsync();
        var configuration = new ConfigurationBuilder()
            .AddConfiguration(factory.Services.GetRequiredService<IConfiguration>())
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SeedGrammarCurriculum:Enabled"] = "true"
            })
            .Build();

        await SecuritySeeder.SeedSecurityAsync(factory.Services, configuration);
        dbContext.ChangeTracker.Clear();
        var firstIds = await ReadGrammarIdsAsync(dbContext);
        var habitRule = await dbContext.GrammarRules
            .Include(item => item.RelatedWords)
            .SingleAsync(item => item.Slug == GrammarRule.GenerateSlug("Present simple for habits"));
        habitRule.RemoveRelatedWord(learn.Id, now.AddMinutes(1));
        habitRule.AddRelatedWord(unexpected.Id, now.AddMinutes(1));
        await dbContext.SaveChangesAsync();

        await SecuritySeeder.SeedSecurityAsync(factory.Services, configuration);
        dbContext.ChangeTracker.Clear();
        var secondIds = await ReadGrammarIdsAsync(dbContext);
        var relatedWordIds = await dbContext.GrammarRuleWords
            .AsNoTracking()
            .Where(item => item.GrammarRuleId == habitRule.Id)
            .Select(item => item.WordId)
            .ToArrayAsync();

        Assert.Equal(firstIds.TopicIds, secondIds.TopicIds);
        Assert.Equal(firstIds.RuleIds, secondIds.RuleIds);
        Assert.Equal(firstIds.ExampleIds, secondIds.ExampleIds);
        Assert.Equal(13, await dbContext.GrammarTopics.CountAsync());
        Assert.Equal(13, await dbContext.GrammarRules.CountAsync());
        Assert.Equal(39, await dbContext.GrammarExamples.CountAsync());
        Assert.Contains(learn.Id, relatedWordIds);
        Assert.Contains(daily.Id, relatedWordIds);
        Assert.DoesNotContain(unexpected.Id, relatedWordIds);
    }

    private static async Task<GrammarIds> ReadGrammarIdsAsync(EnglishMasterDbContext dbContext) =>
        new(
            await dbContext.GrammarTopics
                .AsNoTracking()
                .OrderBy(item => item.Slug)
                .Select(item => item.Id)
                .ToArrayAsync(),
            await dbContext.GrammarRules
                .AsNoTracking()
                .OrderBy(item => item.Slug)
                .Select(item => item.Id)
                .ToArrayAsync(),
            await dbContext.GrammarExamples
                .AsNoTracking()
                .OrderBy(item => item.GrammarRuleId)
                .ThenBy(item => item.SortOrder)
                .Select(item => item.Id)
                .ToArrayAsync());

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

    private sealed record GrammarIds(
        IReadOnlyCollection<Guid> TopicIds,
        IReadOnlyCollection<Guid> RuleIds,
        IReadOnlyCollection<Guid> ExampleIds);
}