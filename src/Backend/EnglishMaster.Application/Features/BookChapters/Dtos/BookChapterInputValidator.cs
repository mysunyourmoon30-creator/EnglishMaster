using EnglishMaster.Domain.Books;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Dtos;

internal static class BookChapterInputValidator
{
    public static Result<BookChapterInput> Validate(
        string? title,
        string? summary,
        string? contentMarkdown,
        int sortOrder,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        var normalizedTitle = NormalizeRequired(title, nameof(title), BookChapterFieldLimits.Title, errors);
        var normalizedSlug = NormalizeSlug(normalizedTitle, nameof(title), errors);
        var normalizedSummary = NormalizeOptional(summary, nameof(summary), BookChapterFieldLimits.Summary, errors);
        var normalizedContentMarkdown = NormalizeOptional(contentMarkdown, nameof(contentMarkdown), BookChapterFieldLimits.ContentMarkdown, errors);

        if (sortOrder < 0)
        {
            errors.Add(new ValidationError(nameof(sortOrder), $"{nameof(sortOrder)} must be greater than or equal to zero."));
        }

        if (errors.Count > 0)
        {
            return Result<BookChapterInput>.Validation([.. errors]);
        }

        return Result<BookChapterInput>.Success(new BookChapterInput(
            normalizedTitle,
            normalizedSlug,
            normalizedSummary,
            normalizedContentMarkdown,
            sortOrder,
            isActive));
    }

    private static string NormalizeSlug(
        string normalizedTitle,
        string field,
        ICollection<ValidationError> errors)
    {
        if (normalizedTitle.Length == 0)
        {
            return string.Empty;
        }

        try
        {
            return BookChapter.GenerateSlug(normalizedTitle);
        }
        catch (ArgumentException)
        {
            errors.Add(new ValidationError(field, $"{field} must contain at least one letter or digit."));
            return string.Empty;
        }
    }

    private static string NormalizeRequired(
        string? value,
        string field,
        int maxLength,
        ICollection<ValidationError> errors)
    {
        var normalized = NormalizeOptional(value, field, maxLength, errors);
        if (normalized.Length == 0)
        {
            errors.Add(new ValidationError(field, $"{field} is required."));
        }

        return normalized;
    }

    private static string NormalizeOptional(
        string? value,
        string field,
        int maxLength,
        ICollection<ValidationError> errors)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            errors.Add(new ValidationError(field, $"{field} must be {maxLength} characters or fewer."));
        }

        return normalized;
    }
}
