using System.Xml.Serialization;
using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;
using CoreWCF.Server.REST;
using CoreWCF.Server.REST.Middleware;
using CoreWCF.Server.REST.RestWrappers;
using CoreWCF.Server.REST.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddTransient<ICatInformationService, CatInformationService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<SoapHeaderLoggingMiddleware>();
//app.UseMiddleware<SoapRoutingMiddleware>();
//app.UseRouting();

app.UseHttpsRedirection();

// LESS BAD - REQUIRES ROUTING
// app.MapPost("/CatInformationService/GetPhoto", async (
//     [FromServices] ICatInformationService catInformationService) =>
// {
//     var photo = catInformationService.GetPhoto();
//     var response = new SoapEnvelope<GetPhotoResponse>
//     {
//         Body = new SoapBody<GetPhotoResponse>
//         {
//             Response = new GetPhotoResponse
//             {
//                 GetPhotoResult = photo
//             }
//         }
//     };
//     
//     var xmlSerializer = new XmlSerializer(typeof(SoapEnvelope<GetPhotoResponse>));
//     await using var stringWriter = new StringWriter();
//     xmlSerializer.Serialize(stringWriter, response);
//
//     return Results.Content(stringWriter.ToString(), "text/xml; charset=utf-8");
// });
//
// app.MapPost("/CatInformationService/GetCatTypes", async (
//      [FromBody] SoapEnvelope<GetCatTypesRequest> envelope,
//     [FromServices] ICatInformationService catInformationService) 
//     => catInformationService.GetCatTypes(envelope.Body.Request));


// BETTER - Using SoapEnvelope wrapper for automatic deserialization
// app.MapPost("/CatInformationService/GetCatTypes", async (
//     [FromBody] SoapEnvelope<GetCatTypesRequest> envelope,
//     [FromServices] ICatInformationService catInformationService) =>
// {
//     if (envelope.Body.Request != null && envelope.Header != null)
//     {
//         envelope.Body.Request.CatLoverHeader = envelope.Header.CatLoverHeader;
//     }
//     return catInformationService.GetCatTypes(envelope.Body.Request);
// });


// VERY BAD
app.MapPost("/CatInformationService", async (HttpContext httpContext, 
        [FromServices]ICatInformationService catInformationService) =>
    {
        using var reader = new StreamReader(httpContext.Request.Body);
        var soapRequest = await reader.ReadToEndAsync();
        var operation = SoapRequestOperationExtractor.GetSoapOperation(soapRequest);

        string? soapResponse;
        
        if (operation == "GetPhoto")
        {
            var photoResponse = new GetPhotoResponse
            {
                GetPhotoResult = catInformationService.GetPhoto()
            };
            
            soapResponse = SoapResponseBuilder.GetSOAPResponse(photoResponse);
            
            return Results.Content(soapResponse, "text/xml");
        }
        
        // Extract the body content from the SOAP envelope
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
        var catTypesResponse = catInformationService.GetCatTypes(getCatTypesRequest);
        soapResponse = SoapResponseBuilder.GetSOAPResponse(catTypesResponse);
        return Results.Content(soapResponse, "text/xml");

    })
.WithName("CatInformationService");

app.Run();