using CoreWCF.Server.REST.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Services
builder.Services.AddHttpClient();
builder.Services.AddTransient<ICatFactsService, CatFactsService>();

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

app.Run();