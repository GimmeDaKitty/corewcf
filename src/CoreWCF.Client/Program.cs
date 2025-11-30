using System.ServiceModel;
using CoreWCF.Client;
using CoreWCF.Client.Behaviors;
using CoreWCF.Client.Components;
using CoreWCF.Client.Services;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<FakeJwtTokenProvider>();

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
    
    builder.Services.AddScoped<ICatInformationProvider, CatInformationProviderHard>();
}
else
{
    builder.Services.AddTransient<CatInformationServiceClient>(_ => new CatInformationServiceClient(
        CatInformationServiceClient.EndpointConfiguration.BasicHttpBinding_ICatInformationService, 
        new EndpointAddress($"{remoteServiceUrl}CatInformationService")));
    
    builder.Services.AddTransient<BellyRubServiceClient>(sp =>
    {
        var client = new BellyRubServiceClient(
            BellyRubServiceClient.EndpointConfiguration.BasicHttpBinding_IBellyRubService,
            new EndpointAddress($"{remoteServiceUrl}BellyRubService"));
        
        // AUTH from DI
        var tokenProvider = sp.GetRequiredService<FakeJwtTokenProvider>();
        var logger = sp.GetRequiredService<ILogger<ContractBehavior>>();
        client.Endpoint.EndpointBehaviors.Add(new AuthorizationHeaderBehavior(tokenProvider));
        client.Endpoint.Contract.ContractBehaviors.Add(new ContractBehavior(logger));

        return client;
    });
    
    builder.Services.AddScoped<ICatInformationProvider, CatInformationProviderEasy>(); 
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
