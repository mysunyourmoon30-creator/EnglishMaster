using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.BookChapters;

namespace EnglishMaster.Web.Services.Books;

public sealed class BookChapterFormModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Summary { get; set; }

    [StringLength(12000)]
    public string? ContentMarkdown { get; set; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public static BookChapterFormModel FromDto(BookChapterDto chapter)
    {
        return new BookChapterFormModel
        {
            Title = chapter.Title,
            Summary = chapter.Summary,
            ContentMarkdown = chapter.ContentMarkdown,
            SortOrder = chapter.SortOrder,
            IsActive = chapter.IsActive
        };
    }

    public CreateBookChapterRequest ToCreateRequest()
    {
        return new CreateBookChapterRequest(
            Title,
            Summary,
            ContentMarkdown,
            SortOrder);
    }

    public UpdateBookChapterRequest ToUpdateRequest()
    {
        return new UpdateBookChapterRequest(
            Title,
            Summary,
            ContentMarkdown,
            SortOrder,
            IsActive);
    }
}
