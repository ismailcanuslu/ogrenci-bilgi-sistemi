using Auditing.Api.Endpoints;
using Auditing.Application;
using Auditing.Infrastructure;
using BuildingBlocks.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration);
builder.Services.AddAuditingApplication();

var connectionString = builder.Configuration.GetConnectionString("AuditingDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost,1433;Database=Obs_Auditing;User Id=sa;Password=Your_strong_password123;TrustServerCertificate=True";

builder.Services.AddAuditingInfrastructure(builder.Configuration, connectionString);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseBuildingBlocksApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapBuildingBlocksApi();
app.MapAuditLogEndpoints();

app.Run();

public partial class Program { }
