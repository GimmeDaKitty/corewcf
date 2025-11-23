using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;
using CoreWCF.Server.CoreWCF;
using CoreWCF.Server.CoreWCF.Behaviors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddTransient<ICatInformationService, CatInformationService>();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services.AddSingleton<CatLoverHeaderBehavior>();
builder.Services.AddHttpClient();
builder.Services.AddTransient<CatInformationService>(); // If not added, you need an empty constructor on the service class

// TODO - error handling
//builder.Services.AddSingleton<IServiceBehavior, FaultContractAttribute>();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => 
    {
        options.Authority = "fake-issuer";
        options.Audience = "fake-audience";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            RequireSignedTokens = false
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("IsCoolHuman", new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .RequireClaim("iscoolhuman", ["owner", "isalergic", "catlady"])
        .Build());

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Authentication
app.UseAuthentication();
app.UseAuthorization();

// Service configuration
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<CatInformationService>(ops => 
    {
        ops.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });
    
    serviceBuilder.AddServiceEndpoint<CatInformationService, ICatInformationService>(
        Bindings.AuthorizationHttpBinding,
        "/CatInformationService", ep =>
        {
            var endpointBehavior = app.Services.GetRequiredService<CatLoverHeaderBehavior>();
            ep.EndpointBehaviors.Add(endpointBehavior);
        });
    
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
    // metadata available at: https://localhost:5002/CatInformationService?wsdl
});

app.Run();
