using BuildingBlocks.Api;
using Security.Api.Endpoints;
using Security.Application;
using Security.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration);

builder.Services.AddSecurityApplication();

var connectionString = builder.Configuration.GetConnectionString("SecurityDatabase")
    ?? "Server=localhost;Database=obs_security;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddSecurityInfrastructure(
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
app.MapSecurityEndpoints();

app.Run();