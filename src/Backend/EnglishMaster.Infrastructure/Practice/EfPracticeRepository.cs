using EnglishMaster.Application.Features.Practice;
using EnglishMaster.Application.Features.Practice.Dtos;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.Practice;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Practice;

public sealed class EfPracticeRepository : IPracticeRepository
{
    private readonly EnglishMasterDbContext dbContext;
    private readonly TimeProvider timeProvider;

    public EfPracticeRepository(EnglishMasterDbContext dbContext, TimeProvider timeProvider)
    {
        this.dbContext = dbContext;
        this.timeProvider = timeProvider;
    }

    public async Task<PracticeItemDto> CreatePracticeItemAsync(Guid userId, string contentType, Guid contentId, string practiceType, CancellationToken cancellationToken)
    {
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var item = PracticeItem.Create(profile.Id, contentType, contentId, practiceType, now, now);
        dbContext.PracticeItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(item);
    }

    public async Task<int> GeneratePracticeItemsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        var existing = (await dbContext.PracticeItems.AsNoTracking()
            .Where(item => item.StudentProfileId == profile.Id)
            .Select(item => item.ContentType + "|" + item.ContentId + "|" + item.PracticeType)
            .ToArrayAsync(cancellationToken)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var now = timeProvider.GetUtcNow();
        var created = 0;

        var words = await dbContext.Words.AsNoTracking().Where(word => word.IsActive).OrderByDescending(word => word.UpdatedAt).Take(50).ToArrayAsync(cancellationToken);
        foreach (var word in words)
        {
            created += AddIfMissing(profile.Id, existing, "word", word.Id, "WordFlashcard", now);
            created += AddIfMissing(profile.Id, existing, "word", word.Id, "WordMeaning", now);
        }

        var grammarRules = await dbContext.GrammarRules.AsNoTracking().Where(rule => rule.IsActive).OrderByDescending(rule => rule.UpdatedAt).Take(30).ToArrayAsync(cancellationToken);
        foreach (var rule in grammarRules)
        {
            created += AddIfMissing(profile.Id, existing, "grammar-rule", rule.Id, "GrammarReview", now);
        }

        var minimalPairs = await dbContext.MinimalPairs.AsNoTracking().Where(pair => pair.IsActive).OrderByDescending(pair => pair.UpdatedAt).Take(20).ToArrayAsync(cancellationToken);
        foreach (var pair in minimalPairs)
        {
            created += AddIfMissing(profile.Id, existing, "minimal-pair", pair.Id, "MinimalPairPractice", now);
        }

        var lowQuizIds = await dbContext.QuizAttempts.AsNoTracking().Where(attempt => attempt.UserId == userId && !attempt.Passed).Select(attempt => attempt.QuizId).Distinct().ToArrayAsync(cancellationToken);
        var quizzes = await dbContext.Quizzes.AsNoTracking().Where(quiz => quiz.IsActive && quiz.IsPublished && lowQuizIds.Contains(quiz.Id)).Take(20).ToArrayAsync(cancellationToken);
        foreach (var quiz in quizzes)
        {
            created += AddIfMissing(profile.Id, existing, "quiz", quiz.Id, "QuizReview", now);
        }

        if (created == 0)
        {
            created += AddDemoPracticeItems(profile.Id, existing, now);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return created;
    }

    public async Task<IReadOnlyCollection<PracticeItemDto>> GetDuePracticeItemsAsync(Guid userId, int limit, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return [];
        }

        var now = timeProvider.GetUtcNow();
        var items = await dbContext.PracticeItems.AsNoTracking()
            .Where(item => item.StudentProfileId == profile.Id && item.Status != PracticeItemStatus.Suspended && item.NextReviewAt <= now)
            .OrderBy(item => item.NextReviewAt)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
        return items.Select(ToDto).ToArray();
    }

