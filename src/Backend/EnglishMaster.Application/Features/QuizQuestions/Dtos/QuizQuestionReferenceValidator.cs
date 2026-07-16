using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Pronunciations;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizQuestions.Dtos;

internal static class QuizQuestionReferenceValidator
{
    public static async Task<IReadOnlyCollection<ValidationError>> ValidateReferencesAsync(
        IWordRepository wordRepository,
        IGrammarRuleRepository grammarRuleRepository,
        IPronunciationRepository pronunciationRepository,
        QuizQuestionInput input,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();

        if (input.WordId.HasValue)
        {
            var word = await wordRepository.GetByIdAsync(input.WordId.Value, cancellationToken);
            if (word is null)
            {
                errors.Add(new ValidationError(nameof(input.WordId), "Word was not found."));
            }
            else if (!word.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.WordId), "Word is inactive."));
            }
        }

        if (input.GrammarRuleId.HasValue)
        {
            var grammarRule = await grammarRuleRepository.GetByIdAsync(input.GrammarRuleId.Value, cancellationToken);
            if (grammarRule is null)
            {
                errors.Add(new ValidationError(nameof(input.GrammarRuleId), "Grammar rule was not found."));
            }
            else if (!grammarRule.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.GrammarRuleId), "Grammar rule is inactive."));
            }
        }

        if (input.PronunciationId.HasValue)
        {
            var pronunciation = await pronunciationRepository.GetByIdAsync(input.PronunciationId.Value, cancellationToken);
            if (pronunciation is null)
            {
                errors.Add(new ValidationError(nameof(input.PronunciationId), "Pronunciation was not found."));
            }
            else if (!pronunciation.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.PronunciationId), "Pronunciation is inactive."));
            }
        }

        return errors;
    }
}
