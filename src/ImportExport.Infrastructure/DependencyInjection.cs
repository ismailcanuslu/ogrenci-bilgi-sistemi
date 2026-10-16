using ImportExport.Application.Abstractions;
using ImportExport.Infrastructure.Persistence;
using ImportExport.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ImportExport.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddImportExportInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IImportExportService, CsvImportExportService>();
        services.AddSingleton<IImportJobRepository, InMemoryImportJobRepository>();
        return services;
    }
}
