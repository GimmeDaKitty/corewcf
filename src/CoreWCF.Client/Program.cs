using System.ServiceModel;
using CoreWCF.Client;
using CoreWCF.Client.Components;
using CoreWCF.Client.REST;
using CoreWCF.Client.Services;

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
    .GetValue<ClientType>("Client:ClientType");

if (clientType == ClientType.REST)
{
    builder.Services.AddHttpClient(ClientConstants.CatInformationClientName, client =>
    {
        client.BaseAddress = new Uri(remoteServiceUrl);
    });
    
    // THE HARD WAY
    //builder.Services.AddScoped<ICatInformationProvider, RestCatInformationProvider>();
    
    // THE EASY WAY
    builder.Services.AddTransient<CatInformationServiceClient>(_ => new CatInformationServiceClient(
        CatInformationServiceClient.EndpointConfiguration.BasicHttpBinding_ICatInformationService, 
        new EndpointAddress(remoteServiceUrl))); 
    builder.Services.AddScoped<ICatInformationProvider, RestCatInformationProviderEasyWay>();
}
else
{
    builder.Services.AddTransient<CatInformationServiceClient>(_ => new CatInformationServiceClient(
        CatInformationServiceClient.EndpointConfiguration.BasicHttpBinding_ICatInformationService, 
        new EndpointAddress(remoteServiceUrl)));
    
    builder.Services.AddScoped<ICatInformationProvider, CoreWcfCatInformationProvider>(); 
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
//app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
