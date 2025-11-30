using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;
using CoreWCF.Server.REST.Filters;
using CoreWCF.Server.REST.Middleware;
using CoreWCF.Server.REST.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Services
builder.Services.AddTransient<ICatInformationService, CatInformationService>();
builder.Services.AddTransient<IBellyRubService, BellyRubService>();

// Controllers
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
});

// Authorization for controllers
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => 
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            RequireSignedTokens = false,
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes("this-is-a-super-secret-key-for-development-only-min-32-chars")
            )
        };
    });

// Authorization for minimal APIs
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("IsCoolHuman", policy =>
        policy.RequireClaim("iscoolhuman", "owner", "isalergic", "catlady"));

// Auth debugging
IdentityModelEventSource.ShowPII = true;
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware
app.UseMiddleware<SoapHeaderLoggingMiddleware>();
app.UseMiddleware<RequestIdLoggingMiddleware>();

// Routing for SOAP-as-REST
app.UseMiddleware<SoapRoutingMiddleware>();
app.UseRouting();

// Authentication
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

// -------------------------------------------------------------------

// LESS BAD - REQUIRES ROUTING - WORKS
app.MapPost("/CatInformationService/GetPhoto", (
    [FromServices] ICatInformationService catInformationService) =>
{
    var photo = catInformationService.GetPhoto();
    var soapResponse = SoapResponseEnvelopeBuilder.GetSOAPPhotoResponse(photo);
    return Results.Content(soapResponse, "text/xml; charset=utf-8");

});

app.MapPost("/BellyRubService", (
    [FromServices] IBellyRubService bellyRubService) =>
{
    var bellyRubAllowed = bellyRubService.AllowBellyRub();
    var soapResponse = SoapResponseEnvelopeBuilder.GetSOAPBellyRubResponse(bellyRubAllowed);
    return Results.Content(soapResponse, "text/xml; charset=utf-8");
})
.RequireAuthorization("IsCoolHuman");


// LESS BAD - REQUIRES ROUTING [DOES NOT WORK]
// app.MapPost("/CatInformationService/GetCatTypes", async (
//         [FromBody] GetCatTypesRequestEnvelope envelope, 
//         [FromServices] ICatInformationService catInformationService) =>
// {
//     envelope.Body.Request.CatLoverHeader = envelope.Header.CatLoverHeader;
//     var response = new GetCatTypesResponseEnvelope
//     {
//         Body = new GetCatTypesResponseBody
//         {
//             Response = catInformationService.GetCatTypes(envelope.Body.Request)
//         }
//     };
//     
//     return await GetCatTypesResponseSerializer.Serialize(response);
// })
// .AddEndpointFilter<SoapAuthorizationFilter>();

// VERY BAD
// app.MapPost("/CatInformationService", async (HttpContext httpContext, 
//         [FromServices]ICatInformationService catInformationService,
//         [FromServices]IBellyRubService bellyRubService) =>
//     {
//         using var reader = new StreamReader(httpContext.Request.Body);
//         var soapRequest = await reader.ReadToEndAsync();
//         var operation = SoapRequestOperationExtractor.GetSoapOperation(soapRequest);
//
//         string? soapResponse;
//         
//         if (operation == "GetPhoto")
//         {
//             var photo = catInformationService.GetPhoto();
//             soapResponse = SoapResponseEnvelopeBuilder.GetSOAPPhotoResponse(photo);
//             return Results.Content(soapResponse, "text/xml");
//         }
//         
//         if (operation == "AllowBellyRub")
//         {
//             var bellyRubAllowed = bellyRubService.AllowBellyRub();
//             soapResponse = SoapResponseEnvelopeBuilder.GetSOAPBellyRubResponse(bellyRubAllowed);
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
//         
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
//         
//         using var stringReader = new StringReader(bodyNode.OuterXml);
//         var getCatTypesRequest = (GetCatTypesRequest)xmlSerializer.Deserialize(stringReader);
//         getCatTypesRequest.CatLoverHeader = catLoverHeaderValue;
//         var serviceResponse = catInformationService.GetCatTypes(getCatTypesRequest);
//         
//         var response = new GetCatTypesResponseEnvelope
//         {
//             Body = new GetCatTypesResponseBody
//             {
//                 Response = serviceResponse
//             }
//         };
//         
//         // Using the generic serializer, since the response already contains the SOAP envelope.
//         soapResponse = await GenericSerializer.Serialize(response);
//         
//         return Results.Content(soapResponse, "text/xml");
//     })
//     .AddEndpointFilter<SoapAuthorizationFilter>() 
//     .WithName("CatInformationService");

// Controllers
app.MapControllers();

app.Run();