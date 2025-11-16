using System.Xml.Serialization;
using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;
using CoreWCF.Server.REST.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddTransient<ICatInformationService, CatInformationService>();
builder.Services.AddTransient<GetPhotoHandler>();
builder.Services.AddTransient<CatTypesRequestHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/CatInformationService", async (HttpContext httpContext,
    [FromServices]GetPhotoHandler photoHandler, 
        [FromServices]CatTypesRequestHandler catTypesHandler) =>
    {
        using var reader = new StreamReader(httpContext.Request.Body);
        var soapRequest = await reader.ReadToEndAsync();
        var operation = SoapRequestOperationExtractor.GetSoapOperation(soapRequest);

        string? soapResponse;
        
        if (operation == "GetPhoto")
        {
            var photoResponse = photoHandler.GetPhoto();
            soapResponse = SoapResponseBuilder.GetSOAPResponseAsync(photoResponse);
            return Results.Content(soapResponse, "text/xml");
        }

        var xmlSerializer = new XmlSerializer(typeof(GetCatTypesRequest));
        var getCatTypesRequest = (GetCatTypesRequest)xmlSerializer.Deserialize(new StringReader(soapRequest));

        var catTypesResponse = catTypesHandler.GetCatTypes(getCatTypesRequest);
        soapResponse = SoapResponseBuilder.GetSOAPResponseAsync(catTypesResponse);
        return Results.Content(soapResponse, "text/xml");

    })
.WithName("CatInformationService");

app.Run();