using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Tags;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Application.Features.Words.Dtos;
using EnglishMaster.Contracts.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Words.Commands;

public sealed class UpdateWordCommandHandler
{
    private readonly IWordRepository wordRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly ITagRepository tagRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public UpdateWordCommandHandler(
        IWordRepository wordRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IMediaRepository mediaRepository,
        TimeProvider timeProvider)
    {
        this.wordRepository = wordRepository;
        this.categoryRepository = categoryRepository;
        this.tagRepository = tagRepository;
        this.mediaRepository = mediaRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<WordDto>> HandleAsync(
        UpdateWordCommand command,
        CancellationToken cancellationToken)
    {
        var word = await wordRepository.GetByIdAsync(command.Id, cancellationToken);
        if (word is null)
        {
            return Result<WordDto>.NotFound(nameof(command.Id), "Word was not found.");
        }

        var validation = WordInputValidator.Validate(
            command.Text,
            command.IpaUk,
            command.IpaUs,
            command.ThaiReading,
            command.MeaningTh,
            command.MeaningEn,
            command.PartOfSpeech,
            command.CefrLevel,
            command.ExampleEn,
            command.ExampleTh,
            command.CategoryId,
            command.TagIds,
            command.ImageMediaId,
            command.AudioMediaId,
            command.IsActive);

        if (!validation.IsSuccess)
        {
            return Result<WordDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (await wordRepository.SlugExistsAsync(input.Slug, command.Id, cancellationToken))
        {
            return Result<WordDto>.Validation(
                new ValidationError(nameof(command.Text), "A word with this text already exists."));
        }

        var referenceValidation = await WordReferenceValidator.ValidateAsync(
            categoryRepository,
            tagRepository,
            mediaRepository,
            input,
            cancellationToken);
        if (referenceValidation.Errors.Count > 0)
        {
            return Result<WordDto>.Validation([.. referenceValidation.Errors]);
        }

        word.Update(
            input.Text,
            input.IpaUk,
            input.IpaUs,
            input.ThaiReading,
            input.MeaningTh,
            input.MeaningEn,
            input.PartOfSpeech,
            input.CefrLevel,
            input.ExampleEn,
            input.ExampleTh,
            input.CategoryId,
            input.TagIds,
            input.ImageMediaId,
            input.AudioMediaId,
            input.IsActive,
            timeProvider.GetUtcNow());

        await wordRepository.SaveChangesAsync(cancellationToken);

        return Result<WordDto>.Success(WordMapper.ToDto(
            word,
            referenceValidation.Category,
            referenceValidation.Tags,
            referenceValidation.ImageMedia,
            referenceValidation.AudioMedia));
    }
}
