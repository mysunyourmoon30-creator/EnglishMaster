using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Contracts.Quizzes;

namespace EnglishMaster.IntegrationTests.Quizzes;

public sealed class QuizEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public QuizEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task QuizEndpointsSupportQuestionsChoicesPublishAndSoftDelete()
    {
        var quizResponse = await client.PostAsJsonAsync(
            "/api/v1/quizzes",
            new CreateQuizRequest(
                "Starter Quiz Integration",
                "Check beginner knowledge",
                "A short admin quiz.",
                "A1",
                null,
                null,
                null,
                null,
                15,
                70,
                0));
        Assert.Equal(HttpStatusCode.Created, quizResponse.StatusCode);
        var quiz = await quizResponse.Content.ReadFromJsonAsync<QuizDto>();
        Assert.NotNull(quiz);
        Assert.Equal("starter-quiz-integration", quiz.Slug);
        Assert.False(quiz.IsPublished);

        var quizSearch = await client.GetFromJsonAsync<QuizSearchResponse>(
            "/api/v1/quizzes?cefrLevel=A1&search=Starter");
        Assert.Contains(quizSearch!.Items, item => item.Id == quiz.Id);

        var firstQuestionResponse = await client.PostAsJsonAsync(
            $"/api/v1/quizzes/{quiz.Id}/questions",
            new CreateQuizQuestionRequest(
                "Choose the correct greeting.",
                "SingleChoice",
                null,
                "Use a normal greeting.",
                1,
                0,
                null,
                null,
                null));
        Assert.Equal(HttpStatusCode.Created, firstQuestionResponse.StatusCode);
        var firstQuestion = await firstQuestionResponse.Content.ReadFromJsonAsync<QuizQuestionDto>();

        var secondQuestionResponse = await client.PostAsJsonAsync(
            $"/api/v1/quizzes/{quiz.Id}/questions",
            new CreateQuizQuestionRequest(
                "Choose all vowels.",
                "MultipleChoice",
                null,
                null,
                2,
                1,
                null,
                null,
                null));
        Assert.Equal(HttpStatusCode.Created, secondQuestionResponse.StatusCode);
        var secondQuestion = await secondQuestionResponse.Content.ReadFromJsonAsync<QuizQuestionDto>();

        var questions = await client.GetFromJsonAsync<IReadOnlyCollection<QuizQuestionDto>>(
            $"/api/v1/quizzes/{quiz.Id}/questions");
        Assert.Equal(2, questions!.Count);

        var updateQuestionResponse = await client.PutAsJsonAsync(
            $"/api/v1/quiz-questions/{firstQuestion!.Id}",
            new UpdateQuizQuestionRequest(
                "Choose the correct greeting updated.",
                "SingleChoice",
                null,
                "Updated explanation.",
                1,
                0,
                null,
                null,
                null,
                true));
        Assert.Equal(HttpStatusCode.OK, updateQuestionResponse.StatusCode);
        var updatedQuestion = await updateQuestionResponse.Content.ReadFromJsonAsync<QuizQuestionDto>();
        Assert.Equal("Choose the correct greeting updated.", updatedQuestion!.QuestionText);

        var correctChoiceResponse = await client.PostAsJsonAsync(
            $"/api/v1/quiz-questions/{firstQuestion.Id}/choices",
            new CreateQuizChoiceRequest("Hello", true, null, "Correct.", 0));
        Assert.Equal(HttpStatusCode.Created, correctChoiceResponse.StatusCode);
        var correctChoice = await correctChoiceResponse.Content.ReadFromJsonAsync<QuizChoiceDto>();

        var wrongChoiceResponse = await client.PostAsJsonAsync(
            $"/api/v1/quiz-questions/{firstQuestion.Id}/choices",
            new CreateQuizChoiceRequest("Table", false, null, "Not a greeting.", 1));
        Assert.Equal(HttpStatusCode.Created, wrongChoiceResponse.StatusCode);
        var wrongChoice = await wrongChoiceResponse.Content.ReadFromJsonAsync<QuizChoiceDto>();

        var duplicateCorrectResponse = await client.PostAsJsonAsync(
            $"/api/v1/quiz-questions/{firstQuestion.Id}/choices",
            new CreateQuizChoiceRequest("Hi", true, null, null, 2));
        Assert.Equal(HttpStatusCode.BadRequest, duplicateCorrectResponse.StatusCode);

        var choices = await client.GetFromJsonAsync<IReadOnlyCollection<QuizChoiceDto>>(
            $"/api/v1/quiz-questions/{firstQuestion.Id}/choices");
        Assert.Equal(2, choices!.Count);

        var reorderChoicesResponse = await client.PostAsJsonAsync(
            $"/api/v1/quiz-questions/{firstQuestion.Id}/choices/reorder",
            new ReorderQuizChoicesRequest([wrongChoice!.Id, correctChoice!.Id]));
        Assert.Equal(HttpStatusCode.OK, reorderChoicesResponse.StatusCode);
        var reorderedChoices = await reorderChoicesResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<QuizChoiceDto>>();
        Assert.Equal(wrongChoice.Id, reorderedChoices!.First().Id);

        var reorderQuestionsResponse = await client.PostAsJsonAsync(
            $"/api/v1/quizzes/{quiz.Id}/questions/reorder",
            new ReorderQuizQuestionsRequest([secondQuestion!.Id, firstQuestion.Id]));
        Assert.Equal(HttpStatusCode.OK, reorderQuestionsResponse.StatusCode);
        var reorderedQuestions = await reorderQuestionsResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<QuizQuestionDto>>();
        Assert.Equal(secondQuestion.Id, reorderedQuestions!.First().Id);

        var publishResponse = await client.PostAsync($"/api/v1/quizzes/{quiz.Id}/publish", content: null);
        Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);
        var publishedQuiz = await publishResponse.Content.ReadFromJsonAsync<QuizDto>();
        Assert.True(publishedQuiz!.IsPublished);

        var unpublishResponse = await client.PostAsync($"/api/v1/quizzes/{quiz.Id}/unpublish", content: null);
        Assert.Equal(HttpStatusCode.OK, unpublishResponse.StatusCode);
        var unpublishedQuiz = await unpublishResponse.Content.ReadFromJsonAsync<QuizDto>();
        Assert.False(unpublishedQuiz!.IsPublished);

        var updateQuizResponse = await client.PutAsJsonAsync(
            $"/api/v1/quizzes/{quiz.Id}",
            new UpdateQuizRequest(
                "Starter Quiz Integration Updated",
                "Check beginner knowledge",
                "A short admin quiz.",
                "A1",
                null,
                null,
                null,
                null,
                20,
                80,
                1,
                false,
                true));
        Assert.Equal(HttpStatusCode.OK, updateQuizResponse.StatusCode);
        var updatedQuiz = await updateQuizResponse.Content.ReadFromJsonAsync<QuizDto>();
        Assert.Equal("Starter Quiz Integration Updated", updatedQuiz!.Title);

        var deleteChoiceResponse = await client.DeleteAsync($"/api/v1/quiz-choices/{wrongChoice.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteChoiceResponse.StatusCode);

        var deleteQuestionResponse = await client.DeleteAsync($"/api/v1/quiz-questions/{secondQuestion.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteQuestionResponse.StatusCode);

        var deleteQuizResponse = await client.DeleteAsync($"/api/v1/quizzes/{quiz.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteQuizResponse.StatusCode);

        var activeQuizSearch = await client.GetFromJsonAsync<QuizSearchResponse>(
            "/api/v1/quizzes?search=Updated");
        Assert.DoesNotContain(activeQuizSearch!.Items, item => item.Id == quiz.Id);

        var inactiveQuizSearch = await client.GetFromJsonAsync<QuizSearchResponse>(
            "/api/v1/quizzes?search=Updated&isActive=false");
        Assert.Contains(inactiveQuizSearch!.Items, item => item.Id == quiz.Id);
    }

    [Fact]
    public async Task CreateQuizReturnsValidationProblemWhenTitleIsMissing()
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/quizzes",
            new CreateQuizRequest(" ", null, null, null, null, null, null, null, 0, 70, 0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateQuizReturnsValidationProblemWhenTitleAlreadyExists()
    {
        var title = $"Duplicate Quiz Title {Guid.NewGuid():N}";
        var firstResponse = await client.PostAsJsonAsync(
            "/api/v1/quizzes",
            new CreateQuizRequest(title, null, null, null, null, null, null, null, 0, 70, 0));
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await client.PostAsJsonAsync(
            "/api/v1/quizzes",
            new CreateQuizRequest(title, null, null, null, null, null, null, null, 0, 70, 1));

        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }
}
