using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreWCF.Server.REST.Filters;

// Implementing endpointFilters and AuthorizationFilters in the same class for conciseness
public sealed class CatLoverHeaderAuthorizationFilter(ILogger<CatLoverHeaderAuthorizationFilter> logger) 
    : IEndpointFilter, 
    IAsyncAuthorizationFilter
{
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
        var authToken = ExtractCatLoverHeader(soapRequest);
        
        if (string.IsNullOrWhiteSpace(authToken))
        {
            logger.LogError("Authorization filter: I'm sorry this API is only for true cat lovers");
            return false;
        }
        
        return true;
    }
    
    private string? ExtractCatLoverHeader(string soapRequest)
    {
        try
        {
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(soapRequest);

            var nsmgr = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tem", "http://tempuri.org/");

            var tokenNode = xmlDoc.SelectSingleNode("//soapenv:Header/tem:CatLoverHeader", nsmgr);
            return tokenNode?.InnerText;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting Cat Lover header from SOAP request");
            return null;
        }
    }
}