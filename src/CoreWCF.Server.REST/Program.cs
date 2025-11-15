using CoreWCF.Contracts;
using CoreWCF.Server.Common.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddTransient<ICatInformationService,CatInformationService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/CatInformationService", ([FromServices]ICatInformationService catInformationService) =>
{
    var photoBytes = catInformationService.GetPhoto();
    return Results.File(photoBytes, "image/jpeg");
})
.WithName("CatInformationService");

app.Run();