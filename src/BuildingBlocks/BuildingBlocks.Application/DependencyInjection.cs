using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
            configuration.AddOpenBehavior(
                typeof(Behaviors.ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.TryAddSingleton<BuildingBlocks.Application.Abstractions.Time.IDateTimeProvider, BuildingBlocks.Application.Abstractions.Time.DateTimeProvider>();

        return services;
    }

    /// <summary>
    /// Modül application katmanlarının ortak kullandığı pipeline ve yetki servislerini kaydeder.
    /// </summary>
    public static IServiceCollection AddModuleApplication(
        this IServiceCollection services,
        System.Reflection.Assembly moduleAssembly)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(moduleAssembly);
        });

        // MediatR pipeline davranışları açık generic olduğundan tüm modüller için geçerlidir;
        // birden fazla modül çağırsa bile tek kez kaydedilir.
        services.TryAddEnumerable(
            ServiceDescriptor.Transient(
                typeof(MediatR.IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>)));

        services.AddValidatorsFromAssembly(moduleAssembly);

        services.TryAddScoped<IPermissionGuard, PermissionGuard>();
        services.TryAddSingleton<BuildingBlocks.Application.Abstractions.Time.IDateTimeProvider, BuildingBlocks.Application.Abstractions.Time.DateTimeProvider>();

        return services;
    }
}
