using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreWCF.Server.REST.Filters;

// Implementing endpointFilters and AuthorizationFilters in the same class for conciseness
public sealed class SoapAuthorizationFilter(ILogger<SoapAuthorizationFilter> logger) : IEndpointFilter, IAsyncAuthorizationFilter
{
    private const string ValidToken = "SecretCatToken123";

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var isAuthorized = await IsAuthorized(context.HttpContext);
        
        if (!isAuthorized)
        {
            return Results.Unauthorized();
        }
        
        return await next(context);
    }
    
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var isAuthorized = await IsAuthorized(context.HttpContext);
        
        if (!isAuthorized)
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private async Task<bool> IsAuthorized(HttpContext httpContext)
    {
        // Enable buffering to read the body multiple times
        httpContext.Request.EnableBuffering();

        string soapRequest;
        using (var reader = new StreamReader(
                   httpContext.Request.Body, 
                   System.Text.Encoding.UTF8, 
                   detectEncodingFromByteOrderMarks: false, 
                   leaveOpen: true))
        {
            soapRequest = await reader.ReadToEndAsync();
            httpContext.Request.Body.Position = 0;
        }

        // Extract authorization token from SOAP header
        var authToken = ExtractAuthToken(soapRequest);
        
        // TODO - BEA - IMPLEMENT AUTHORIZATION LOGIC
        // TODO - RETURN 401 INSTEAD OF 500 AND REFLECT IN CLIENT
        // TODO - IMPLEMENT ALSO IN COREWCF
        // if (string.IsNullOrWhiteSpace(authToken))
        // {
        //     logger.LogWarning("Authorization failed: No auth token found in SOAP header");
        //     return false;
        // }
        //
        // if (authToken != ValidToken)
        // {
        //     logger.LogWarning("Authorization failed: Invalid token provided");
        //     return false;
        // }
        //
        // logger.LogInformation("Authorization successful");
        return true;
    }
    
    private string? ExtractAuthToken(string soapRequest)
    {
        try
        {
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(soapRequest);

            var nsmgr = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tem", "http://tempuri.org/");

            var tokenNode = xmlDoc.SelectSingleNode("//soapenv:Header/tem:AuthToken", nsmgr);
            return tokenNode?.InnerText;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting auth token from SOAP request");
            return null;
        }
    }
}
