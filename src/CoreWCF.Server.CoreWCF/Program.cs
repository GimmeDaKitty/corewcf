using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;
using CoreWCF.Server.CoreWCF;
using CoreWCF.Server.CoreWCF.Behaviors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
IdentityModelEventSource.ShowPII = true; // Auth debugging - don't do this in production!
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

// Service implementations. Moeten Singleton zijn omdat CoreWCF anders een default lege constructor nodig heeft
builder.Services.AddSingleton<CatFactsService>();
builder.Services.AddSingleton<CatInformationService>();
builder.Services.AddSingleton<BellyRubService>(); 

// Behaviors
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services.AddSingleton<CatInformationServiceEndpointBehaviors>();
builder.Services.AddSingleton<CatInformationServiceOperationBehaviors>();
builder.Services.AddHttpClient();

// TODO - error handling
//builder.Services.AddSingleton<IServiceBehavior, FaultContractAttribute>();

// Authentication
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsCoolHuman", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("iscoolhuman", "owner", "isalergic", "catlady");
    });
});

var app = builder.Build();

// Authentication
app.UseAuthentication();
app.UseAuthorization();

// Service configuration
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<CatFactsService>(ops =>
    {
        ops.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });
    
    serviceBuilder.AddService<CatInformationService>(ops => 
    {
        ops.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });
    
    serviceBuilder.AddService<BellyRubService>(ops => 
    {
        ops.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });
    
    serviceBuilder.AddServiceEndpoint<CatFactsService, ICatFactsService>(
        Bindings.BasicHttpBindingWithEncoding, "/CatFactsService");
    
    serviceBuilder.AddServiceEndpoint<CatInformationService, ICatInformationService>(
        Bindings.BasicHttpBindingWithEncoding,
        "/CatInformationService", ep =>
        {
            var endpointBehavior = app.Services.GetRequiredService<CatInformationServiceEndpointBehaviors>();
            ep.EndpointBehaviors.Add(endpointBehavior);

            var operationBehavior = app.Services.GetRequiredService<CatInformationServiceOperationBehaviors>();
            var getCatTypesOperation = ep.Contract.Operations.First(o => o.Name == nameof(ICatInformationService.GetCatTypes));
            getCatTypesOperation.OperationBehaviors.Add(operationBehavior);
        });
    
    serviceBuilder.AddServiceEndpoint<BellyRubService, IBellyRubService>(
        Bindings.AuthorizationHttpBinding, "/BellyRubService");
    
    // Enables metadata endpoint
    //  https://localhost:5002/CatFactsService?wsdl
    //  https://localhost:5002/CatInformationService?wsdl
    //  https://localhost:5002/BellyRubService?wsdl
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.Run();
