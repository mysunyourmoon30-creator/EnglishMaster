using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Books;

namespace EnglishMaster.Web.Services.Books;

public sealed class BookFormModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Subtitle { get; set; }

    [StringLength(1000)]
    public string? Summary { get; set; }

    [StringLength(4000)]
    public string? Description { get; set; }

    public string? CefrLevel { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? CoverMediaId { get; set; }

    public Guid? CourseId { get; set; }

    [StringLength(200)]
    public string? AuthorName { get; set; }

    [StringLength(100)]
    public string? Edition { get; set; }

    [StringLength(50)]
    public string? Version { get; set; }

    [Range(0, int.MaxValue)]
    public int EstimatedPages { get; set; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsPublished { get; set; }

    public bool IsActive { get; set; } = true;

    public static BookFormModel FromDto(BookDto book)
    {
        return new BookFormModel
        {
            Title = book.Title,
            Subtitle = book.Subtitle,
            Summary = book.Summary,
            Description = book.Description,
            CefrLevel = book.CefrLevel,
            CategoryId = book.CategoryId,
            CoverMediaId = book.CoverMediaId,
            CourseId = book.CourseId,
            AuthorName = book.AuthorName,
            Edition = book.Edition,
            Version = book.Version,
            EstimatedPages = book.EstimatedPages,
            SortOrder = book.SortOrder,
            IsPublished = book.IsPublished,
            IsActive = book.IsActive
        };
    }

    public CreateBookRequest ToCreateRequest()
    {
        return new CreateBookRequest(
            Title,
            Subtitle,
            Summary,
            Description,
            CefrLevel,
            CategoryId,
            CoverMediaId,
            CourseId,
            AuthorName,
            Edition,
            Version,
            EstimatedPages,
            SortOrder);
    }

    public UpdateBookRequest ToUpdateRequest()
    {
        return new UpdateBookRequest(
            Title,
            Subtitle,
            Summary,
            Description,
            CefrLevel,
            CategoryId,
            CoverMediaId,
            CourseId,
            AuthorName,
            Edition,
            Version,
            EstimatedPages,
            SortOrder,
            IsPublished,
            IsActive);
    }
}
