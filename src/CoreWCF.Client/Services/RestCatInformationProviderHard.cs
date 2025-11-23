using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

// You are valuable. You deserve love, friends and free time. Do not this.
public class RestCatInformationProviderHard(IHttpClientFactory httpClientFactory) : ICatInformationProvider
{
    private const string GetPhotoSoapAction = "http://tempuri.org/ICatInformationService/GetPhoto";
    private const string GetCatTypesSoapAction = "http://tempuri.org/ICatInformationService/GetCatTypes";
    private const string AttemptBellyRubSoapAction = "http://tempuri.org/ICatInformationService/AttemptBellyRub";
    
    public async Task<Result<byte[]>> GetCatPictureAsync()
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(ClientConstants.CatInformationClientName);

            var soapRequest = SoapRequestBuilder.BuildGetCatPhotoRequest();
            
            var content = new StringContent(soapRequest, System.Text.Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", GetPhotoSoapAction);
            
            var response = await httpClient.PostAsync("/CatInformationService", content);
            
            if (response.IsSuccessStatusCode)
            {
                var photoResponse = await SoapResponseBuilder.GetResponseAsync<GetPhotoResponse>("GetPhotoResponse", response);
                return Result<byte[]>.OkResult(photoResponse.GetPhotoResult);
            }
            
            return Result<byte[]>.NOkResult($"Remote API returned error: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            return Result<byte[]>.NOkResult($"Error while processing request: {ex.Message}");
        }
    }

    public async Task<Result<CatType[]>> GetCatTypes(bool containsHeader, bool onlyCatsThatLikeChildren)
    {
        try
        {
            var catLoverHeader = containsHeader ? "I love cats!" : null;
            
            var httpClient = httpClientFactory.CreateClient(ClientConstants.CatInformationClientName);

            var soapRequest = SoapRequestBuilder.BuildGetCatTypesRequest(catLoverHeader, onlyCatsThatLikeChildren);
            
            var content = new StringContent(soapRequest, System.Text.Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", GetCatTypesSoapAction);
            
            var response = await httpClient.PostAsync("/CatInformationService", content);
            
            if (response.IsSuccessStatusCode)
            {
                var soapresponse = await SoapResponseBuilder.GetResponseAsync<GetCatTypesResponse>("GetCatTypesResponse", response);
                return Result<CatType[]>.OkResult(soapresponse.CatTypes);
            }
            
            var responseHasContent = response.Content.Headers.ContentLength > 0;
            var errorMessage = responseHasContent
                ? await response.Content.ReadAsStringAsync()
                : $"response code {(int)response.StatusCode} - {response.StatusCode}";
            
            return Result<CatType[]>.NOkResult($"Remote API returned error: {errorMessage}");
        }
        catch (Exception ex)
        {
            return Result<CatType[]>.NOkResult($"Error while processing request: {ex.Message}");
        }
    }

    public async Task<Result> CanPetTheCatAsync(HumanType humanType)
    {
        try
        {
            var token = FakeJwtTokenGenerator.GenerateToken(humanType);
            
            var httpClient = httpClientFactory.CreateClient(ClientConstants.CatInformationClientName);

            var soapRequest = SoapRequestBuilder.BuildAttemptBellyRubRequest();
            
            var content = new StringContent(soapRequest, System.Text.Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", AttemptBellyRubSoapAction);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/CatInformationService");
            requestMessage.Headers.Add("Authorization", $"Bearer {token}");
            requestMessage.Content = content;
            var response = await httpClient.SendAsync(requestMessage);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var soapresponse = await SoapResponseBuilder.GetResponseAsync<BellyRubResponse>("BellyRubResponse", response);
                return soapresponse.Allowed
                    ? Result.Ok
                    : Result.NOk("You are not allowed to pet the cat.");
            }
            
            var responseHasContent = response.Content.Headers.ContentLength > 0;
            var errorMessage = responseHasContent
                ? await response.Content.ReadAsStringAsync()
                : $"response code {(int)response.StatusCode} - {response.StatusCode}";
            
            return Result<bool>.NOkResult($"Remote API returned error: {errorMessage}");
        }
        catch (Exception ex)
        {
            return Result<bool>.NOkResult($"Error while processing request: {ex.Message}");
        }
    }
}

