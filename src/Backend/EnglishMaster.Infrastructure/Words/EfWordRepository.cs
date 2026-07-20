using EnglishMaster.Application.Features.Words;
using EnglishMaster.Application.Features.Words.Dtos;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Words;

internal sealed class EfWordRepository : IWordRepository
{
    private const int MaximumSearchTotalCount = 10_000;

    private readonly EnglishMasterDbContext dbContext;

    public EfWordRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Word word, CancellationToken cancellationToken)
    {
        await dbContext.Words.AddAsync(word, cancellationToken);
    }

    public async Task<Word?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Words
            .Include(word => word.Tags)
            .FirstOrDefaultAsync(word => word.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Word>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Distinct().ToArray();
        return await dbContext.Words
            .AsNoTracking()
            .Include(word => word.Tags)
            .Where(word => normalizedIds.Contains(word.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedWordId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Words.AsNoTracking()
            .Where(word => word.Slug == slug);

        if (excludedWordId.HasValue)
        {
            query = query.Where(word => word.Id != excludedWordId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<WordSearchResult> SearchAsync(
        WordSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<Word> query = dbContext.Words.AsNoTracking()
            .Include(word => word.Tags);

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(word => word.IsActive == criteria.IsActive.Value);
        }

        if (criteria.PartOfSpeech.HasValue)
        {
            query = query.Where(word => word.PartOfSpeech == criteria.PartOfSpeech.Value);
        }

        if (criteria.CefrLevel.HasValue)
        {
            query = query.Where(word => word.CefrLevel == criteria.CefrLevel.Value);
        }

        if (criteria.CategoryId.HasValue)
        {
            query = query.Where(word => word.CategoryId == criteria.CategoryId.Value);
        }

        if (criteria.TagId.HasValue)
        {
            query = query.Where(word => word.Tags.Any(tag => tag.TagId == criteria.TagId.Value));
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            return await SearchWithTermAsync(
                query,
                criteria,
                dbContext.Database.IsSqlServer(),
                await HasWordFullTextIndexAsync(cancellationToken),
                cancellationToken);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new WordSearchResult([], totalCount);
        }

        var sortedQuery = ApplySorting(query, criteria);
        var items = await sortedQuery
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new WordSearchResult(items, totalCount);
    }

    private async Task<WordSearchResult> SearchWithTermAsync(
        IQueryable<Word> baseQuery,
        WordSearchCriteria criteria,
        bool useSqlServer,
        bool useSqlServerFullText,
        CancellationToken cancellationToken)
    {
        var searchTerm = criteria.SearchTerm!.Trim();
        if (searchTerm.Length >= 8)
        {
            var fastQuery = baseQuery.Where(word =>
                word.Text.StartsWith(searchTerm) ||
                word.Slug.StartsWith(searchTerm));
            var fastResult = await PageSearchAsync(fastQuery, criteria, cancellationToken);

            if (fastResult.TotalCount > 0)
            {
                return fastResult;
            }
        }
        else if (useSqlServer)
        {
            var prefixResult = await SearchPrefixSqlServerAsync(criteria, searchTerm, cancellationToken);
            if (prefixResult.TotalCount > 0)
            {
                return prefixResult;
            }
        }

        var fullTextCondition = BuildFullTextCondition(searchTerm);
        if (useSqlServerFullText && fullTextCondition is not null)
        {
            var fullTextResult = await SearchFullTextSqlServerAsync(
                criteria,
                fullTextCondition,
                cancellationToken);
            if (fullTextResult.TotalCount > 0)
            {
                return fullTextResult;
            }
        }

        var normalizedSearchTerm = searchTerm.ToLower();
        var containsQuery = baseQuery.Where(word =>
            word.Text.ToLower().Contains(normalizedSearchTerm) ||
            word.Slug.ToLower().Contains(normalizedSearchTerm) ||
            word.MeaningTh.ToLower().Contains(normalizedSearchTerm) ||
            word.MeaningEn.ToLower().Contains(normalizedSearchTerm));

        return await PageSearchAsync(containsQuery, criteria, cancellationToken);
    }

    private async Task<bool> HasWordFullTextIndexAsync(CancellationToken cancellationToken)
    {
        if (!dbContext.Database.IsSqlServer())
        {
            return false;
        }

        var count = await dbContext.Database
            .SqlQueryRaw<int>(
                """
                SELECT COUNT(1) AS [Value]
                FROM sys.fulltext_indexes
                WHERE [object_id] = OBJECT_ID(N'[dbo].[Words]')
                """)
            .SingleAsync(cancellationToken);

        return count > 0;
    }

    private async Task<WordSearchResult> SearchFullTextSqlServerAsync(
        WordSearchCriteria criteria,
        string fullTextCondition,
        CancellationToken cancellationToken)
    {
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new WordSearchResult([], 0);
        }

        var take = criteria.PageSize + 1;
        var topN = (int)Math.Min(MaximumSearchTotalCount, skip + take);
        var filters = new List<string>();
        var parameters = new List<SqlParameter>
        {
            new("@fullTextCondition", fullTextCondition)
        };

        if (criteria.IsActive.HasValue)
        {
            filters.Add("[w].[IsActive] = @isActive");
            parameters.Add(new SqlParameter("@isActive", criteria.IsActive.Value));
        }

        if (criteria.PartOfSpeech.HasValue)
        {
            filters.Add("[w].[PartOfSpeech] = @partOfSpeech");
            parameters.Add(new SqlParameter("@partOfSpeech", criteria.PartOfSpeech.Value.ToString()));
        }

        if (criteria.CefrLevel.HasValue)
        {
            filters.Add("[w].[CefrLevel] = @cefrLevel");
            parameters.Add(new SqlParameter("@cefrLevel", criteria.CefrLevel.Value.ToString()));
        }

        if (criteria.CategoryId.HasValue)
        {
            filters.Add("[w].[CategoryId] = @categoryId");
            parameters.Add(new SqlParameter("@categoryId", criteria.CategoryId.Value));
        }

        if (criteria.TagId.HasValue)
        {
            filters.Add(
                "EXISTS (SELECT 1 FROM [WordTags] AS [wt] WHERE [wt].[WordId] = [w].[Id] AND [wt].[TagId] = @tagId)");
            parameters.Add(new SqlParameter("@tagId", criteria.TagId.Value));
        }

        var whereSql = filters.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", filters)}";
        var tieBreakerDirection = criteria.SortDirection == WordSortDirection.Desc ? "DESC" : "ASC";
        var secondaryOrderSql = criteria.SortBy == WordSortBy.CreatedAt
            ? $"[w].[CreatedAt] {tieBreakerDirection}, [w].[Text] ASC"
            : $"[w].[Text] {tieBreakerDirection}, [w].[Id] ASC";

        var sql = $"""
            SELECT [w].*
            FROM [Words] AS [w]
            INNER JOIN (
                SELECT [matches].[KEY], MAX([matches].[RANK]) AS [RANK]
                FROM (
                    SELECT [KEY], [RANK] FROM CONTAINSTABLE([Words], [Text], @fullTextCondition, {topN})
                    UNION ALL
                    SELECT [KEY], [RANK] FROM CONTAINSTABLE([Words], [Slug], @fullTextCondition, {topN})
                    UNION ALL
                    SELECT [KEY], [RANK] FROM CONTAINSTABLE([Words], [MeaningTh], @fullTextCondition, {topN})
                    UNION ALL
                    SELECT [KEY], [RANK] FROM CONTAINSTABLE([Words], [MeaningEn], @fullTextCondition, {topN})
                ) AS [matches]
                GROUP BY [matches].[KEY]
            ) AS [ft] ON [w].[Id] = [ft].[KEY]
            {whereSql}
            ORDER BY [ft].[RANK] DESC, {secondaryOrderSql}
            OFFSET {(int)skip} ROWS FETCH NEXT {take} ROWS ONLY
            """;

        var results = await dbContext.Words
            .FromSqlRaw(sql, parameters.ToArray<object>())
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return ToSearchPage(results, skip, criteria.PageSize);
    }

    private async Task<WordSearchResult> SearchPrefixSqlServerAsync(
        WordSearchCriteria criteria,
        string searchTerm,
        CancellationToken cancellationToken)
    {
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new WordSearchResult([], 0);
        }

        var take = criteria.PageSize + 1;
        var filters = new List<string>
        {
            "([w].[Text] LIKE @prefix ESCAPE N'\\' OR [w].[Slug] LIKE @prefix ESCAPE N'\\')"
        };
        var parameters = new List<SqlParameter>
        {
            new("@prefix", $"{EscapeLikePattern(searchTerm)}%")
        };

        if (criteria.IsActive.HasValue)
        {
            filters.Add("[w].[IsActive] = @isActive");
            parameters.Add(new SqlParameter("@isActive", criteria.IsActive.Value));
        }

        if (criteria.PartOfSpeech.HasValue)
        {
            filters.Add("[w].[PartOfSpeech] = @partOfSpeech");
            parameters.Add(new SqlParameter("@partOfSpeech", criteria.PartOfSpeech.Value.ToString()));
        }

        if (criteria.CefrLevel.HasValue)
        {
            filters.Add("[w].[CefrLevel] = @cefrLevel");
            parameters.Add(new SqlParameter("@cefrLevel", criteria.CefrLevel.Value.ToString()));
        }

        if (criteria.CategoryId.HasValue)
        {
            filters.Add("[w].[CategoryId] = @categoryId");
            parameters.Add(new SqlParameter("@categoryId", criteria.CategoryId.Value));
        }

        if (criteria.TagId.HasValue)
        {
            filters.Add(
                "EXISTS (SELECT 1 FROM [WordTags] AS [wt] WHERE [wt].[WordId] = [w].[Id] AND [wt].[TagId] = @tagId)");
            parameters.Add(new SqlParameter("@tagId", criteria.TagId.Value));
        }

        var tieBreakerDirection = criteria.SortDirection == WordSortDirection.Desc ? "DESC" : "ASC";
        var orderSql = criteria.SortBy == WordSortBy.CreatedAt
            ? $"[w].[CreatedAt] {tieBreakerDirection}, [w].[Text] ASC"
            : $"[w].[Text] {tieBreakerDirection}, [w].[Id] ASC";

        var sql = $"""
            SELECT [w].*
            FROM [Words] AS [w]
            WHERE {string.Join(" AND ", filters)}
            ORDER BY {orderSql}
            OFFSET {(int)skip} ROWS FETCH NEXT {take} ROWS ONLY
            """;

        var results = await dbContext.Words
            .FromSqlRaw(sql, parameters.ToArray<object>())
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return ToSearchPage(results, skip, criteria.PageSize);
    }

    private static async Task<WordSearchResult> PageSearchAsync(
        IQueryable<Word> query,
        WordSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new WordSearchResult([], 0);
        }

        var results = await ApplySorting(query, criteria)
            .Skip((int)skip)
            .Take(criteria.PageSize + 1)
            .ToListAsync(cancellationToken);

        return ToSearchPage(results, skip, criteria.PageSize);
    }

    private static WordSearchResult ToSearchPage(
        List<Word> results,
        long skip,
        int pageSize)
    {
        var hasNextPage = results.Count > pageSize;
        var items = (hasNextPage ? results.Take(pageSize) : results).ToArray();
        var totalCount = skip + items.Length + (hasNextPage ? 1 : 0);

        return new WordSearchResult(items, totalCount > int.MaxValue ? int.MaxValue : (int)totalCount);
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal)
            .Replace("[", "[[]", StringComparison.Ordinal);
    }

    private static async Task<WordSearchResult> PageAsync(
        IQueryable<Word> query,
        WordSearchCriteria criteria,
        int totalCount,
        CancellationToken cancellationToken)
    {
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new WordSearchResult([], totalCount);
        }

        var items = await ApplySorting(query, criteria)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new WordSearchResult(items, totalCount);
    }

    private static string? BuildFullTextCondition(string searchTerm)
    {
        var terms = searchTerm
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(term => term.Trim('"'))
            .Where(term => term.Length > 0)
            .Select(term => term.Replace("\"", "\"\"", StringComparison.Ordinal))
            .Select(term => $"\"{term}*\"")
            .Take(5)
            .ToArray();

        return terms.Length == 0 ? null : string.Join(" AND ", terms);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Word> ApplySorting(
        IQueryable<Word> query,
        WordSearchCriteria criteria)
    {
        return (criteria.SortBy, criteria.SortDirection) switch
        {
            (WordSortBy.CreatedAt, WordSortDirection.Desc) => query
                .OrderByDescending(word => word.CreatedAt)
                .ThenBy(word => word.Text),
            (WordSortBy.CreatedAt, _) => query
                .OrderBy(word => word.CreatedAt)
                .ThenBy(word => word.Text),
            (WordSortBy.Text, WordSortDirection.Desc) => query
                .OrderByDescending(word => word.Text)
                .ThenBy(word => word.Id),
            _ => query
                .OrderBy(word => word.Text)
                .ThenBy(word => word.Id)
        };
    }
}
