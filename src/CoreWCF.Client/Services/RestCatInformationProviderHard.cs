using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

// You are valuable. You deserve love, friends and free time. Do not this.
public class RestCatInformationProviderHard(IHttpClientFactory httpClientFactory) : ICatInformationProvider
{
    private const string GetPhotoSoapAction = "http://tempuri.org/ICatInformationService/GetPhoto";
    private const string GetCatTypesSoapAction = "http://tempuri.org/ICatInformationService/GetCatTypes";
    
    public async Task<byte[]?> GetCatPictureAsync()
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
                return await SoapResponseBuilder.GetResponseAsync<byte[]>("GetPhotoResult", response);
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<CatType[]> GetCatTypes(bool containsHeader, bool onlyCatsThatLikeChildren)
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
                return soapresponse.CatTypes;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}

