using EnglishMaster.Application.Features.ContentQuality;
using EnglishMaster.Application.Features.ContentQuality.Dtos;
using EnglishMaster.Domain.ContentQuality;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.ContentQuality;

public sealed class ContentQualityService : IContentQualityService
{
    private readonly EnglishMasterDbContext dbContext;
    private readonly IContentQualityRuleProvider ruleProvider;
    private readonly IContentQualityRepository repository;
    private readonly TimeProvider timeProvider;

    public ContentQualityService(
        EnglishMasterDbContext dbContext,
        IContentQualityRuleProvider ruleProvider,
        IContentQualityRepository repository,
        TimeProvider timeProvider)
    {
        this.dbContext = dbContext;
        this.ruleProvider = ruleProvider;
        this.repository = repository;
        this.timeProvider = timeProvider;
    }

    public async Task<ContentQualityCheckDto?> RunAsync(string contentType, Guid contentId, string? checkedBy, CancellationToken cancellationToken)
    {
        var normalizedType = Normalize(contentType);
        var content = await LoadContentAsync(normalizedType, contentId, cancellationToken);
        if (content is null)
        {
            return null;
        }

        var findings = ruleProvider.Evaluate(normalizedType, content);
        var score = CalculateScore(findings);
        var now = timeProvider.GetUtcNow();
        var check = ContentQualityCheck.Create(normalizedType, contentId, checkedBy, score, now);
        foreach (var finding in findings)
        {
            if (!Enum.TryParse<ContentQualitySeverity>(finding.Severity, ignoreCase: true, out var severity))
            {
                severity = ContentQualitySeverity.Warning;
            }

            check.AddFinding(finding.RuleCode, severity, finding.Message, finding.FieldName, finding.Recommendation, now);
        }

        check.Complete(now);
        return await repository.AddCheckAsync(check, cancellationToken);
    }

    private async Task<object?> LoadContentAsync(string contentType, Guid id, CancellationToken cancellationToken) =>
        contentType switch
        {
            "word" => await dbContext.Words.SingleOrDefaultAsync(item => item.Id == id, cancellationToken),
            "pronunciation" => await dbContext.Pronunciations.Include(item => item.MinimalPairs).SingleOrDefaultAsync(item => item.Id == id, cancellationToken),
            "grammartopic" => await dbContext.GrammarTopics.Include(item => item.Rules).SingleOrDefaultAsync(item => item.Id == id, cancellationToken),
            "grammarrule" => await dbContext.GrammarRules.Include(item => item.Examples).SingleOrDefaultAsync(item => item.Id == id, cancellationToken),
            "grammarexample" => await dbContext.GrammarExamples.SingleOrDefaultAsync(item => item.Id == id, cancellationToken),
            "lesson" => await dbContext.Lessons.Include(item => item.Sections).Include(item => item.Words).Include(item => item.GrammarRules).SingleOrDefaultAsync(item => item.Id == id, cancellationToken),
            "course" => await dbContext.Courses.Include(item => item.Lessons).SingleOrDefaultAsync(item => item.Id == id, cancellationToken),
            "book" => await dbContext.Books.Include(item => item.Chapters).SingleOrDefaultAsync(item => item.Id == id, cancellationToken),
            "quiz" => await dbContext.Quizzes.Include(item => item.Questions).ThenInclude(question => question.Choices).SingleOrDefaultAsync(item => item.Id == id, cancellationToken),
            "publishing" => await dbContext.PublishJobs.SingleOrDefaultAsync(item => item.Id == id, cancellationToken),
            _ => null
        };

    private static string Normalize(string value) => value.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().ToLowerInvariant();

    private static int CalculateScore(IReadOnlyCollection<ContentQualityFindingCandidate> findings)
    {
        var penalty = findings.Sum(finding => finding.Severity switch
        {
            nameof(ContentQualitySeverity.Critical) => 40,
            nameof(ContentQualitySeverity.Error) => 25,
            nameof(ContentQualitySeverity.Warning) => 10,
            _ => 2
        });
        return Math.Clamp(100 - penalty, 0, 100);
    }
}
