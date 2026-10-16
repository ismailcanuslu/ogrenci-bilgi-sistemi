using BuildingBlocks.Api;
using Files.Api.Endpoints;
using Files.Application;
using Files.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration);

builder.Services.AddFilesApplication();

var connectionString = builder.Configuration.GetConnectionString("FilesDatabase")
    ?? "Server=localhost;Database=obs_files;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddFilesInfrastructure(
    builder.Configuration,
    connectionString);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseBuildingBlocksApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapBuildingBlocksApi();
app.MapFilesEndpoints();

app.Run();
