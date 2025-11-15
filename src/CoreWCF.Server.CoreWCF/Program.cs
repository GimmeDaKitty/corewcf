using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<CatInformationService>();
    serviceBuilder.AddServiceEndpoint<CatInformationService, ICatInformationService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport), 
        "/CatInformationService");
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.Run();
