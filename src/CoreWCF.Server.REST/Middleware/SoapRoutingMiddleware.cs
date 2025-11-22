namespace CoreWCF.Server.REST.Middleware;

public sealed class SoapRoutingMiddleware(RequestDelegate next, ILogger<SoapRoutingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/CatInformationService") && 
            context.Request.Method == "POST")
        {
            var operation = await ExtractSoapOperation(context);
            
            // Rewrite the path based on the SOAPOperation
            if (!string.IsNullOrEmpty(operation))
            {
                string newPath = operation switch
                {
                    "GetPhoto" => "/CatInformationService/GetPhoto",
                    "GetCatTypesRequest" => "/CatInformationService/GetCatTypes",
                    _ => context.Request.Path
                };

                context.Request.Path = newPath;
                logger.LogInformation("Rewriting path from {OriginalPath} to {NewPath} for operation {Operation}",
                    "/CatInformationService", newPath, operation);
            }
        }

        await next(context);
    }

    private async Task<string?> ExtractSoapOperation(HttpContext context)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var soapRequest = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        
        var xmlDoc = new System.Xml.XmlDocument();
        xmlDoc.LoadXml(soapRequest);

        var namespaceManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
        namespaceManager.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
        namespaceManager.AddNamespace("tem", "http://tempuri.org/");

        var bodyNode = xmlDoc.SelectSingleNode("//soapenv:Body/*[1]", namespaceManager);
        return bodyNode?.LocalName;
    }
}