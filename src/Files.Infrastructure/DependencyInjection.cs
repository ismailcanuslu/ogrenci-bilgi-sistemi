using BuildingBlocks.Application.Abstractions.Persistence;
using Files.Application.Abstractions.Persistence;
using Files.Application.Abstractions.Security;
using Files.Application.Abstractions.Storage;
using Files.Application.Abstractions.Validation;
using Files.Infrastructure.Options;
using Files.Infrastructure.Persistence;
using Files.Infrastructure.Persistence.Repositories;
using Files.Infrastructure.Security;
using Files.Infrastructure.Storage;
using Files.Infrastructure.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Files.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFilesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<FilesDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<FilesDbContext>());

        services.AddScoped<IFileRepository, FileRepository>();

        services
            .AddOptions<FileStorageOptions>()
            .Bind(configuration.GetSection(FileStorageOptions.SectionName));

        services
            .AddOptions<S3StorageOptions>()
            .Bind(configuration.GetSection(S3StorageOptions.SectionName));

        services
            .AddOptions<FileValidationOptions>()
            .Bind(configuration.GetSection(FileValidationOptions.SectionName));

        services
            .AddOptions<FileSecurityOptions>()
            .Bind(configuration.GetSection(FileSecurityOptions.SectionName));

        services.AddSingleton<PhysicalFileStorage>();
        services.AddSingleton<S3FileStorage>();

        services.AddSingleton<IFileStorage>(provider =>
        {
            var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<FileStorageOptions>>().Value;
            return string.Equals(options.Provider, "Physical", StringComparison.OrdinalIgnoreCase)
                ? provider.GetRequiredService<PhysicalFileStorage>()
                : provider.GetRequiredService<S3FileStorage>();
        });

        services.AddSingleton<IFileStorageReader>(provider =>
        {
            var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<FileStorageOptions>>().Value;
            return string.Equals(options.Provider, "Physical", StringComparison.OrdinalIgnoreCase)
                ? provider.GetRequiredService<PhysicalFileStorage>()
                : provider.GetRequiredService<S3FileStorage>();
        });

        services.AddSingleton<IFileSignatureDetector, FileSignatureDetector>();
        services.AddSingleton<IFileHashCalculator, FileHashCalculator>();
        services.AddScoped<IFileValidator, FileValidator>();
        services.AddScoped<IFileAuthorizationService, FileAuthorizationService>();

        return services;
    }
}
