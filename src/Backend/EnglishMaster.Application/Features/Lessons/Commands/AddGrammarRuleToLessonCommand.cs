namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed record AddGrammarRuleToLessonCommand(Guid LessonId, Guid GrammarRuleId, int SortOrder);
