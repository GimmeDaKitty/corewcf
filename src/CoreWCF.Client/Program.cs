using System.ServiceModel;
using CoreWCF.Client;
using CoreWCF.Client.Components;
using CoreWCF.Client.Services;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var remoteServiceUrl = builder
                           .Configuration
                           .GetValue<string>("CatInformationService:Url") 
                       ?? throw new InvalidOperationException("Missing URL endpoint for Cat Information Service");

var clientType = builder
    .Configuration
    .GetValue<DifficultyLevel>("Client:DifficultyLevel");

if (clientType == DifficultyLevel.Hard)
{
    builder.Services.AddHttpClient(ClientConstants.CatInformationClientName, client =>
    {
        client.BaseAddress = new Uri(remoteServiceUrl);
    });
    
    builder.Services.AddScoped<ICatInformationProvider, RestCatInformationProviderHard>();
}
else
{
    builder.Services.AddTransient<CatInformationServiceClient>(_ => new CatInformationServiceClient(
        CatInformationServiceClient.EndpointConfiguration.BasicHttpBinding_ICatInformationService, 
        new EndpointAddress($"{remoteServiceUrl}CatInformationService")));
    
    builder.Services.AddTransient<BellyRubServiceClient>(_ => new BellyRubServiceClient(
        BellyRubServiceClient.EndpointConfiguration.BasicHttpBinding_IBellyRubService, 
        new EndpointAddress($"{remoteServiceUrl}BellyRubService")));
    
    builder.Services.AddScoped<ICatInformationProvider, RestCatInformationProviderEasy>(); 
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Auth debugging
IdentityModelEventSource.ShowPII = true;
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);

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
