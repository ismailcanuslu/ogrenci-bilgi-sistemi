using Academic.Api.Endpoints;
using Academic.Application;
using Academic.Infrastructure;
using Auditing.Api.Endpoints;
using Auditing.Application;
using Auditing.Infrastructure;
using BuildingBlocks.Api;
using Files.Api.Endpoints;
using Files.Application;
using Files.Infrastructure;
using Messaging.Api.Endpoints;
using Messaging.Application;
using Messaging.Infrastructure;
using Notifications.Api.Endpoints;
using Notifications.Application;
using Notifications.Infrastructure;
using Scheduling.Api.Endpoints;
using Scheduling.Application;
using Scheduling.Infrastructure;
using Security.Api.Endpoints;
using Security.Application;
using Security.Infrastructure;
using Translations.Api.Endpoints;
using Translations.Application;
using Translations.Infrastructure;
using ImportExport.Api.Endpoints;
using ImportExport.Application;
using ImportExport.Infrastructure;
using Webhooks.Api.Endpoints;
using Webhooks.Application;
using Webhooks.Infrastructure;
using CampusLife.Api.Endpoints;
using CampusLife.Application;
using CampusLife.Infrastructure;
using SupportDesk.Api.Endpoints;
using SupportDesk.Application;
using SupportDesk.Infrastructure;
using ContinuingEducation.Api.Endpoints;
using ContinuingEducation.Application;
using ContinuingEducation.Infrastructure;
using DocumentManagement.Api.Endpoints;
using DocumentManagement.Application;
using DocumentManagement.Infrastructure;
using StudentApplications.Api.Endpoints;
using StudentApplications.Application;
using StudentApplications.Infrastructure;
using SocialContribution.Api;
using SocialContribution.Application;
using SocialContribution.Infrastructure;
using InstitutionalEvaluation.Api;
using InstitutionalEvaluation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// CORS for SPA Clients
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// BuildingBlocks
builder.Services.AddBuildingBlocksApi(builder.Configuration);
builder.Services.AddSingleton<BuildingBlocks.Application.Abstractions.Time.IDateTimeProvider, BuildingBlocks.Application.Abstractions.Time.DateTimeProvider>();
builder.Services.AddSingleton<BuildingBlocks.Api.Idempotency.IIdempotencyStore, BuildingBlocks.Api.Idempotency.InMemoryIdempotencyStore>();

// Security Module
builder.Services.AddSecurityApplication();
var securityConnectionString = builder.Configuration.GetConnectionString("SecurityDatabase")
    ?? "Server=localhost;Database=obs_security;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddSecurityInfrastructure(builder.Configuration, securityConnectionString);

// Academic Module (Faculty / Department)
builder.Services.AddAcademicApplication();
var academicConnectionString = builder.Configuration.GetConnectionString("AcademicDatabase")
    ?? "Server=localhost;Database=obs_academic;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddAcademicInfrastructure(builder.Configuration, academicConnectionString);

// Files Module
builder.Services.AddFilesApplication();
var filesConnectionString = builder.Configuration.GetConnectionString("FilesDatabase")
    ?? "Server=localhost;Database=obs_files;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddFilesInfrastructure(builder.Configuration, filesConnectionString);

// Notifications Module
builder.Services.AddNotificationsApplication();
var notificationsConnectionString = builder.Configuration.GetConnectionString("NotificationsDatabase")
    ?? "Server=localhost;Database=obs_notifications;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddNotificationsInfrastructure(builder.Configuration, notificationsConnectionString);

// Translations Module
builder.Services.AddTranslationsApplication();
builder.Services.AddTranslationsInfrastructure();

// Auditing Module
builder.Services.AddAuditingApplication();
var auditingConnectionString = builder.Configuration.GetConnectionString("AuditingDatabase")
    ?? builder.Configuration.GetConnectionString("AuditingDb")
    ?? "Server=localhost;Database=obs_auditing;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddAuditingInfrastructure(builder.Configuration, auditingConnectionString);

// Messaging Module
builder.Services.AddMessagingApplication();
var messagingConnectionString = builder.Configuration.GetConnectionString("MessagingDatabase")
    ?? builder.Configuration.GetConnectionString("MessagingDb")
    ?? "Server=localhost;Database=obs_messaging;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddMessagingInfrastructure(builder.Configuration, messagingConnectionString);

// Scheduling Module
builder.Services.AddSchedulingApplication();
var schedulingConnectionString = builder.Configuration.GetConnectionString("SchedulingDatabase")
    ?? builder.Configuration.GetConnectionString("SchedulingDb")
    ?? "Server=localhost;Database=obs_scheduling;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddSchedulingInfrastructure(builder.Configuration, schedulingConnectionString);

