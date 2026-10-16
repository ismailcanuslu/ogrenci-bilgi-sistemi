using BuildingBlocks.Api;
using Translations.Api.Endpoints;
using Translations.Application;
using Translations.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration);
builder.Services.AddTranslationsApplication();
builder.Services.AddTranslationsInfrastructure();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseBuildingBlocksApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapBuildingBlocksApi();
app.MapTranslationEndpoints();

app.Run();
