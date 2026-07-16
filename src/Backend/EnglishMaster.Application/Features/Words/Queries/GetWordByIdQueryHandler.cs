using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Pronunciations;
using EnglishMaster.Application.Features.Tags;
using EnglishMaster.Application.Features.Words.Dtos;
using EnglishMaster.Contracts.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Words.Queries;

public sealed class GetWordByIdQueryHandler
{
    private readonly IWordRepository wordRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly ITagRepository tagRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly IPronunciationRepository pronunciationRepository;

    public GetWordByIdQueryHandler(
        IWordRepository wordRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IMediaRepository mediaRepository,
        IPronunciationRepository pronunciationRepository)
    {
        this.wordRepository = wordRepository;
        this.categoryRepository = categoryRepository;
        this.tagRepository = tagRepository;
        this.mediaRepository = mediaRepository;
        this.pronunciationRepository = pronunciationRepository;
    }

    public async Task<Result<WordDto>> HandleAsync(GetWordByIdQuery query, CancellationToken cancellationToken)
    {
        var word = await wordRepository.GetByIdAsync(query.Id, cancellationToken);
        if (word is null)
        {
            return Result<WordDto>.NotFound(nameof(query.Id), "Word was not found.");
        }

        var category = word.CategoryId.HasValue
            ? await categoryRepository.GetByIdAsync(word.CategoryId.Value, cancellationToken)
            : null;
        var tags = word.Tags.Count == 0
            ? []
            : await tagRepository.GetByIdsAsync(word.Tags.Select(tag => tag.TagId), cancellationToken);
        var imageMedia = word.ImageMediaId.HasValue
            ? await mediaRepository.GetByIdAsync(word.ImageMediaId.Value, cancellationToken)
            : null;
        var audioMedia = word.AudioMediaId.HasValue
            ? await mediaRepository.GetByIdAsync(word.AudioMediaId.Value, cancellationToken)
            : null;
        var pronunciation = await pronunciationRepository.GetByWordIdAsync(word.Id, cancellationToken);
        var pronunciationMedia = pronunciation is null
            ? []
            : await mediaRepository.GetByIdsAsync(GetPronunciationMediaIds(pronunciation), cancellationToken);
        var pronunciationMediaById = pronunciationMedia.ToDictionary(media => media.Id);

        return Result<WordDto>.Success(WordMapper.ToDto(
            word,
            category,
            tags,
            imageMedia,
            audioMedia,
            pronunciation,
            GetMedia(pronunciationMediaById, pronunciation?.AudioSlowMediaId),
            GetMedia(pronunciationMediaById, pronunciation?.AudioNormalMediaId),
            GetMedia(pronunciationMediaById, pronunciation?.MouthImageMediaId),
            pronunciationMediaById));
    }

    private static IEnumerable<Guid> GetPronunciationMediaIds(Domain.Pronunciations.Pronunciation pronunciation)
    {
        return new[]
            {
                pronunciation.AudioSlowMediaId,
                pronunciation.AudioNormalMediaId,
                pronunciation.MouthImageMediaId
            }
            .Concat(pronunciation.MinimalPairs.Select(pair => pair.AudioMediaId))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct();
    }

    private static Domain.Media.Media? GetMedia(
        IReadOnlyDictionary<Guid, Domain.Media.Media> mediaById,
        Guid? id)
    {
        return id.HasValue && mediaById.TryGetValue(id.Value, out var media)
            ? media
            : null;
    }
}