    public async Task<IReadOnlyCollection<DailyVocabularyItemDto>> GetDailyVocabularyAsync(Guid userId, int limit, CancellationToken cancellationToken)
    {
        var profileId = await dbContext.StudentProfiles.AsNoTracking()
            .Where(profile => profile.UserId == userId)
            .Select(profile => (Guid?)profile.Id)
            .SingleOrDefaultAsync(cancellationToken);
        if (!profileId.HasValue)
        {
            return [];
        }

        var now = timeProvider.GetUtcNow();
        var rows = await (
            from item in dbContext.PracticeItems.AsNoTracking()
            join word in dbContext.Words.AsNoTracking() on item.ContentId equals word.Id
            where item.StudentProfileId == profileId.Value &&
                item.ContentType == "word" &&
                item.Status != PracticeItemStatus.Suspended &&
                item.NextReviewAt <= now &&
                word.IsActive
            orderby item.NextReviewAt, item.Id
            select new
            {
                item.Id,
                item.PracticeType,
                item.Status,
                item.DueAt,
                item.NextReviewAt,
                WordId = word.Id,
                word.Text,
                word.IpaUk,
                word.IpaUs,
                word.ThaiReading,
                word.MeaningTh,
                word.MeaningEn,
                word.PartOfSpeech,
                word.CefrLevel,
                word.ExampleEn,
                word.ExampleTh
            })
            .Take(limit)
            .ToArrayAsync(cancellationToken);

        return rows.Select(row => new DailyVocabularyItemDto(
            row.Id,
            row.WordId,
            row.PracticeType,
            row.Status.ToString(),
            row.DueAt,
            row.NextReviewAt,
            row.Text,
            row.IpaUk,
            row.IpaUs,
            row.ThaiReading,
            row.MeaningTh,
            row.MeaningEn,
            row.PartOfSpeech.ToString(),
            row.CefrLevel.ToString(),
            row.ExampleEn,
            row.ExampleTh)).ToArray();
    }

    public async Task<PracticeSessionDto> StartPracticeSessionAsync(Guid userId, int limit, CancellationToken cancellationToken)
    {
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var dueItems = await dbContext.PracticeItems
            .Where(item => item.StudentProfileId == profile.Id && item.Status != PracticeItemStatus.Suspended && item.NextReviewAt <= now)
            .OrderBy(item => item.NextReviewAt)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
        if (dueItems.Length == 0)
        {
            await GeneratePracticeItemsAsync(userId, cancellationToken);
            dueItems = await dbContext.PracticeItems.Where(item => item.StudentProfileId == profile.Id && item.Status != PracticeItemStatus.Suspended && item.NextReviewAt <= now).OrderBy(item => item.NextReviewAt).Take(limit).ToArrayAsync(cancellationToken);
        }

        var session = PracticeSession.Create(profile.Id, now);
        var promptAnswers = await LoadPromptAnswersAsync(dueItems, cancellationToken);
        foreach (var item in dueItems)
        {
            var promptAnswer = promptAnswers.GetValueOrDefault(item.Id);
            session.AddItem(item, promptAnswer.Prompt, promptAnswer.Answer, now);
        }

        dbContext.PracticeSessions.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(session);
    }

