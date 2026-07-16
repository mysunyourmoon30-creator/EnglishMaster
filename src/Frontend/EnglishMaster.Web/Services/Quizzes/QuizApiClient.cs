using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Contracts.Quizzes;

namespace EnglishMaster.Web.Services.Quizzes;

public sealed class QuizApiClient : IQuizApiClient
{
    private readonly HttpClient httpClient;

    public QuizApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<QuizSearchResponse> SearchAsync(
        QuizSearchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildSearchEndpoint(request), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<QuizSearchResponse>(
            cancellationToken: cancellationToken) ?? new QuizSearchResponse(
                [],
                request.PageNumber,
                request.PageSize,
                0,
                0,
                false,
                false);
    }

    public async Task<QuizDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/quizzes/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<QuizDto>(cancellationToken: cancellationToken);
    }

    public async Task<QuizDto> CreateAsync(CreateQuizRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/quizzes", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<QuizDto>(response, cancellationToken);
    }

    public async Task<QuizDto> UpdateAsync(
        Guid id,
        UpdateQuizRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/quizzes/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<QuizDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/quizzes/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<QuizDto> PublishAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/quizzes/{id}/publish", content: null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<QuizDto>(response, cancellationToken);
    }

    public async Task<QuizDto> UnpublishAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/quizzes/{id}/unpublish", content: null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<QuizDto>(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<QuizQuestionDto>> GetQuestionsAsync(
        Guid quizId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/quizzes/{quizId}/questions", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<QuizQuestionDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    public async Task<QuizQuestionDto> AddQuestionAsync(
        Guid quizId,
        CreateQuizQuestionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/quizzes/{quizId}/questions",
            request,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<QuizQuestionDto>(response, cancellationToken);
    }

    public async Task<QuizQuestionDto> UpdateQuestionAsync(
        Guid id,
        UpdateQuizQuestionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync(
            $"api/v1/quiz-questions/{id}",
            request,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<QuizQuestionDto>(response, cancellationToken);
    }

    public async Task DeleteQuestionAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/quiz-questions/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<QuizQuestionDto>> ReorderQuestionsAsync(
        Guid quizId,
        IReadOnlyList<Guid> orderedQuestionIds,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/quizzes/{quizId}/questions/reorder",
            new ReorderQuizQuestionsRequest(orderedQuestionIds),
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<QuizQuestionDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    public async Task<IReadOnlyCollection<QuizChoiceDto>> GetChoicesAsync(
        Guid questionId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/quiz-questions/{questionId}/choices", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<QuizChoiceDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    public async Task<QuizChoiceDto> AddChoiceAsync(
        Guid questionId,
        CreateQuizChoiceRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/quiz-questions/{questionId}/choices",
            request,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<QuizChoiceDto>(response, cancellationToken);
    }

    public async Task<QuizChoiceDto> UpdateChoiceAsync(
        Guid id,
        UpdateQuizChoiceRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync(
            $"api/v1/quiz-choices/{id}",
            request,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<QuizChoiceDto>(response, cancellationToken);
    }

    public async Task DeleteChoiceAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/quiz-choices/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<QuizChoiceDto>> ReorderChoicesAsync(
        Guid questionId,
        IReadOnlyList<Guid> orderedChoiceIds,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/quiz-questions/{questionId}/choices/reorder",
            new ReorderQuizChoicesRequest(orderedChoiceIds),
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<QuizChoiceDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    private static string BuildSearchEndpoint(QuizSearchRequest request)
    {
        var parameters = new List<string>();
        Add(parameters, "search", request.Search);
        Add(parameters, "cefrLevel", request.CefrLevel);
        Add(parameters, "categoryId", request.CategoryId?.ToString());
        Add(parameters, "lessonId", request.LessonId?.ToString());
        Add(parameters, "courseId", request.CourseId?.ToString());
        Add(parameters, "bookId", request.BookId?.ToString());
        Add(parameters, "isPublished", request.IsPublished?.ToString().ToLowerInvariant());
        Add(parameters, "isActive", request.IsActive?.ToString().ToLowerInvariant());
        Add(parameters, "pageNumber", request.PageNumber.ToString());
        Add(parameters, "pageSize", request.PageSize.ToString());
        Add(parameters, "sortBy", request.SortBy);
        Add(parameters, "sortDirection", request.SortDirection);

        return parameters.Count == 0
            ? "api/v1/quizzes"
            : $"api/v1/quizzes?{string.Join("&", parameters)}";
    }

    private static void Add(ICollection<string> parameters, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        parameters.Add($"{name}={Uri.EscapeDataString(value.Trim())}");
    }
}