// Webhooks Module
builder.Services.AddWebhooksApplication();
var webhooksConnectionString = builder.Configuration.GetConnectionString("WebhooksDatabase")
    ?? builder.Configuration.GetConnectionString("WebhooksDb")
    ?? "Server=localhost;Database=obs_webhooks;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddWebhooksInfrastructure(builder.Configuration, webhooksConnectionString);

// Import / Export Module
builder.Services.AddImportExportApplication();
builder.Services.AddImportExportInfrastructure();

// CampusLife Module (Reservations, Clubs, Events)
builder.Services.AddCampusLifeApplication();
var campusLifeConnectionString = builder.Configuration.GetConnectionString("CampusLifeDatabase")
    ?? "Server=localhost;Database=obs_campus_life;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddCampusLifeInfrastructure(builder.Configuration, campusLifeConnectionString);

// SupportDesk Module (Helpdesk & Institution Ticketing)
builder.Services.AddSupportDeskApplication();
var supportDeskConnectionString = builder.Configuration.GetConnectionString("SupportDeskDatabase")
    ?? "Server=localhost;Database=obs_support_desk;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddSupportDeskInfrastructure(builder.Configuration, supportDeskConnectionString);

// ContinuingEducation Module (SEM / Kurs ve Sertifika)
builder.Services.AddContinuingEducationApplication();
builder.Services.AddContinuingEducationInfrastructure(builder.Configuration);

// DocumentManagement Module (EBYS & Belge Doğrulama)
builder.Services.AddDocumentManagementApplication();
builder.Services.AddDocumentManagementInfrastructure(builder.Configuration);

// StudentApplications Module (ÇAP, Yandal, Yatay Geçiş, Yaz Okulu, Belge Talebi)
builder.Services.AddStudentApplicationsApplication();
builder.Services.AddStudentApplicationsInfrastructure(builder.Configuration);

// SocialContribution Module (Toplumsal Katkı Koordinatörlüğü)
builder.Services.AddSocialContributionApplication();
builder.Services.AddSocialContributionInfrastructure(builder.Configuration);

// InstitutionalEvaluation Module (Kurumsal Değerlendirme & İstatistikler)
builder.Services.AddInstitutionalEvaluationInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseBuildingBlocksApi();
app.UseCors();

// JWT doğrulama (Security.Infrastructure) ve authorization middleware'i.
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapBuildingBlocksApi();
app.MapSecurityEndpoints();
app.MapForgotPasswordEndpoints();
app.MapAcademicEndpoints();
app.MapPortalSettingsEndpoints();
app.MapStudentPortalEndpoints();
app.MapDashboardEndpoints();
app.MapFilesEndpoints();
app.MapNotificationsEndpoints();
app.MapTranslationEndpoints();
app.MapAuditLogEndpoints();
app.MapMessagingEndpoints();
app.MapSchedulingEndpoints();
app.MapWebhooksEndpoints();
app.MapImportExportEndpoints();
app.MapCampusLifeEndpoints();
app.MapSupportDeskEndpoints();
app.MapContinuingEducationEndpoints();
app.MapDocumentManagementEndpoints();
app.MapStudentApplicationsEndpoints();
app.MapSocialContributionEndpoints();
app.MapEducationCatalogEndpoints();
app.MapInstitutionalEvaluationEndpoints();

// Veritabanı tablolarını ilk açılışta garanti altına alma (Docker / Local DB kalkınca otomatik hazır olur)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var securityDb = scope.ServiceProvider.GetRequiredService<Security.Infrastructure.Persistence.SecurityDbContext>();
        await securityDb.Database.EnsureCreatedAsync();

        var academicDb = scope.ServiceProvider.GetRequiredService<Academic.Infrastructure.Persistence.AcademicDbContext>();
        await academicDb.Database.EnsureCreatedAsync();

        var socialDb = scope.ServiceProvider.GetRequiredService<SocialContribution.Infrastructure.Persistence.SocialContributionDbContext>();
        await socialDb.Database.EnsureCreatedAsync();

        var evalDb = scope.ServiceProvider.GetRequiredService<InstitutionalEvaluation.Infrastructure.Persistence.InstitutionalEvaluationDbContext>();
        await evalDb.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInit");
        logger.LogWarning(ex, "Veritabanı tabloları başlatılırken uyarı oluştu (Migration zaten yapılmış veya DB bağlantısı bekleniyor olabilir).");
    }
}

app.Run();
