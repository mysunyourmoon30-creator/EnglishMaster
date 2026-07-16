namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed record ReorderBookChaptersCommand(
    Guid BookId,
    IReadOnlyList<Guid> OrderedChapterIds);
