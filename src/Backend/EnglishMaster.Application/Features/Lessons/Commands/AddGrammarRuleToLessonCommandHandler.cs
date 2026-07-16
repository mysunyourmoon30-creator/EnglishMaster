using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Lessons.Dtos;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed class AddGrammarRuleToLessonCommandHandler
{
    private readonly ILessonRepository lessonRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly IWordRepository wordRepository;
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly TimeProvider timeProvider;

    public AddGrammarRuleToLessonCommandHandler(
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
        AddGrammarRuleToLessonCommand command,
        CancellationToken cancellationToken)
    {
        if (command.LessonId == Guid.Empty)
        {
            return Result<LessonDto>.Validation(new ValidationError(nameof(command.LessonId), $"{nameof(command.LessonId)} cannot be empty."));
        }

        if (command.GrammarRuleId == Guid.Empty)
        {
            return Result<LessonDto>.Validation(new ValidationError(nameof(command.GrammarRuleId), $"{nameof(command.GrammarRuleId)} cannot be empty."));
        }

        if (command.SortOrder < 0)
        {
            return Result<LessonDto>.Validation(new ValidationError(nameof(command.SortOrder), $"{nameof(command.SortOrder)} must be greater than or equal to zero."));
        }

        var lesson = await lessonRepository.GetByIdAsync(command.LessonId, cancellationToken);
        if (lesson is null)
        {
            return Result<LessonDto>.NotFound(nameof(command.LessonId), "Lesson was not found.");
        }

        if (!lesson.IsActive)
        {
            return Result<LessonDto>.Validation(new ValidationError(nameof(command.LessonId), "Lesson is inactive."));
        }

        var grammarRuleErrors = await LessonReferenceValidator.ValidateGrammarRuleAsync(
            grammarRuleRepository,
            command.GrammarRuleId,
            cancellationToken);
        if (grammarRuleErrors.Count > 0)
        {
            return Result<LessonDto>.Validation([.. grammarRuleErrors]);
        }

        lesson.AddGrammarRule(command.GrammarRuleId, command.SortOrder, timeProvider.GetUtcNow());
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
