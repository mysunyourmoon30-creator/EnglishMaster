using EnglishMaster.Application.Features.Books.Dtos;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Books;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Books.Commands;

public sealed class UpdateBookCommandHandler
{
    private readonly IBookRepository bookRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly ICourseRepository courseRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public UpdateBookCommandHandler(
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.bookRepository = bookRepository;
        this.categoryRepository = categoryRepository;
        this.mediaRepository = mediaRepository;
        this.courseRepository = courseRepository;
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<BookDto>> HandleAsync(
        UpdateBookCommand command,
        CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(command.Id, cancellationToken);
        if (book is null)
        {
            return Result<BookDto>.NotFound(nameof(command.Id), "Book was not found.");
        }

        var validation = BookInputValidator.Validate(
            command.Title,
            command.Subtitle,
            command.Summary,
            command.Description,
            command.CefrLevel,
            command.CategoryId,
            command.CoverMediaId,
            command.CourseId,
            command.AuthorName,
            command.Edition,
            command.Version,
            command.EstimatedPages,
            command.SortOrder,
            command.IsPublished,
            command.IsActive);

        if (!validation.IsSuccess)
        {
            return Result<BookDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var referenceValidation = await BookReferenceValidator.ValidateReferencesAsync(
            categoryRepository,
            mediaRepository,
            courseRepository,
            input,
            cancellationToken);
        if (referenceValidation.Errors.Count > 0)
        {
            return Result<BookDto>.Validation([.. referenceValidation.Errors]);
        }

        if (await bookRepository.SlugExistsAsync(input.Slug, command.Id, cancellationToken))
        {
            return Result<BookDto>.Validation(
                new ValidationError(nameof(command.Title), "A book with this title already exists."));
        }

        book.Update(
            input.Title,
            input.Subtitle,
            input.Summary,
            input.Description,
            input.CefrLevel,
            input.CategoryId,
            input.CoverMediaId,
            input.CourseId,
            input.AuthorName,
            input.Edition,
            input.Version,
            input.EstimatedPages,
            input.SortOrder,
            input.IsPublished,
            input.IsActive,
            timeProvider.GetUtcNow());

        await bookRepository.SaveChangesAsync(cancellationToken);

        return Result<BookDto>.Success(await BookReadModelBuilder.MapAsync(
            book,
            categoryRepository,
            mediaRepository,
            courseRepository,
            lessonRepository,
            cancellationToken));
    }
}
