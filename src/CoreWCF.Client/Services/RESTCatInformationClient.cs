namespace CoreWCF.Client.Services;

public class RESTCatInformationClient(IHttpClientFactory httpClientFactory) : ICatInformationClient
{
    private const string SoapAction = "http://tempuri.org/ICatInformationService/GetPhoto";
    
    public async Task<byte[]?> GetCatPictureAsync()
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(ClientConstants.CatInformationClientName);
            
            var soapEnvelope = """
                <?xml version="1.0" encoding="utf-8"?>
                <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
                    <s:Body>
                        <GetPhoto xmlns="http://tempuri.org/" />
                    </s:Body>
                </s:Envelope>
                """;
            
            var content = new StringContent(soapEnvelope, System.Text.Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", SoapAction);
            
            var response = await httpClient.PostAsync("/CatInformationService", content);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}

