using CoreWCF.Client;
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


builder.Services.AddHttpClient(ClientConstants.CatInformationClientName, client =>
{
    client.BaseAddress = new Uri(remoteServiceUrl);
});

builder.Services.AddScoped<ICatInformationProvider, CatInformationProviderHard>();



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
