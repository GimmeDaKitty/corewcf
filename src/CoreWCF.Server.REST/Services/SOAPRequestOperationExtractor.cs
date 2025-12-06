namespace CoreWCF.Server.REST.Services;

public static class SoapRequestOperationExtractor
{
    public static async Task<string> GetSoapOperation(HttpContext httpContext)
    {
        var soapAction = httpContext.Request.Headers["SOAPAction"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(soapAction))
        {
            throw new InvalidOperationException("Incoming request is not a valid SOAP request: Missing SOAPAction header.");
        }

        var operationName = soapAction
            .Trim('"')
            .Split('/')
            .LastOrDefault();
        
        return !string.IsNullOrWhiteSpace(operationName) 
            ? operationName
            : throw new InvalidOperationException($"Could not extract operation name from SOAPAction: {soapAction}");
    }
}