    public async Task<PracticeSessionDto?> GetPracticeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var session = await dbContext.PracticeSessions.AsNoTracking().Include(session => session.Items).SingleOrDefaultAsync(session => session.Id == sessionId && session.StudentProfileId == profile.Id, cancellationToken);
        return session is null ? null : ToDto(session);
    }

    public async Task<PracticeSessionItemDto?> SubmitPracticeSessionItemAsync(Guid userId, Guid sessionItemId, string? userAnswer, string result, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null || !Enum.TryParse<PracticeResult>(result, ignoreCase: true, out var parsedResult))
        {
            return null;
        }

        var sessionItem = await dbContext.PracticeSessionItems.SingleOrDefaultAsync(item => item.Id == sessionItemId, cancellationToken);
        if (sessionItem is null)
        {
            return null;
        }

        var session = await dbContext.PracticeSessions.Include(session => session.Items).SingleOrDefaultAsync(session => session.Id == sessionItem.PracticeSessionId && session.StudentProfileId == profile.Id, cancellationToken);
        var practiceItem = await dbContext.PracticeItems.SingleOrDefaultAsync(item => item.Id == sessionItem.PracticeItemId && item.StudentProfileId == profile.Id, cancellationToken);
        if (session is null || practiceItem is null || session.Status != PracticeSessionStatus.Started || sessionItem.PracticedAt.HasValue)
        {
            return null;
        }

        var now = timeProvider.GetUtcNow();
        sessionItem.Submit(userAnswer, parsedResult, now);
        practiceItem.ApplyResult(parsedResult, now);
        session.Recount(now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(sessionItem);
    }

    public async Task<PracticeSessionDto?> CompletePracticeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var session = await dbContext.PracticeSessions.Include(session => session.Items).SingleOrDefaultAsync(session => session.Id == sessionId && session.StudentProfileId == profile.Id, cancellationToken);
        if (session is null || session.Status != PracticeSessionStatus.Started)
        {
            return null;
        }

        session.Complete(timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(session);
    }

    public async Task<IReadOnlyCollection<PracticeSessionDto>> GetPracticeHistoryAsync(Guid userId, int limit, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return [];
        }

        var sessions = await dbContext.PracticeSessions.AsNoTracking().Include(session => session.Items).Where(session => session.StudentProfileId == profile.Id).OrderByDescending(session => session.StartedAt).Take(limit).ToArrayAsync(cancellationToken);
        return sessions.Select(ToDto).ToArray();
    }

    public async Task<PracticeSummaryDto> GetPracticeSummaryAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return new PracticeSummaryDto(0, 0, 0, 0);
        }

        var now = timeProvider.GetUtcNow();
        var query = dbContext.PracticeItems.AsNoTracking().Where(item => item.StudentProfileId == profile.Id);
        return new PracticeSummaryDto(
            await query.CountAsync(item => item.Status != PracticeItemStatus.Suspended && item.NextReviewAt <= now, cancellationToken),
            await query.CountAsync(item => item.Status == PracticeItemStatus.New, cancellationToken),
            await query.CountAsync(item => item.Status == PracticeItemStatus.Reviewing || item.Status == PracticeItemStatus.Learning || item.Status == PracticeItemStatus.Due, cancellationToken),
            await query.CountAsync(item => item.Status == PracticeItemStatus.Mastered, cancellationToken));
    }

    public async Task<PracticeItemDto?> SuspendPracticeItemAsync(Guid userId, Guid practiceItemId, CancellationToken cancellationToken) =>
        await MutateItemAsync(userId, practiceItemId, item => item.Suspend(timeProvider.GetUtcNow()), cancellationToken);

    public async Task<PracticeItemDto?> ResumePracticeItemAsync(Guid userId, Guid practiceItemId, CancellationToken cancellationToken) =>
        await MutateItemAsync(userId, practiceItemId, item => item.Resume(timeProvider.GetUtcNow()), cancellationToken);

    private async Task<PracticeItemDto?> MutateItemAsync(Guid userId, Guid practiceItemId, Action<PracticeItem> action, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var item = await dbContext.PracticeItems.SingleOrDefaultAsync(item => item.Id == practiceItemId && item.StudentProfileId == profile.Id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        action(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(item);
    }

    private int AddIfMissing(Guid profileId, ISet<string> existing, string contentType, Guid contentId, string practiceType, DateTimeOffset now)
    {
        var key = contentType + "|" + contentId + "|" + practiceType;
        if (existing.Contains(key) ||
            dbContext.PracticeItems.Local.Any(item => item.StudentProfileId == profileId && item.ContentType == contentType && item.ContentId == contentId && item.PracticeType == practiceType))
        {
            return 0;
        }

        dbContext.PracticeItems.Add(PracticeItem.Create(profileId, contentType, contentId, practiceType, now, now));
        existing.Add(key);
        return 1;
    }

    private async Task<StudentProfile> GetOrCreateProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is not null)
        {
            return profile;
        }

        profile = StudentProfile.Create(userId, null, timeProvider.GetUtcNow());
        dbContext.StudentProfiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);
        return profile;
    }

    private async Task<StudentProfile?> GetProfileAsync(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.StudentProfiles.SingleOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);

    private async Task<IReadOnlyDictionary<Guid, (string Prompt, string Answer)>> LoadPromptAnswersAsync(IReadOnlyCollection<PracticeItem> items, CancellationToken cancellationToken)
    {
        var results = new Dictionary<Guid, (string Prompt, string Answer)>();
        var byContentType = items.GroupBy(item => item.ContentType).ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.OrdinalIgnoreCase);

        if (byContentType.TryGetValue("word", out var wordItems))
        {
            var wordIds = wordItems.Select(item => item.ContentId).Distinct().ToArray();
            var words = await dbContext.Words.AsNoTracking()
                .Where(word => wordIds.Contains(word.Id))
                .Select(word => new { word.Id, word.Text, word.MeaningTh })
                .ToDictionaryAsync(word => word.Id, cancellationToken);
            foreach (var item in wordItems)
            {
                results[item.Id] = words.TryGetValue(item.ContentId, out var word) ? (word.Text, word.MeaningTh ?? string.Empty) : ("Practice this word", string.Empty);
            }
        }

        if (byContentType.TryGetValue("grammar-rule", out var grammarItems))
        {
            var grammarIds = grammarItems.Select(item => item.ContentId).Distinct().ToArray();
            var rules = await dbContext.GrammarRules.AsNoTracking()
                .Where(rule => grammarIds.Contains(rule.Id))
                .Select(rule => new { rule.Id, rule.Title, rule.RuleText })
                .ToDictionaryAsync(rule => rule.Id, cancellationToken);
            foreach (var item in grammarItems)
            {
                results[item.Id] = rules.TryGetValue(item.ContentId, out var rule) ? (rule.Title, rule.RuleText ?? string.Empty) : ("Review this grammar rule", string.Empty);
            }
        }

        if (byContentType.TryGetValue("minimal-pair", out var minimalPairItems))
        {
            var minimalPairIds = minimalPairItems.Select(item => item.ContentId).Distinct().ToArray();
            var pairs = await dbContext.MinimalPairs.AsNoTracking()
                .Where(pair => minimalPairIds.Contains(pair.Id))
                .Select(pair => new { pair.Id, pair.PairWordText, pair.DifferenceNote })
                .ToDictionaryAsync(pair => pair.Id, cancellationToken);
            foreach (var item in minimalPairItems)
            {
                results[item.Id] = pairs.TryGetValue(item.ContentId, out var pair) ? (pair.PairWordText, pair.DifferenceNote ?? string.Empty) : ("Practice this minimal pair", string.Empty);
            }
        }

        if (byContentType.TryGetValue("quiz", out var quizItems))
        {
            var quizIds = quizItems.Select(item => item.ContentId).Distinct().ToArray();
            var quizzes = await dbContext.Quizzes.AsNoTracking()
                .Where(quiz => quizIds.Contains(quiz.Id))
                .Select(quiz => new { quiz.Id, quiz.Title })
                .ToDictionaryAsync(quiz => quiz.Id, cancellationToken);
            foreach (var item in quizItems)
            {
                results[item.Id] = quizzes.TryGetValue(item.ContentId, out var quiz) ? (quiz.Title, string.Empty) : ("Review this quiz", string.Empty);
            }
        }

        if (byContentType.TryGetValue("demo", out var demoItems))
        {
            foreach (var item in demoItems)
            {
                results[item.Id] = DemoPromptAnswers.GetValueOrDefault(
                    item.ContentId,
                    ("Say this sentence aloud: I am learning English today.", "I am learning English today."));
            }
        }

        foreach (var item in items)
        {
            results.TryAdd(item.Id, ("Practice item", string.Empty));
        }

        return results;
    }

    private static PracticeItemDto ToDto(PracticeItem item) =>
        new(item.Id, item.ContentType, item.ContentId, item.PracticeType, item.Status.ToString(), item.DueAt, item.LastPracticedAt, item.NextReviewAt, item.ReviewCount, item.CorrectCount, item.IncorrectCount, item.CurrentIntervalDays);

    private static PracticeSessionDto ToDto(PracticeSession session) =>
        new(session.Id, session.StartedAt, session.CompletedAt, session.Status.ToString(), session.TotalItems, session.CompletedItems, session.CorrectItems, session.IncorrectItems, session.Items.Select(ToDto).ToArray());

    private static PracticeSessionItemDto ToDto(PracticeSessionItem item) =>
        new(item.Id, item.PracticeSessionId, item.PracticeItemId, item.ContentType, item.ContentId, item.PracticeType, item.PromptText, item.AnswerText, item.UserAnswer, item.Result?.ToString(), item.IsCorrect, item.PracticedAt);

    private int AddDemoPracticeItems(Guid profileId, ISet<string> existing, DateTimeOffset now)
    {
        var created = 0;
        foreach (var item in DemoPromptAnswers)
        {
            created += AddIfMissing(profileId, existing, "demo", item.Key, "StarterPractice", now);
        }

        return created;
    }

    private static readonly IReadOnlyDictionary<Guid, (string Prompt, string Answer)> DemoPromptAnswers =
        new Dictionary<Guid, (string Prompt, string Answer)>
        {
            [Guid.Parse("9b24ec40-36d5-4a5e-bbeb-0dd6f4364f01")] = ("What does 'learn' mean in Thai?", "เรียนรู้"),
            [Guid.Parse("3be5172e-9e61-4a6f-af2e-6497673c0f72")] = ("Choose a simple sentence: I ___ English every day.", "study"),
            [Guid.Parse("356225e0-5f87-475c-87c4-3ec872901054")] = ("Say this aloud: Practice makes progress.", "Practice makes progress."),
            [Guid.Parse("175e4d76-c969-4a2e-8f1e-39fffb20fe1c")] = ("What is the opposite of 'easy'?", "hard"),
            [Guid.Parse("247a7a8e-5c9e-45d9-98cb-4b679f36e071")] = ("Translate: วันนี้ฉันจะฝึกภาษาอังกฤษ", "Today I will practice English.")
        };
}
