using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Lessons.Dtos;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Lessons.Queries;

public sealed class GetLessonByIdQueryHandler
{
    private readonly ILessonRepository lessonRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly IWordRepository wordRepository;
    private readonly IGrammarRuleRepository grammarRuleRepository;

    public GetLessonByIdQueryHandler(
        ILessonRepository lessonRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        IWordRepository wordRepository,
        IGrammarRuleRepository grammarRuleRepository)
    {
        this.lessonRepository = lessonRepository;
        this.categoryRepository = categoryRepository;
        this.mediaRepository = mediaRepository;
        this.wordRepository = wordRepository;
        this.grammarRuleRepository = grammarRuleRepository;
    }

    public async Task<Result<LessonDto>> HandleAsync(
        GetLessonByIdQuery query,
        CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(query.Id, cancellationToken);
        if (lesson is null)
        {
            return Result<LessonDto>.NotFound(nameof(query.Id), "Lesson was not found.");
        }

        return Result<LessonDto>.Success(await LessonReadModelBuilder.MapAsync(
            lesson,
            categoryRepository,
            mediaRepository,
            wordRepository,
            grammarRuleRepository,
            cancellationToken));
    }
}
