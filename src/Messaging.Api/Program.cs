using BuildingBlocks.Api;
using Messaging.Api.Endpoints;
using Messaging.Application;
using Messaging.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration);
builder.Services.AddMessagingApplication();

var connectionString = builder.Configuration.GetConnectionString("MessagingDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost,1433;Database=Obs_Messaging;User Id=sa;Password=Your_strong_password123;TrustServerCertificate=True";

builder.Services.AddMessagingInfrastructure(builder.Configuration, connectionString);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseBuildingBlocksApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapBuildingBlocksApi();
app.MapMessagingEndpoints();

app.Run();

public partial class Program { }
