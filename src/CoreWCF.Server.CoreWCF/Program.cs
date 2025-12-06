using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;
using CoreWCF.Server.CoreWCF;
using CoreWCF.Server.CoreWCF.Behaviors;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

// Service implementations. Moeten Singleton zijn omdat CoreWCF anders een default lege constructor nodig heeft
builder.Services.AddSingleton<CatFactsService>();
builder.Services.AddSingleton<CatInformationService>();

// Behaviors
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services.AddSingleton<CatInformationServiceEndpointBehaviors>();
builder.Services.AddSingleton<CatInformationServiceOperationBehaviors>();
builder.Services.AddHttpClient();

// TODO - error handling
//builder.Services.AddSingleton<IServiceBehavior, FaultContractAttribute>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Auth debugging
IdentityModelEventSource.ShowPII = true;
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);

var app = builder.Build();

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
    
    serviceBuilder.AddServiceEndpoint<CatFactsService, ICatFactsService>(
        Bindings.BasicHttpBindingWithEncoding, "/CatFactsService");
    
    serviceBuilder.AddServiceEndpoint<CatInformationService, ICatInformationService>(
        Bindings.BasicHttpBindingWithEncoding,
        "/CatInformationService");
    
    // Enables metadata endpoint
    //  https://localhost:5002/CatFactsService?wsdl
    //  https://localhost:5002/CatInformationService?wsdl
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.Run();
