using System.Text.Json;
using System.Text.Json.Serialization;
using CoreWCF.Contracts;

namespace CoreWCF.Server.Common.Services;

public sealed class CatFactsService(IHttpClientFactory httpClientFactory) : ICatFactsService
{
    public CatFactResponse GetCatFact()
    {
        var httpClient = httpClientFactory.CreateClient();
        var response = httpClient.GetAsync("https://catfact.ninja/fact").Result;
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
        [JsonPropertyName("fact")]
        public string Fact { get; set; }
        
        [JsonPropertyName("length")]
        public int Length { get; set; }
    }
}