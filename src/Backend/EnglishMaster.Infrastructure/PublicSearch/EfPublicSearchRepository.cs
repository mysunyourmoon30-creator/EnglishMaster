using EnglishMaster.Application.Features.PublicSearch;
using EnglishMaster.Application.Features.PublicSearch.Dtos;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.PublicSearch;

public sealed class EfPublicSearchRepository : IPublicSearchRepository
{
    private const int PerTypeLimit = 200;
    private readonly EnglishMasterDbContext dbContext;

    public EfPublicSearchRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<PublicSearchResponse> SearchAsync(PublicSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var results = new List<PublicSearchResultDto>();
        var query = criteria.Query?.Trim();
        var cefrFilter = Enum.TryParse<CefrLevel>(criteria.CefrLevel, ignoreCase: true, out var parsedCefr)
            ? parsedCefr
            : (CefrLevel?)null;
        var categoryNames = await dbContext.Categories.AsNoTracking().ToDictionaryAsync(category => category.Id, category => category.Name, cancellationToken);
        var tagNames = await dbContext.Tags.AsNoTracking().ToDictionaryAsync(tag => tag.Id, tag => tag.Name, cancellationToken);

        if (MatchesType(criteria.ContentType, "word"))
        {
            var wordRows = await dbContext.Words.AsNoTracking()
                .Where(word => word.IsActive)
                .Where(word => criteria.CategoryId == null || word.CategoryId == criteria.CategoryId)
                .Where(word => cefrFilter == null || word.CefrLevel == cefrFilter)
                .Where(word => query == null || word.Text.Contains(query) || word.MeaningTh.Contains(query) || word.MeaningEn.Contains(query))
                .OrderByDescending(word => word.UpdatedAt)
                .Take(PerTypeLimit)
                .Select(word => new
                {
                    word.Id,
                    word.Text,
                    word.Slug,
                    word.MeaningTh,
                    word.MeaningEn,
                    word.CefrLevel,
                    word.CategoryId,
                    word.UpdatedAt
                })
                .ToArrayAsync(cancellationToken);
            var wordIds = wordRows.Select(word => word.Id).ToArray();
            var tagsByWord = await dbContext.WordTags.AsNoTracking()
                .Where(wordTag => wordIds.Contains(wordTag.WordId))
                .ToArrayAsync(cancellationToken);

            foreach (var word in wordRows)
            {
                var wordTagNames = tagsByWord.Where(wordTag => wordTag.WordId == word.Id && tagNames.ContainsKey(wordTag.TagId)).Select(wordTag => tagNames[wordTag.TagId]).ToArray();
                if (criteria.TagId.HasValue && !tagsByWord.Any(wordTag => wordTag.WordId == word.Id && wordTag.TagId == criteria.TagId))
                {
                    continue;
                }

                results.Add(new("word", word.Text, word.Slug, FirstNonEmpty(word.MeaningTh, word.MeaningEn), word.CefrLevel.ToString(), CategoryName(categoryNames, word.CategoryId), wordTagNames, $"/dictionary/{word.Slug}", Highlight(query, word.Text, word.MeaningTh), word.UpdatedAt));
            }
        }

        if (MatchesType(criteria.ContentType, "grammar"))
        {
            var topics = await dbContext.GrammarTopics.AsNoTracking()
                .Where(topic => topic.IsActive)
                .Where(topic => cefrFilter == null || topic.CefrLevel == cefrFilter)
                .Where(topic => query == null || topic.Title.Contains(query) || topic.Summary.Contains(query))
                .OrderByDescending(topic => topic.UpdatedAt)
                .Take(PerTypeLimit)
                .Select(topic => new { topic.Title, topic.Slug, topic.Summary, topic.CefrLevel, topic.UpdatedAt })
                .ToArrayAsync(cancellationToken);
            results.AddRange(topics.Select(topic => new PublicSearchResultDto("grammar", topic.Title, topic.Slug, topic.Summary, topic.CefrLevel.ToString(), null, [], $"/grammar/{topic.Slug}", Highlight(query, topic.Title, topic.Summary), topic.UpdatedAt)));

            var rules = await dbContext.GrammarRules.AsNoTracking()
                .Where(rule => rule.IsActive)
                .Where(rule => query == null || rule.Title.Contains(query) || rule.RuleText.Contains(query))
                .OrderByDescending(rule => rule.UpdatedAt)
                .Take(PerTypeLimit)
                .Select(rule => new { rule.Title, rule.Slug, rule.RuleText, rule.UpdatedAt })
                .ToArrayAsync(cancellationToken);
            results.AddRange(rules.Select(rule => new PublicSearchResultDto("grammar", rule.Title, rule.Slug, rule.RuleText, null, null, [], $"/grammar/{rule.Slug}", Highlight(query, rule.Title, rule.RuleText), rule.UpdatedAt)));
        }

        if (MatchesType(criteria.ContentType, "lesson"))
        {
            var lessons = await dbContext.Lessons.AsNoTracking()
                .Where(lesson => lesson.IsActive && lesson.IsPublished)
                .Where(lesson => criteria.CategoryId == null || lesson.CategoryId == criteria.CategoryId)
                .Where(lesson => cefrFilter == null || lesson.CefrLevel == cefrFilter)
                .Where(lesson => query == null || lesson.Title.Contains(query) || lesson.Summary.Contains(query))
                .OrderByDescending(lesson => lesson.UpdatedAt)
                .Take(PerTypeLimit)
                .Select(lesson => new { lesson.Title, lesson.Slug, lesson.Summary, lesson.CefrLevel, lesson.CategoryId, lesson.UpdatedAt })
                .ToArrayAsync(cancellationToken);
            results.AddRange(lessons.Select(lesson => new PublicSearchResultDto("lesson", lesson.Title, lesson.Slug, lesson.Summary, lesson.CefrLevel?.ToString(), CategoryName(categoryNames, lesson.CategoryId), [], $"/lessons/{lesson.Slug}", Highlight(query, lesson.Title, lesson.Summary), lesson.UpdatedAt)));
        }

        if (MatchesType(criteria.ContentType, "course"))
        {
            var courses = await dbContext.Courses.AsNoTracking()
                .Where(course => course.IsActive && course.IsPublished)
                .Where(course => criteria.CategoryId == null || course.CategoryId == criteria.CategoryId)
                .Where(course => cefrFilter == null || course.CefrLevel == cefrFilter)
                .Where(course => query == null || course.Title.Contains(query) || course.Summary.Contains(query))
                .OrderByDescending(course => course.UpdatedAt)
                .Take(PerTypeLimit)
                .Select(course => new { course.Title, course.Slug, course.Summary, course.CefrLevel, course.CategoryId, course.UpdatedAt })
                .ToArrayAsync(cancellationToken);
            results.AddRange(courses.Select(course => new PublicSearchResultDto("course", course.Title, course.Slug, course.Summary, course.CefrLevel?.ToString(), CategoryName(categoryNames, course.CategoryId), [], $"/courses/{course.Slug}", Highlight(query, course.Title, course.Summary), course.UpdatedAt)));
        }

        if (MatchesType(criteria.ContentType, "book"))
        {
            var books = await dbContext.Books.AsNoTracking()
                .Where(book => book.IsActive && book.IsPublished)
                .Where(book => criteria.CategoryId == null || book.CategoryId == criteria.CategoryId)
                .Where(book => cefrFilter == null || book.CefrLevel == cefrFilter)
                .Where(book => query == null || book.Title.Contains(query) || book.Summary.Contains(query))
                .OrderByDescending(book => book.UpdatedAt)
                .Take(PerTypeLimit)
                .Select(book => new { book.Title, book.Slug, book.Summary, book.CefrLevel, book.CategoryId, book.UpdatedAt })
                .ToArrayAsync(cancellationToken);
            results.AddRange(books.Select(book => new PublicSearchResultDto("book", book.Title, book.Slug, book.Summary, book.CefrLevel?.ToString(), CategoryName(categoryNames, book.CategoryId), [], $"/books/{book.Slug}", Highlight(query, book.Title, book.Summary), book.UpdatedAt)));
        }

        if (MatchesType(criteria.ContentType, "quiz"))
        {
            var quizzes = await dbContext.Quizzes.AsNoTracking()
                .Where(quiz => quiz.IsActive && quiz.IsPublished)
                .Where(quiz => criteria.CategoryId == null || quiz.CategoryId == criteria.CategoryId)
                .Where(quiz => cefrFilter == null || quiz.CefrLevel == cefrFilter)
                .Where(quiz => query == null || quiz.Title.Contains(query) || quiz.Summary.Contains(query))
                .OrderByDescending(quiz => quiz.UpdatedAt)
                .Take(PerTypeLimit)
                .Select(quiz => new { quiz.Title, quiz.Slug, quiz.Summary, quiz.CefrLevel, quiz.CategoryId, quiz.UpdatedAt })
                .ToArrayAsync(cancellationToken);
            results.AddRange(quizzes.Select(quiz => new PublicSearchResultDto("quiz", quiz.Title, quiz.Slug, quiz.Summary, quiz.CefrLevel?.ToString(), CategoryName(categoryNames, quiz.CategoryId), [], $"/quizzes/{quiz.Slug}", Highlight(query, quiz.Title, quiz.Summary), quiz.UpdatedAt)));
        }

        var sorted = Sort(results, criteria.SortBy, criteria.SortDirection).ToArray();
        var totalCount = sorted.Length;
        var totalPages = (int)Math.Ceiling(totalCount / (double)criteria.PageSize);
        var pageItems = sorted.Skip((criteria.PageNumber - 1) * criteria.PageSize).Take(criteria.PageSize).ToArray();
        return new PublicSearchResponse(pageItems, criteria.PageNumber, criteria.PageSize, totalCount, totalPages, criteria.PageNumber > 1, criteria.PageNumber < totalPages);
    }

