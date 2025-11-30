using CoreWCF.Server.REST.Services;

namespace CoreWCF.Server.REST.Middleware;

public sealed class SoapRoutingMiddleware(RequestDelegate next, ILogger<SoapRoutingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/CatInformationService") && 
            context.Request.Method == "POST")
        {
            var operation = await SoapRequestOperationExtractor.GetSoapOperation(context);
            
            // Rewrite the path based on the SOAPOperation
            if (!string.IsNullOrEmpty(operation))
            {
                string newPath = operation switch
                {
                    "GetPhoto" => "/CatInformationService/GetPhoto",
                    "GetCatTypes" => "/CatInformationService/GetCatTypes",
                    _ => context.Request.Path
                };

                context.Request.Path = newPath;
                logger.LogInformation("Rewriting path from {OriginalPath} to {NewPath} for operation {Operation}",
                    "/CatInformationService", newPath, operation);
            }
        }

        await next(context);
    }
}