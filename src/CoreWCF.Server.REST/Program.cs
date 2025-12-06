using CoreWCF.Server.REST.Middleware;
using CoreWCF.Server.REST.RestWrappers;
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

// Middleware
app.UseMiddleware<SoapRoutingMiddleware>();
app.UseRouting();

app.MapPost("/CatFactsService", async (
    [FromServices] ICatFactsService catFactsService) =>
{
    // No request, so no need to read the body
    var catFactResponse = await catFactsService.GetCatFactAsync(new GetCatFactRequest());
    var xmlResponse = SoapResponseEnvelopeBuilder.GetCatFactResponse(catFactResponse.Fact);
    return Results.Content(xmlResponse, "text/xml");
});

app.MapPost("/CatInformationService/GetPhoto", async(
    [FromServices] ICatInformationService catInformationService) =>
{
    var photo = await catInformationService.GetPhotoAsync(new GetPhotoRequest());
    var soapResponse = SoapResponseEnvelopeBuilder.GetSOAPPhotoResponse(photo.GetPhotoResult);
    return Results.Content(soapResponse, "text/xml");
});

app.MapPost("/CatInformationService/GetCatTypes", async (
    [FromBody] GetCatTypesRequestEnvelope envelope,
    [FromServices] ICatInformationService catInformationService) =>
{
    envelope.Body.Request.CatLoverHeader = envelope.Header.CatLoverHeader;
    var response = new GetCatTypesResponseEnvelope
    {
        Body = new GetCatTypesResponseBody
        {
            Response = await catInformationService.GetCatTypesAsync(envelope.Body.Request)
        }
    };

    // Generic serializer can be used here because the response wrapper contains namespace info
    return await GenericSerializer.Serialize(response);
});

app.Run();