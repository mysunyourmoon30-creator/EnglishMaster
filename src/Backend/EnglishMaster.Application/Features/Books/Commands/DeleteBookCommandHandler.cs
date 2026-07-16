using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Books.Commands;

public sealed class DeleteBookCommandHandler
{
    private readonly IBookRepository bookRepository;
    private readonly TimeProvider timeProvider;

    public DeleteBookCommandHandler(
        IBookRepository bookRepository,
        TimeProvider timeProvider)
    {
        this.bookRepository = bookRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteBookCommand command,
        CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(command.Id, cancellationToken);
        if (book is null)
        {
            return Result.NotFound(nameof(command.Id), "Book was not found.");
        }

        book.Deactivate(timeProvider.GetUtcNow());
        await bookRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
