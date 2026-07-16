using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.Application.Features.QuizQuestions.Dtos;

internal static class QuizQuestionReadModelBuilder
{
    public static QuizQuestionDto Map(QuizQuestion question)
    {
        return QuizQuestionMapper.ToDto(question);
    }

    public static IReadOnlyCollection<QuizQuestionDto> Map(IReadOnlyCollection<QuizQuestion> questions)
    {
        return QuizQuestionMapper.ToDtos(questions);
    }
}
