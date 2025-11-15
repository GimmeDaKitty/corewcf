using CoreWCF.Client.Data;

namespace CoreWCF.Client.Services;

// You are valuable. You deserve love, friends and free time. Do not this.
public class CatInformationProviderHard(IHttpClientFactory httpClientFactory) : ICatInformationProvider
{
    private const string GetCatFactAction = "http://tempuri.org/ICatFactsService/GetCatFact";

    public async Task<Result<string>> GetCatFactAsync()
    {
        try
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/CatFactsService");
            requestMessage.Headers.Add("SOAPAction", GetCatFactAction);
            requestMessage.Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "text/xml");
            
            var httpClient = httpClientFactory.CreateClient(ClientConstants.CatInformationClientName);

            var response = await httpClient.SendAsync(requestMessage);
            
            if (response.IsSuccessStatusCode)
            {
                var catFact = await SoapResponseBuilder.GetResponseAsync<CatFactResponse>("CatFactResponse", response);
                return Result<string>.OkResult(catFact.Fact);
            }
            
            return Result<string>.NOkResult($"Remote API returned error: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            return Result<string>.NOkResult($"Error while processing request: {ex.Message}");
        }    
    }
}

