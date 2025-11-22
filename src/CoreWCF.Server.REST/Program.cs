using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;
using CoreWCF.Server.REST;
using CoreWCF.Server.REST.Filters;
using CoreWCF.Server.REST.Middleware;
using CoreWCF.Server.REST.RestWrappers;
using CoreWCF.Server.REST.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddTransient<ICatInformationService, CatInformationService>();

// Register request filters for use in controllers
builder.Services.AddScoped<SoapAuthorizationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<SoapAuthorizationFilter>();
    
    options.InputFormatters.RemoveType<XmlSerializerInputFormatter>();
    options.OutputFormatters.RemoveType<XmlSerializerOutputFormatter>();
    
    var xmlInputFormatter = new XmlSerializerInputFormatter(options);
    xmlInputFormatter.SupportedMediaTypes.Clear();
    xmlInputFormatter.SupportedMediaTypes.Add("text/xml");
    xmlInputFormatter.SupportedMediaTypes.Add("application/xml");
    options.InputFormatters.Add(xmlInputFormatter);

    var xmlOutputFormatter = new XmlSerializerOutputFormatter();
    xmlOutputFormatter.SupportedMediaTypes.Clear();
    xmlOutputFormatter.SupportedMediaTypes.Add("text/xml");
    xmlOutputFormatter.SupportedMediaTypes.Add("application/xml");
    options.OutputFormatters.Add(xmlOutputFormatter);
    
    //options.FormatterMappings.SetMediaTypeMappingForFormat("xml", "text/xml");
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<SoapHeaderLoggingMiddleware>();
app.UseMiddleware<SoapRoutingMiddleware>();
app.UseRouting();

app.UseHttpsRedirection();

// TODO - BEA - RECUPERA
// LESS BAD - REQUIRES ROUTING
// app.MapPost("/CatInformationService/GetPhoto", async (
//     [FromServices] ICatInformationService catInformationService) =>
// {
//     var photo = catInformationService.GetPhoto();
//     var response = new SoapEnvelope<GetPhotoResponse>
//     {
//         ResponseBody = new SoapRequestBody<GetPhotoResponse>
//         {
//             Res = new GetPhotoResponse
//             {
//                 GetPhotoResult = photo
//             }
//         }
//     };
//
//     return await ResponseXmlSerializer.Serialize(response);
// });


// BETTER BUT DOES NOT WORK - Using SoapEnvelope wrapper for automatic deserialization
// app.MapPost("/CatInformationService/GetCatTypes", async (
//     [FromBody] SoapEnvelope<GetCatTypesRequestWrapper> envelope,
//     [FromServices] ICatInformationService catInformationService) =>
// {
//     var response = new SoapEnvelope<GetCatTypesResponseWrapper>
//     {
//         Body = new SoapBody<GetCatTypesResponse>
//         {
//             Response = catInformationService.GetCatTypes(envelope.Body.Request)
//         }
//     };
//     
//     return await ResponseXmlSerializer.Serialize(response);
// })
// .AddEndpointFilter<SoapAuthorizationFilter>();


// VERY BAD
// app.MapPost("/CatInformationService", async (HttpContext httpContext, 
//         [FromServices]ICatInformationService catInformationService) =>
//     {
//         using var reader = new StreamReader(httpContext.Request.Body);
//         var soapRequest = await reader.ReadToEndAsync();
//         var operation = SoapRequestOperationExtractor.GetSoapOperation(soapRequest);
//
//         string? soapResponse;
//         
//         if (operation == "GetPhoto")
//         {
//             var photoResponse = new GetPhotoResponse
//             {
//                 GetPhotoResult = catInformationService.GetPhoto()
//             };
//             
//             soapResponse = SoapResponseBuilder.GetSOAPResponse(photoResponse);
//             
//             return Results.Content(soapResponse, "text/xml");
//         }
//         
//         // Extract the body content from the SOAP envelope
//         var xmlDoc = new System.Xml.XmlDocument();
//         xmlDoc.LoadXml(soapRequest);
//         
//         var namespaceManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
//         namespaceManager.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
//         namespaceManager.AddNamespace("tem", "http://tempuri.org/");
//         
//         var bodyNode = xmlDoc.SelectSingleNode("//soapenv:Body/tem:GetCatTypesRequest", namespaceManager);
//         
//         if (bodyNode == null)
//         {
//             return Results.BadRequest("Invalid SOAP request");
//         }
//         // get header CatLoverHeader under SOAP header
//         var headerNode = xmlDoc.SelectSingleNode("//soapenv:Header/tem:CatLoverHeader", namespaceManager);
//         var catLoverHeaderValue = headerNode?.InnerText;
//
//         var xmlSerializer = new XmlSerializer(
//             typeof(GetCatTypesRequest),
//             new XmlRootAttribute("GetCatTypesRequest")
//             {
//                 Namespace = "http://tempuri.org/"
//             });
//         using var stringReader = new StringReader(bodyNode.OuterXml);
//         var getCatTypesRequest = (GetCatTypesRequest)xmlSerializer.Deserialize(stringReader);
//         getCatTypesRequest.CatLoverHeader = catLoverHeaderValue;
//         var catTypesResponse = catInformationService.GetCatTypes(getCatTypesRequest);
//         soapResponse = SoapResponseBuilder.GetSOAPResponse(catTypesResponse);
//         return Results.Content(soapResponse, "text/xml");
//
//     })
//     .AddEndpointFilter<SoapAuthorizationFilter>() 
//     .WithName("CatInformationService");
app.MapControllers();
app.Run();