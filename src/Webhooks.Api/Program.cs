using BuildingBlocks.Api;
using Webhooks.Api.Endpoints;
using Webhooks.Application;
using Webhooks.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration);
builder.Services.AddWebhooksApplication();

var connectionString = builder.Configuration.GetConnectionString("WebhooksDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost,1433;Database=Obs_Webhooks;User Id=sa;Password=Your_strong_password123;TrustServerCertificate=True";

builder.Services.AddWebhooksInfrastructure(builder.Configuration, connectionString);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseBuildingBlocksApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapBuildingBlocksApi();
app.MapWebhooksEndpoints();

app.Run();

public partial class Program { }
