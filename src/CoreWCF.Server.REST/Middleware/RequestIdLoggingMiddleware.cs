using System.Text;
using System.Xml;
using CoreWCF.Server.REST.Services;

namespace CoreWCF.Server.REST.Middleware;

public class RequestIdLoggingMiddleware(RequestDelegate next, ILogger<RequestIdLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var soapRequest = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        
        var operation = await SoapRequestOperationExtractor.GetSoapOperation(context);
        
        if (context.Request.Method == "POST" && operation == "GetCatTypes")
        {
            var requestId = ExtractRequestId(soapRequest);
            
            if (!string.IsNullOrWhiteSpace(requestId))
            {
                logger.LogInformation("Received RequestId: {RequestId} for operation: {Operation}", requestId, operation);
            }
        }

        await next(context);
    }

    private string? ExtractRequestId(string soapRequest)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapRequest);

            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tem", "http://tempuri.org/");
            
            //     <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
            //         <s:Header>
            //             <h:CatLoverHeader xmlns:h="http://tempuri.org/">I love cats!</h:CatLoverHeader>
            //         </s:Header>
            //         <s:Body>
            //             <GetCatTypesRequest xmlns="http://tempuri.org/">
            //                 <LikesChildren>false</LikesChildren>
            //                 <RequestId>ecea18b4-724b-4f5f-ac3a-f7e01b7a0fa9</RequestId>
            //                 <RequestTimestamp>2025-11-24T09:50:59.9577556Z</RequestTimestamp>
            //             </GetCatTypesRequest>
            //         </s:Body>
            //     </s:Envelope>
            // 
            var node = xmlDoc.SelectSingleNode("//soapenv:Body/tem:GetCatTypesRequest/tem:RequestId", nsmgr);

            return node?.InnerText;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting RequestId from SOAP request");
            return null;
        }
    }
}