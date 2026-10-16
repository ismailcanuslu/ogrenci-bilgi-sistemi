using BuildingBlocks.Api;
using Scheduling.Api.Endpoints;
using Scheduling.Application;
using Scheduling.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration);
builder.Services.AddSchedulingApplication();

var connectionString = builder.Configuration.GetConnectionString("SchedulingDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost,1433;Database=Obs_Scheduling;User Id=sa;Password=Your_strong_password123;TrustServerCertificate=True";

builder.Services.AddSchedulingInfrastructure(builder.Configuration, connectionString);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseBuildingBlocksApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapBuildingBlocksApi();
app.MapSchedulingEndpoints();

app.Run();

public partial class Program { }
