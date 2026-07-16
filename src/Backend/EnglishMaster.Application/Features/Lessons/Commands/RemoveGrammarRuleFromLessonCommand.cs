namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed record RemoveGrammarRuleFromLessonCommand(Guid LessonId, Guid GrammarRuleId);
