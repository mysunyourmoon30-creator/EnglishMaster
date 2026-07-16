using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Lessons.Dtos;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed class PublishLessonCommandHandler
{
    private readonly ILessonRepository lessonRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly IWordRepository wordRepository;
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly TimeProvider timeProvider;

    public PublishLessonCommandHandler(
        ILessonRepository lessonRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        IWordRepository wordRepository,
        IGrammarRuleRepository grammarRuleRepository,
        TimeProvider timeProvider)
    {
        this.lessonRepository = lessonRepository;
        this.categoryRepository = categoryRepository;
        this.mediaRepository = mediaRepository;
        this.wordRepository = wordRepository;
        this.grammarRuleRepository = grammarRuleRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<LessonDto>> HandleAsync(
        PublishLessonCommand command,
        CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(command.Id, cancellationToken);
        if (lesson is null)
        {
            return Result<LessonDto>.NotFound(nameof(command.Id), "Lesson was not found.");
        }

        lesson.Publish(timeProvider.GetUtcNow());
        await lessonRepository.SaveChangesAsync(cancellationToken);

        return Result<LessonDto>.Success(await LessonReadModelBuilder.MapAsync(
            lesson,
            categoryRepository,
            mediaRepository,
            wordRepository,
            grammarRuleRepository,
            cancellationToken));
    }
}
