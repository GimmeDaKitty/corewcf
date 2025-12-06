using CoreWCF.Server.REST.Filters;
using CoreWCF.Server.REST.Middleware;
using CoreWCF.Server.REST.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
IdentityModelEventSource.ShowPII = true; // Auth debugging - do not do this in production!
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);

// Services
builder.Services.AddHttpClient();
builder.Services.AddTransient<ICatFactsService, CatFactsService>();
builder.Services.AddTransient<ICatInformationService, CatInformationService>();
builder.Services.AddTransient<IBellyRubService, BellyRubService>();

// Controllers
builder.Services.AddScoped<CatLoverHeaderAuthorizationFilter>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CatLoverHeaderAuthorizationFilter>();
    
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

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();

// Middleware
app.UseMiddleware<SoapRoutingMiddleware>();
app.UseMiddleware<RequestIdLoggingMiddleware>();

app.UseRouting();

// Authentication
app.UseAuthentication();
app.UseAuthorization();

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

app.MapPost("/BellyRubService", async (
        [FromServices] IBellyRubService bellyRubService) =>
    {
        var bellyRubAllowedResponse = await bellyRubService.AllowBellyRubAsync(new AllowBellyRubRequest());
        var soapResponse = SoapResponseEnvelopeBuilder.GetSOAPBellyRubResponse(bellyRubAllowedResponse.AllowBellyRubResult);
        return Results.Content(soapResponse, "text/xml; charset=utf-8");
    })
    .RequireAuthorization("IsCoolHuman");

app.MapControllers();

app.Run();