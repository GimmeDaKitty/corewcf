using System.Text;
using System.Xml;
using CoreWCF.Server.REST.Services;

namespace CoreWCF.Server.REST.Middleware;

public class SoapHeaderLoggingMiddleware(RequestDelegate next, ILogger<SoapHeaderLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var soapRequest = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        
        var operation = SoapRequestOperationExtractor.GetSoapOperation(soapRequest);
        
        if (context.Request.Method == "POST" && operation == "GetCatTypes")
        {
            var catLoverHeader = ExtractCatLoverHeader(soapRequest);
            
            if (!string.IsNullOrWhiteSpace(catLoverHeader))
            {
                logger.LogInformation("CatLoverHeader found in SOAP request: {CatLoverHeader}", catLoverHeader);
            }
        }

        await next(context);
    }

    private string? ExtractCatLoverHeader(string soapRequest)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapRequest);

            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tem", "http://tempuri.org/");

            var headerNode = xmlDoc.SelectSingleNode("//s:Header/tem:CatLoverHeader", nsmgr) 
                          ?? xmlDoc.SelectSingleNode("//soapenv:Header/tem:CatLoverHeader", nsmgr);

            return headerNode?.InnerText;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting CatLoverHeader from SOAP request");
            return null;
        }
    }
}

