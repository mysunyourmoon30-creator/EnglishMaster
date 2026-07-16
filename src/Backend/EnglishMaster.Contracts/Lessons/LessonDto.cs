namespace EnglishMaster.Contracts.Lessons;

public sealed record LessonDto(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string Description,
    string? CefrLevel,
    Guid? CategoryId,
    LessonCategoryDto? Category,
    Guid? ThumbnailMediaId,
    LessonMediaDto? ThumbnailMedia,
    int EstimatedMinutes,
    int SortOrder,
    IReadOnlyCollection<LessonWordSummaryDto> Words,
    IReadOnlyCollection<LessonGrammarRuleSummaryDto> GrammarRules,
    bool IsPublished,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record LessonCategoryDto(
    Guid Id,
    string Name,
    string Slug);

public sealed record LessonMediaDto(
    Guid Id,
    string FileName,
    string ContentType,
    string MediaType,
    string PublicUrl,
    string AltText);

public sealed record LessonWordSummaryDto(
    Guid Id,
    string Text,
    string Slug,
    string MeaningTh);

public sealed record LessonGrammarRuleSummaryDto(
    Guid Id,
    string Title,
    string Slug);
