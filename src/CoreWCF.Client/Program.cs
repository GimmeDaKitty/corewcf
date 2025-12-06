using System.ServiceModel;
using CoreWCF.Client.Behaviors;
using CoreWCF.Client.Components;
using CoreWCF.Client.Services;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

// Infra
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
IdentityModelEventSource.ShowPII = true; // Auth debugging - don't do this in production!
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Services
builder.Services.AddSingleton<FakeJwtTokenProvider>();

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
    
builder.Services.AddTransient<BellyRubServiceClient>(sp =>
{
    var client = new BellyRubServiceClient(
        BellyRubServiceClient.EndpointConfiguration.BasicHttpBinding_IBellyRubService,
        new EndpointAddress($"{remoteServiceUrl}BellyRubService"));
        
    // AUTH from DI - applicable to this service only.
    var tokenProvider = sp.GetRequiredService<FakeJwtTokenProvider>();
    client.Endpoint.EndpointBehaviors.Add(new AuthorizationHeaderBehavior(tokenProvider));

    return client;
});

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
