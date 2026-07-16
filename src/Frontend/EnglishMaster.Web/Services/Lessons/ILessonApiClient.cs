using EnglishMaster.Contracts.LessonSections;
using EnglishMaster.Contracts.Lessons;

namespace EnglishMaster.Web.Services.Lessons;

public interface ILessonApiClient
{
    Task<LessonSearchResponse> SearchAsync(LessonSearchRequest request, CancellationToken cancellationToken);

    Task<LessonDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<LessonDto> CreateAsync(CreateLessonRequest request, CancellationToken cancellationToken);

    Task<LessonDto> UpdateAsync(Guid id, UpdateLessonRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<LessonDto> PublishAsync(Guid id, CancellationToken cancellationToken);

    Task<LessonDto> UnpublishAsync(Guid id, CancellationToken cancellationToken);

    Task<LessonDto> AddWordAsync(Guid lessonId, Guid wordId, int sortOrder, CancellationToken cancellationToken);

    Task RemoveWordAsync(Guid lessonId, Guid wordId, CancellationToken cancellationToken);

    Task<LessonDto> AddGrammarRuleAsync(
        Guid lessonId,
        Guid grammarRuleId,
        int sortOrder,
        CancellationToken cancellationToken);

    Task RemoveGrammarRuleAsync(Guid lessonId, Guid grammarRuleId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LessonSectionDto>> GetSectionsAsync(Guid lessonId, CancellationToken cancellationToken);

    Task<LessonSectionDto> AddSectionAsync(
        Guid lessonId,
        CreateLessonSectionRequest request,
        CancellationToken cancellationToken);

    Task<LessonSectionDto> UpdateSectionAsync(
        Guid id,
        UpdateLessonSectionRequest request,
        CancellationToken cancellationToken);

    Task DeleteSectionAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LessonSectionDto>> ReorderSectionsAsync(
        Guid lessonId,
        IReadOnlyList<Guid> orderedSectionIds,
        CancellationToken cancellationToken);
}
