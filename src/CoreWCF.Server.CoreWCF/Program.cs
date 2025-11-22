using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;
using CoreWCF.Server.CoreWCF.Behaviors;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddTransient<ICatInformationService, CatInformationService>();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services.AddSingleton<CatLoverHeaderBehavior>();
builder.Services.AddHttpClient();
//builder.Services.AddSingleton<IServiceBehavior, FaultContractAttribute>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<CatInformationService>(ops => 
    {
        ops.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });
    
    serviceBuilder.AddServiceEndpoint<CatInformationService, ICatInformationService>(
        new BasicHttpBinding(BasicHttpSecurityMode.Transport), 
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
