using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Media;
using EnglishMaster.Contracts.MinimalPairs;
using EnglishMaster.Contracts.Pronunciations;
using EnglishMaster.Contracts.Words;

namespace EnglishMaster.IntegrationTests.Pronunciations;

public sealed class PronunciationEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public PronunciationEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task PronunciationEndpointsSupportCreateGetSearchUpdateMinimalPairsAndDelete()
    {
        var word = await CreateWordAsync("pronunciation hello");
        var audio = await CreateMediaAsync("pronunciation-hello.mp3", "Audio", "audio/mpeg");
        var image = await CreateMediaAsync("pronunciation-mouth.jpg", "Image", "image/jpeg");

        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/pronunciations",
            new CreatePronunciationRequest(
                word.Id,
                "/hallo/",
                "/hello/",
                "heh-lo",
                "hel-lo",
                "first syllable",
                "mouth open",
                "tongue low",
                "avoid silent h",
                "practice slowly",
                audio.Id,
                audio.Id,
                image.Id));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var pronunciation = await createResponse.Content.ReadFromJsonAsync<PronunciationDto>();
        Assert.NotNull(pronunciation);
        Assert.Equal(word.Id, pronunciation.WordId);
        Assert.Equal(audio.Id, pronunciation.AudioSlowMediaId);
        Assert.Equal(image.Id, pronunciation.MouthImageMediaId);

        var duplicateResponse = await client.PostAsJsonAsync(
            "/api/v1/pronunciations",
            new CreatePronunciationRequest(word.Id, "/again/", null, null, null, null, null, null, null, null));
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);

        var byWord = await client.GetFromJsonAsync<PronunciationDto>(
            $"/api/v1/words/{word.Id}/pronunciation");
        Assert.Equal(pronunciation.Id, byWord!.Id);

        var wordDetail = await client.GetFromJsonAsync<WordDto>($"/api/v1/words/{word.Id}");
        Assert.Equal(pronunciation.Id, wordDetail!.Pronunciation!.Id);
        Assert.Equal("/hallo/", wordDetail.Pronunciation.IpaUk);

        var search = await client.GetFromJsonAsync<PronunciationSearchResponse>(
            "/api/v1/pronunciations?search=hallo");
        Assert.Contains(search!.Items, item => item.Id == pronunciation.Id);

        var pairResponse = await client.PostAsJsonAsync(
            $"/api/v1/pronunciations/{pronunciation.Id}/minimal-pairs",
            new CreateMinimalPairRequest(
                "yellow",
                "/jellow/",
                "yel-low",
                "initial y sound",
                audio.Id,
                1));
        Assert.Equal(HttpStatusCode.Created, pairResponse.StatusCode);
        var pair = await pairResponse.Content.ReadFromJsonAsync<MinimalPairDto>();
        Assert.NotNull(pair);
        Assert.Equal(pronunciation.Id, pair.PronunciationId);

        var pairs = await client.GetFromJsonAsync<IReadOnlyCollection<MinimalPairDto>>(
            $"/api/v1/pronunciations/{pronunciation.Id}/minimal-pairs");
        Assert.Contains(pairs!, item => item.Id == pair.Id);

        var wordDetailWithPair = await client.GetFromJsonAsync<WordDto>($"/api/v1/words/{word.Id}");
        Assert.Contains(
            wordDetailWithPair!.Pronunciation!.MinimalPairs,
            item => item.Id == pair.Id && item.IsActive);

        var updatePairResponse = await client.PutAsJsonAsync(
            $"/api/v1/minimal-pairs/{pair.Id}",
            new UpdateMinimalPairRequest(
                "jello",
                "/jello/",
                "jel-lo",
                "soft j sound",
                audio.Id,
                2,
                true));
        Assert.Equal(HttpStatusCode.OK, updatePairResponse.StatusCode);
        var updatedPair = await updatePairResponse.Content.ReadFromJsonAsync<MinimalPairDto>();
        Assert.Equal("jello", updatedPair!.PairWordText);
        Assert.Equal(2, updatedPair.SortOrder);

        var deletePairResponse = await client.DeleteAsync($"/api/v1/minimal-pairs/{pair.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deletePairResponse.StatusCode);

        var wordDetailAfterPairDelete = await client.GetFromJsonAsync<WordDto>($"/api/v1/words/{word.Id}");
        Assert.DoesNotContain(
            wordDetailAfterPairDelete!.Pronunciation!.MinimalPairs,
            item => item.Id == pair.Id);

        var updatePronunciationResponse = await client.PutAsJsonAsync(
            $"/api/v1/pronunciations/{pronunciation.Id}",
            new UpdatePronunciationRequest(
                word.Id,
                "/hullo/",
                "/hello/",
                "heh-lo",
                "hel-lo",
                "first syllable",
                "mouth open",
                "tongue low",
                "avoid silent h",
                "practice daily",
                audio.Id,
                audio.Id,
                image.Id,
                true));
        Assert.Equal(HttpStatusCode.OK, updatePronunciationResponse.StatusCode);
        var updatedPronunciation = await updatePronunciationResponse.Content.ReadFromJsonAsync<PronunciationDto>();
        Assert.Equal("/hullo/", updatedPronunciation!.IpaUk);

        var deleteResponse = await client.DeleteAsync($"/api/v1/pronunciations/{pronunciation.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var activeSearch = await client.GetFromJsonAsync<PronunciationSearchResponse>(
            "/api/v1/pronunciations?search=hullo");
        Assert.DoesNotContain(activeSearch!.Items, item => item.Id == pronunciation.Id);

        var inactiveSearch = await client.GetFromJsonAsync<PronunciationSearchResponse>(
            "/api/v1/pronunciations?search=hullo&isActive=false");
        Assert.Contains(inactiveSearch!.Items, item => item.Id == pronunciation.Id);
    }

    [Fact]
    public async Task CreateReturnsValidationProblemWhenIpaIsMissing()
    {
        var word = await CreateWordAsync("pronunciation validation");

        var response = await client.PostAsJsonAsync(
            "/api/v1/pronunciations",
            new CreatePronunciationRequest(word.Id, null, null, null, null, null, null, null, null, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<WordDto> CreateWordAsync(string text)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/words",
            new CreateWordRequest(
                text,
                null,
                null,
                null,
                "Thai meaning",
                text,
                "Noun",
                "A1",
                null,
                null));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<WordDto>())!;
    }

    private async Task<MediaDto> CreateMediaAsync(
        string fileName,
        string mediaType,
        string contentType)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/media",
            new CreateMediaRequest(
                fileName,
                fileName,
                Path.GetExtension(fileName),
                contentType,
                128,
                mediaType,
                $"/media/{fileName}",
                $"{fileName} alt",
                $"{fileName} description"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<MediaDto>())!;
    }
}
