using EnglishMaster.Contracts.GrammarTopics;
using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.Application.Features.GrammarTopics.Dtos;

internal static class GrammarTopicMapper
{
    public static GrammarTopicDto ToDto(GrammarTopic grammarTopic)
    {
        return new GrammarTopicDto(
            grammarTopic.Id,
            grammarTopic.Title,
            grammarTopic.Slug,
            grammarTopic.Summary,
            grammarTopic.CefrLevel.ToString(),
            grammarTopic.SortOrder,
            grammarTopic.IsActive,
            grammarTopic.CreatedAt,
            grammarTopic.UpdatedAt);
    }
}
