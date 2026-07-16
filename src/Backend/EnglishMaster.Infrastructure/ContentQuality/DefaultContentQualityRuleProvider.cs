using EnglishMaster.Application.Features.ContentQuality;
using EnglishMaster.Application.Features.ContentQuality.Dtos;
using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.ContentQuality;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Infrastructure.ContentQuality;

public sealed class DefaultContentQualityRuleProvider : IContentQualityRuleProvider
{
    public IReadOnlyCollection<ContentQualityFindingCandidate> Evaluate(string contentType, object content)
    {
        var findings = new List<ContentQualityFindingCandidate>();
        switch (Normalize(contentType))
        {
            case "word" when content is Word word:
                Required(findings, "WORD.MEANING_TH", word.MeaningTh, "MeaningTh", "MeaningTh is required.", ContentQualitySeverity.Error);
                Recommended(findings, "WORD.MEANING_EN", word.MeaningEn, "MeaningEn", "Add an English meaning.");
                if (string.IsNullOrWhiteSpace(word.IpaUk) && string.IsNullOrWhiteSpace(word.IpaUs))
                {
                    findings.Add(Warning("WORD.IPA", "IpaUk", "IPA UK or IPA US should exist.", "Add at least one IPA transcription."));
                }

                Recommended(findings, "WORD.EXAMPLE_EN", word.ExampleEn, "ExampleEn", "Add an English example sentence.");
                Recommended(findings, "WORD.EXAMPLE_TH", word.ExampleTh, "ExampleTh", "Add a Thai example translation.");
                break;
            case "pronunciation" when content is Pronunciation pronunciation:
                if (pronunciation.WordId == Guid.Empty)
                {
                    findings.Add(Error("PRONUNCIATION.WORD", "WordId", "WordId is required.", "Attach pronunciation to a word."));
                }

                if (string.IsNullOrWhiteSpace(pronunciation.IpaUk) && string.IsNullOrWhiteSpace(pronunciation.IpaUs))
                {
                    findings.Add(Warning("PRONUNCIATION.IPA", "IpaUk", "IPA UK or IPA US should exist.", "Add at least one IPA transcription."));
                }

                if (!pronunciation.AudioSlowMediaId.HasValue && !pronunciation.AudioNormalMediaId.HasValue)
                {
                    findings.Add(Warning("PRONUNCIATION.AUDIO", "AudioMediaId", "Audio is recommended.", "Attach slow or normal audio."));
                }

                break;
            case "grammartopic" when content is GrammarTopic topic:
                if (topic.Rules.Count == 0)
                {
                    findings.Add(Error("GRAMMAR_TOPIC.RULES", "Rules", "GrammarTopic must have at least one rule.", "Add a grammar rule."));
                }

                break;
            case "grammarrule" when content is GrammarRule rule:
                Required(findings, "GRAMMAR_RULE.RULE_TEXT", rule.RuleText, "RuleText", "GrammarRule must have RuleText.", ContentQualitySeverity.Error);
                if (rule.Examples.Count == 0)
                {
                    findings.Add(Warning("GRAMMAR_RULE.EXAMPLES", "Examples", "GrammarRule should have examples.", "Add one or more examples."));
                }

                break;
            case "grammarexample" when content is GrammarExample example:
                Required(findings, "GRAMMAR_EXAMPLE.EXAMPLE_EN", example.ExampleEn, "ExampleEn", "GrammarExample must have ExampleEn.", ContentQualitySeverity.Error);
                break;
            case "lesson" when content is Lesson lesson:
                if (lesson.Sections.Count == 0)
                {
                    findings.Add(Error("LESSON.SECTIONS", "Sections", "Lesson must have at least one section.", "Add lesson sections."));
                }

                if (lesson.Words.Count == 0 && lesson.GrammarRules.Count == 0)
                {
                    findings.Add(Warning("LESSON.RELATED_CONTENT", "Words", "Lesson should have related words or grammar rules.", "Attach related learning content."));
                }

                break;
            case "course" when content is Course course:
                if (course.Lessons.Count == 0)
                {
                    findings.Add(Error("COURSE.LESSONS", "Lessons", "Course must have at least one lesson.", "Add lessons to the course."));
                }

                break;
            case "book" when content is Book book:
                if (book.Chapters.Count == 0)
                {
                    findings.Add(Error("BOOK.CHAPTERS", "Chapters", "Book must have at least one chapter.", "Add chapters to the book."));
                }

                break;
            case "quiz" when content is Quiz quiz:
                if (quiz.Questions.Count == 0)
                {
                    findings.Add(Error("QUIZ.QUESTIONS", "Questions", "Quiz must have at least one question.", "Add quiz questions."));
                }

                foreach (var question in quiz.Questions.Where(question => question.IsActive))
                {
                    var activeChoices = question.Choices.Where(choice => choice.IsActive).ToArray();
                    if (question.QuestionType is QuizQuestionType.SingleChoice or QuizQuestionType.TrueFalse && activeChoices.Count(choice => choice.IsCorrect) != 1)
                    {
                        findings.Add(Error("QUIZ.CORRECT_CHOICE", "Choices", "SingleChoice and TrueFalse questions should have exactly one correct choice.", "Set exactly one active correct choice."));
                    }
                }

                break;
            case "publishing" when content is PublishJob publishJob:
                if (publishJob.SourceId == Guid.Empty)
                {
                    findings.Add(Error("PUBLISHING.SOURCE_ID", "SourceId", "PublishJob source must exist.", "Select a valid source."));
                }

                break;
        }

        return findings;
    }

    private static string Normalize(string value) => value.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().ToLowerInvariant();

    private static void Required(List<ContentQualityFindingCandidate> findings, string code, string value, string field, string message, ContentQualitySeverity severity)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        findings.Add(severity == ContentQualitySeverity.Error
            ? Error(code, field, message, "Fill in the required field.")
            : Warning(code, field, message, "Review the field."));
    }

    private static void Recommended(List<ContentQualityFindingCandidate> findings, string code, string value, string field, string recommendation)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            findings.Add(Warning(code, field, $"{field} should exist.", recommendation));
        }
    }

    private static ContentQualityFindingCandidate Error(string code, string field, string message, string recommendation) =>
        new(code, ContentQualitySeverity.Error.ToString(), message, field, recommendation);

    private static ContentQualityFindingCandidate Warning(string code, string field, string message, string recommendation) =>
        new(code, ContentQualitySeverity.Warning.ToString(), message, field, recommendation);
}
