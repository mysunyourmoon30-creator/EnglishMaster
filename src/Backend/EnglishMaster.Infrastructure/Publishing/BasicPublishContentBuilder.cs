using System.Net;
using EnglishMaster.Application.Features.Publishing;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Publishing;

internal sealed class BasicPublishContentBuilder : IPublishContentBuilder
{
    private readonly EnglishMasterDbContext dbContext;

    public BasicPublishContentBuilder(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<PublishContent> BuildAsync(
        PublishSourceType sourceType,
        Guid sourceId,
        PublishFormat format,
        string title,
        CancellationToken cancellationToken)
    {
        var sourceTitle = await ResolveSourceTitleAsync(sourceType, sourceId, cancellationToken);
        var exportTitle = string.IsNullOrWhiteSpace(sourceTitle) ? title : sourceTitle;
        var slug = SlugFileName(exportTitle);

        return format switch
        {
            PublishFormat.Html => new PublishContent(
                $"{slug}.html",
                BuildHtml(exportTitle, sourceType, sourceId),
                "text/html"),
            PublishFormat.Markdown => new PublishContent(
                $"{slug}.md",
                BuildMarkdown(exportTitle, sourceType, sourceId),
                "text/markdown"),
            PublishFormat.Pdf => new PublishContent(
                $"{slug}.pdf.txt",
                BuildPlaceholder(exportTitle, sourceType, sourceId, "PDF"),
                "text/plain"),
            PublishFormat.Docx => new PublishContent(
                $"{slug}.docx.txt",
                BuildPlaceholder(exportTitle, sourceType, sourceId, "DOCX"),
                "text/plain"),
            _ => throw new InvalidOperationException("Unsupported publish format.")
        };
    }

    private async Task<string?> ResolveSourceTitleAsync(
        PublishSourceType sourceType,
        Guid sourceId,
        CancellationToken cancellationToken)
    {
        return sourceType switch
        {
            PublishSourceType.Lesson => await dbContext.Lessons.AsNoTracking()
                .Where(lesson => lesson.Id == sourceId)
                .Select(lesson => lesson.Title)
                .FirstOrDefaultAsync(cancellationToken),
            PublishSourceType.Course => await dbContext.Courses.AsNoTracking()
                .Where(course => course.Id == sourceId)
                .Select(course => course.Title)
                .FirstOrDefaultAsync(cancellationToken),
            PublishSourceType.Book => await dbContext.Books.AsNoTracking()
                .Where(book => book.Id == sourceId)
                .Select(book => book.Title)
                .FirstOrDefaultAsync(cancellationToken),
            PublishSourceType.Quiz => await dbContext.Quizzes.AsNoTracking()
                .Where(quiz => quiz.Id == sourceId)
                .Select(quiz => quiz.Title)
                .FirstOrDefaultAsync(cancellationToken),
            _ => null
        };
    }

    private static string BuildHtml(string title, PublishSourceType sourceType, Guid sourceId)
    {
        return $"""
            <!doctype html>
            <html lang="en">
            <head>
                <meta charset="utf-8">
                <title>{WebUtility.HtmlEncode(title)}</title>
            </head>
            <body>
                <main>
                    <h1>{WebUtility.HtmlEncode(title)}</h1>
                    <p>Source: {sourceType}</p>
                    <p>Source Id: {sourceId}</p>
                </main>
            </body>
            </html>
            """;
    }

    private static string BuildMarkdown(string title, PublishSourceType sourceType, Guid sourceId)
    {
        return $"""
            # {title}

            Source: {sourceType}

            Source Id: {sourceId}
            """;
    }

    private static string BuildPlaceholder(string title, PublishSourceType sourceType, Guid sourceId, string format)
    {
        return $"""
            {format} rendering is not implemented yet.

            Title: {title}
            Source: {sourceType}
            Source Id: {sourceId}
            """;
    }

    private static string SlugFileName(string title)
    {
        var normalized = new string(title
            .Trim()
            .ToLowerInvariant()
            .Select(character => char.IsLetterOrDigit(character) ? character : '-')
            .ToArray());

        normalized = string.Join('-', normalized.Split('-', StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrWhiteSpace(normalized)
            ? $"publish-{Guid.NewGuid():N}"
            : normalized;
    }
}
