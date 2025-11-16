using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddTransient<ICatInformationService, CatInformationService>();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
//builder.Services.AddSingleton<IServiceBehavior, FaultContractAttribute>();

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<CatInformationService>(ops => 
    {
        ops.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });
    serviceBuilder.AddServiceEndpoint<CatInformationService, ICatInformationService>(
        new BasicHttpBinding(BasicHttpSecurityMode.Transport), 
        "/CatInformationService");
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
    // metadata available at: https://localhost:5002/CatInformationService?wsdl
});

app.Run();
