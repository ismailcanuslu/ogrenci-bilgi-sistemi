using BuildingBlocks.Api;
using Notifications.Api.Endpoints;
using Notifications.Application;
using Notifications.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration);

builder.Services.AddNotificationsApplication();

var connectionString = builder.Configuration.GetConnectionString("NotificationsDatabase")
    ?? "Server=localhost;Database=obs_notifications;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddNotificationsInfrastructure(
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
app.MapNotificationsEndpoints();

app.Run();
