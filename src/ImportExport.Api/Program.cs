using BuildingBlocks.Api;
using ImportExport.Api.Endpoints;
using ImportExport.Application;
using ImportExport.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration);
builder.Services.AddImportExportApplication();
builder.Services.AddImportExportInfrastructure();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseBuildingBlocksApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapBuildingBlocksApi();
app.MapImportExportEndpoints();

app.Run();

public partial class Program { }
