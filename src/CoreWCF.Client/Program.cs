using System.ServiceModel;
using CoreWCF.Client.Components;
using CoreWCF.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Infra
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Services
var remoteServiceUrl = builder
                           .Configuration
                           .GetValue<string>("CatInformationService:Url") 
                       ?? throw new InvalidOperationException("Missing URL endpoint for Cat Information Service");

builder.Services.AddTransient<CatFactsServiceClient>(_ => new CatFactsServiceClient(
    CatFactsServiceClient.EndpointConfiguration.BasicHttpBinding_ICatFactsService, 
    new EndpointAddress($"{remoteServiceUrl}CatFactsService")));
    
builder.Services.AddTransient<CatInformationServiceClient>(_ => new CatInformationServiceClient(
    CatInformationServiceClient.EndpointConfiguration.BasicHttpBinding_ICatInformationService, 
    new EndpointAddress($"{remoteServiceUrl}CatInformationService")));
    
builder.Services.AddScoped<ICatInformationProvider, CatInformationProviderEasy>(); 

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
