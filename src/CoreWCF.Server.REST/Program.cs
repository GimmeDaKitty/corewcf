using CoreWCF.Server.REST.Middleware;
using CoreWCF.Server.REST.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Services
builder.Services.AddHttpClient();
builder.Services.AddTransient<ICatFactsService, CatFactsService>();
builder.Services.AddTransient<ICatInformationService, CatInformationService>();

// Controllers
builder.Services.AddControllers(options =>
{
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

app.MapControllers();

app.Run();