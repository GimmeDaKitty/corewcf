using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreWCF.Server.REST.Services;

public sealed class CatFactsService(IHttpClientFactory httpClientFactory) : ICatFactsService
{
    public async Task<CatFactResponse> GetCatFactAsync(GetCatFactRequest request)
    {
        var httpClient = httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync("https://catfact.ninja/fact");
        response.EnsureSuccessStatusCode();
        var json = response.Content.ReadAsStringAsync().Result;
        var catFactData = JsonSerializer.Deserialize<CatFactApiResponse>(json);

        return new CatFactResponse
        {
            Fact = catFactData?.Fact ?? "No cat fact available"
        };
    }

    private class CatFactApiResponse
    {
        [JsonPropertyName("fact")] public string Fact { get; set; }

        [JsonPropertyName("length")] public int Length { get; set; }
    }
}