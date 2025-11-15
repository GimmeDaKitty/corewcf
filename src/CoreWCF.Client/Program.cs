using CoreWCF.Client;
using CoreWCF.Client.Components;
using CoreWCF.Client.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient(ClientConstants.CatInformationClientName, client =>
{
    var remoteServiceUrl = builder
        .Configuration
        .GetValue<string>("CatInformationService:Url") 
                           ?? throw new InvalidOperationException("Missing URL endpoint for Cat Information Service");
    client.BaseAddress = new Uri(remoteServiceUrl);
});

// TODO - BEA - DEMO - DEMONSTRATE ONE AND THE OTHER. IS MAKING ANOTHER CLIENT PROJECT OVERKILL?
//builder.Services.AddScoped<ICatInformationClient, RESTCatInformationClient>();
builder.Services.AddScoped<ICatInformationClient, CoreWcfCatInformationClient>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