    public async Task<PublicSearchFiltersResponse> GetFiltersAsync(CancellationToken cancellationToken)
    {
        var categories = await dbContext.Categories.AsNoTracking().Where(category => category.IsActive).OrderBy(category => category.Name).Select(category => new PublicSearchFilterDto(category.Id.ToString(), category.Name)).ToArrayAsync(cancellationToken);
        var tags = await dbContext.Tags.AsNoTracking().Where(tag => tag.IsActive).OrderBy(tag => tag.Name).Select(tag => new PublicSearchFilterDto(tag.Id.ToString(), tag.Name)).ToArrayAsync(cancellationToken);
        return new PublicSearchFiltersResponse(
            [new("word", "Words"), new("grammar", "Grammar"), new("lesson", "Lessons"), new("course", "Courses"), new("book", "Books"), new("quiz", "Quizzes")],
            [new("A1", "A1"), new("A2", "A2"), new("B1", "B1"), new("B2", "B2"), new("C1", "C1"), new("C2", "C2")],
            categories,
            tags);
    }

    public async Task<IReadOnlyCollection<string>> GetSuggestionsAsync(string? query, int count, CancellationToken cancellationToken)
    {
        var term = query?.Trim();
        return await dbContext.Words.AsNoTracking()
            .Where(word => word.IsActive)
            .Where(word => term == null || word.Text.Contains(term))
            .OrderBy(word => word.Text)
            .Take(count)
            .Select(word => word.Text)
            .ToArrayAsync(cancellationToken);
    }

    private static bool MatchesType(string? requestedType, string contentType) =>
        requestedType is null || requestedType == contentType;

    private static string? CategoryName(IReadOnlyDictionary<Guid, string> categories, Guid? categoryId) =>
        categoryId.HasValue && categories.TryGetValue(categoryId.Value, out var name) ? name : null;

    private static string FirstNonEmpty(params string[] values) =>
        values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;

    private static string Highlight(string? query, string title, string summary) =>
        string.IsNullOrWhiteSpace(query) ? FirstNonEmpty(summary, title) : FirstNonEmpty(summary, title);

    private static IEnumerable<PublicSearchResultDto> Sort(IEnumerable<PublicSearchResultDto> results, string? sortBy, string? sortDirection)
    {
        var descending = !string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);
        return string.Equals(sortBy, "title", StringComparison.OrdinalIgnoreCase)
            ? descending ? results.OrderByDescending(result => result.Title) : results.OrderBy(result => result.Title)
            : descending ? results.OrderByDescending(result => result.UpdatedAt).ThenBy(result => result.Title) : results.OrderBy(result => result.UpdatedAt).ThenBy(result => result.Title);
    }
}
