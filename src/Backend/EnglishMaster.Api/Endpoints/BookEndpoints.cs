using EnglishMaster.Application.Features.BookChapters.Commands;
using EnglishMaster.Application.Features.BookChapters.Queries;
using EnglishMaster.Application.Features.Books.Commands;
using EnglishMaster.Application.Features.Books.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Contracts.Books;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var books = endpoints.MapGroup("/api/v1/books")
            .WithTags("Books");

        books.MapGet("", SearchBooksAsync).RequireAuthorization(Permissions.BooksRead);
        books.MapGet("/{id:guid}", GetBookAsync).RequireAuthorization(Permissions.BooksRead);
        books.MapPost("", CreateBookAsync).RequireAuthorization(Permissions.BooksCreate);
        books.MapPut("/{id:guid}", UpdateBookAsync).RequireAuthorization(Permissions.BooksUpdate);
        books.MapDelete("/{id:guid}", DeleteBookAsync).RequireAuthorization(Permissions.BooksDelete);
        books.MapPost("/{id:guid}/publish", PublishBookAsync).RequireAuthorization(Permissions.BooksUpdate);
        books.MapPost("/{id:guid}/unpublish", UnpublishBookAsync).RequireAuthorization(Permissions.BooksUpdate);
        books.MapPost("/{id:guid}/activate", ActivateBookAsync).RequireAuthorization(Permissions.BooksUpdate);
        books.MapPost("/{id:guid}/deactivate", DeactivateBookAsync).RequireAuthorization(Permissions.BooksUpdate);
        books.MapGet("/{bookId:guid}/chapters", GetChaptersByBookIdAsync).RequireAuthorization(Permissions.BooksRead);
        books.MapPost("/{bookId:guid}/chapters", AddChapterAsync).RequireAuthorization(Permissions.BooksCreate);
        books.MapPost("/{bookId:guid}/chapters/reorder", ReorderChaptersAsync).RequireAuthorization(Permissions.BooksUpdate);

        var bookChapters = endpoints.MapGroup("/api/v1/book-chapters")
            .WithTags("Book Chapters");

        bookChapters.MapGet("/{id:guid}", GetChapterAsync).RequireAuthorization(Permissions.BooksRead);
        bookChapters.MapPut("/{id:guid}", UpdateChapterAsync).RequireAuthorization(Permissions.BooksUpdate);
        bookChapters.MapDelete("/{id:guid}", DeleteChapterAsync).RequireAuthorization(Permissions.BooksDelete);
        bookChapters.MapGet("/{chapterId:guid}/lessons", GetChapterLessonsAsync).RequireAuthorization(Permissions.BooksRead);
        bookChapters.MapPost("/{chapterId:guid}/lessons/{lessonId:guid}", AddLessonAsync).RequireAuthorization(Permissions.BooksUpdate);
        bookChapters.MapDelete("/{chapterId:guid}/lessons/{lessonId:guid}", RemoveLessonAsync).RequireAuthorization(Permissions.BooksUpdate);
        bookChapters.MapPost("/{chapterId:guid}/lessons/reorder", ReorderChapterLessonsAsync).RequireAuthorization(Permissions.BooksUpdate);

        return endpoints;
    }

    private static async Task<IResult> SearchBooksAsync(
        SearchBooksQueryHandler handler,
        string? search,
        string? cefrLevel,
        Guid? categoryId,
        Guid? courseId,
        bool? isPublished,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        string? sortBy,
        string? sortDirection,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchBooksQuery(
                search,
                cefrLevel,
                categoryId,
                courseId,
                isPublished,
                isActive,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetBookAsync(
        Guid id,
        GetBookByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetBookByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateBookAsync(
        CreateBookRequest request,
        CreateBookCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateBookCommand(
                request.Title,
                request.Subtitle,
                request.Summary,
                request.Description,
                request.CefrLevel,
                request.CategoryId,
                request.CoverMediaId,
                request.CourseId,
                request.AuthorName,
                request.Edition,
                request.Version,
                request.EstimatedPages,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/books/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateBookAsync(
        Guid id,
        UpdateBookRequest request,
        UpdateBookCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateBookCommand(
                id,
                request.Title,
                request.Subtitle,
                request.Summary,
                request.Description,
                request.CefrLevel,
                request.CategoryId,
                request.CoverMediaId,
                request.CourseId,
                request.AuthorName,
                request.Edition,
                request.Version,
                request.EstimatedPages,
                request.SortOrder,
                request.IsPublished,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteBookAsync(
        Guid id,
        DeleteBookCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteBookCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> PublishBookAsync(
        Guid id,
        PublishBookCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new PublishBookCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> UnpublishBookAsync(
        Guid id,
        UnpublishBookCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new UnpublishBookCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> ActivateBookAsync(
        Guid id,
        ActivateBookCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new ActivateBookCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeactivateBookAsync(
        Guid id,
        DeactivateBookCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeactivateBookCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetChaptersByBookIdAsync(
        Guid bookId,
        GetBookChaptersByBookIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetBookChaptersByBookIdQuery(bookId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> AddChapterAsync(
        Guid bookId,
        CreateBookChapterRequest request,
        AddBookChapterCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new AddBookChapterCommand(
                bookId,
                request.Title,
                request.Summary,
                request.ContentMarkdown,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/book-chapters/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> ReorderChaptersAsync(
        Guid bookId,
        ReorderBookChaptersRequest request,
        ReorderBookChaptersCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ReorderBookChaptersCommand(bookId, request.OrderedChapterIds),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetChapterAsync(
        Guid id,
        GetBookChapterByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetBookChapterByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> UpdateChapterAsync(
        Guid id,
        UpdateBookChapterRequest request,
        UpdateBookChapterCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateBookChapterCommand(
                id,
                request.Title,
                request.Summary,
                request.ContentMarkdown,
                request.SortOrder,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteChapterAsync(
        Guid id,
        DeleteBookChapterCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteBookChapterCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetChapterLessonsAsync(
        Guid chapterId,
        GetBookChapterLessonsByBookChapterIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetBookChapterLessonsByBookChapterIdQuery(chapterId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> AddLessonAsync(
        Guid chapterId,
        Guid lessonId,
        AddLessonToBookChapterCommandHandler handler,
        CancellationToken cancellationToken,
        int sortOrder = 0)
    {
        var result = await handler.HandleAsync(
            new AddLessonToBookChapterCommand(chapterId, lessonId, sortOrder),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> RemoveLessonAsync(
        Guid chapterId,
        Guid lessonId,
        RemoveLessonFromBookChapterCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new RemoveLessonFromBookChapterCommand(chapterId, lessonId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> ReorderChapterLessonsAsync(
        Guid chapterId,
        ReorderBookChapterLessonsRequest request,
        ReorderBookChapterLessonsCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ReorderBookChapterLessonsCommand(chapterId, request.OrderedBookChapterLessonIds),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static IResult ToHttpResult(Result result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.NoContent(),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };
    }

    private static IResult ToHttpResult<T>(Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.Ok(result.Value),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };
    }

    private static Dictionary<string, string[]> ToValidationDictionary(
        IEnumerable<ValidationError> errors)
    {
        return errors
            .GroupBy(error => error.Field)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).ToArray());
    }
}
