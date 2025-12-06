using System.Xml.Serialization;
using CoreWCF.Server.REST.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Services
builder.Services.AddHttpClient();
builder.Services.AddTransient<ICatFactsService, CatFactsService>();
builder.Services.AddTransient<ICatInformationService, CatInformationService>();

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();

app.MapPost("/CatFactsService", async (
    [FromServices] ICatFactsService catFactsService) =>
{
    // No request, so no need to read the body
    var catFactResponse = await catFactsService.GetCatFactAsync(new GetCatFactRequest());
    var xmlResponse = SoapResponseEnvelopeBuilder.GetCatFactResponse(catFactResponse.Fact);
    return Results.Content(xmlResponse, "text/xml");
});

app.MapPost("/CatInformationService", async (HttpContext httpContext, 
        [FromServices]ICatInformationService catInformationService) =>
    {
        using var reader = new StreamReader(httpContext.Request.Body);
        var soapRequest = await reader.ReadToEndAsync();
        var operation = SoapRequestOperationExtractor.GetSoapOperation(soapRequest);

        string? soapResponse;
        
        if (operation == "GetPhoto")
        {
            // No need to deserialize because it's an empty request
            var photo = await catInformationService.GetPhotoAsync(new GetPhotoRequest());
            soapResponse = SoapResponseEnvelopeBuilder.GetSOAPPhotoResponse(photo.GetPhotoResult);
            return Results.Content(soapResponse, "text/xml");
        }

        if (operation == "GetCatTypes")
        {
            // Deserialize the request
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(soapRequest);
        
            var namespaceManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            namespaceManager.AddNamespace("tem", "http://tempuri.org/");
        
            var bodyNode = xmlDoc.SelectSingleNode("//soapenv:Body/tem:GetCatTypesRequest", namespaceManager);
        
            if (bodyNode == null)
            {
                return Results.BadRequest("Invalid SOAP request");
            }
        
            // get header CatLoverHeader under SOAP header
            var headerNode = xmlDoc.SelectSingleNode("//soapenv:Header/tem:CatLoverHeader", namespaceManager);
            var catLoverHeaderValue = headerNode?.InnerText;

            var xmlSerializer = new XmlSerializer(
                typeof(GetCatTypesRequest),
                new XmlRootAttribute("GetCatTypesRequest")
                {
                    Namespace = "http://tempuri.org/"
                });
        
            using var stringReader = new StringReader(bodyNode.OuterXml);
            var getCatTypesRequest = (GetCatTypesRequest)xmlSerializer.Deserialize(stringReader);
            getCatTypesRequest.CatLoverHeader = catLoverHeaderValue;
            var catTypesResponse = await catInformationService.GetCatTypesAsync(getCatTypesRequest);
        
            // Using the generic serializer, since the response already contains the SOAP envelope.
            soapResponse = SoapResponseEnvelopeBuilder.GetCatTypesResponse(catTypesResponse);
        
            return Results.Content(soapResponse, "text/xml");   
        }

        // If there are other operations, you need to handle them here (!)
        
        throw new InvalidOperationException("OPERATION NOT SUPPORTED");
    })
    .WithName("CatInformationService");

app.Run();