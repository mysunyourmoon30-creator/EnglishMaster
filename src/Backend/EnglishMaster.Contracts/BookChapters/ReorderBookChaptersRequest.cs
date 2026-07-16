namespace EnglishMaster.Contracts.BookChapters;

public sealed record ReorderBookChaptersRequest(
    IReadOnlyList<Guid> OrderedChapterIds);
