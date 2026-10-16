using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace UniversalSystem.UnitTests.Architecture;

public class CleanArchitectureTests
{
    private static readonly Assembly[] DomainAssemblies = new[]
    {
        typeof(global::BuildingBlocks.Domain.Primitives.Entity<>).Assembly,
        typeof(global::Security.Domain.Entities.User).Assembly,
        typeof(global::Files.Domain.Files.File).Assembly,
        typeof(global::Notifications.Domain.Notifications.Notification).Assembly,
        typeof(global::Translations.Domain.LanguageCode).Assembly,
        typeof(global::Auditing.Domain.AuditLog).Assembly,
        typeof(global::Messaging.Domain.OutboxMessage).Assembly,
        typeof(global::Scheduling.Domain.JobDefinition).Assembly,
        typeof(global::Webhooks.Domain.WebhookSubscription).Assembly,
        typeof(global::ImportExport.Domain.ImportJob).Assembly
    };

    private static readonly Assembly[] ApplicationAssemblies = new[]
    {
        typeof(global::Security.Application.DependencyInjection).Assembly,
        typeof(global::Files.Application.DependencyInjection).Assembly,
        typeof(global::Notifications.Application.DependencyInjection).Assembly,
        typeof(global::Translations.Application.DependencyInjection).Assembly,
        typeof(global::Auditing.Application.DependencyInjection).Assembly,
        typeof(global::Messaging.Application.DependencyInjection).Assembly,
        typeof(global::Scheduling.Application.DependencyInjection).Assembly,
        typeof(global::Webhooks.Application.DependencyInjection).Assembly,
        typeof(global::ImportExport.Application.DependencyInjection).Assembly
    };

    [Fact]
    public void DomainLayers_ShouldNotHaveDependencyOn_InfrastructureLayers()
    {
        foreach (var assembly in DomainAssemblies)
        {
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(
                    "Security.Infrastructure",
                    "Files.Infrastructure",
                    "Notifications.Infrastructure",
                    "Translations.Infrastructure",
                    "Auditing.Infrastructure",
                    "Messaging.Infrastructure",
                    "Scheduling.Infrastructure",
                    "Webhooks.Infrastructure",
                    "ImportExport.Infrastructure",
                    "BuildingBlocks.Api")
                .GetResult();

            Assert.True(
                result.IsSuccessful,
                $"Domain projesi '{assembly.GetName().Name}' Infrastructure veya Api katmanına bağımlı olamaz! İhlaller: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }

    [Fact]
    public void DomainLayers_ShouldNotHaveDependencyOn_ApiLayers()
    {
        foreach (var assembly in DomainAssemblies)
        {
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(
                    "Security.Api",
                    "Files.Api",
                    "Notifications.Api",
                    "Translations.Api",
                    "Auditing.Api",
                    "Messaging.Api",
                    "Scheduling.Api",
                    "Webhooks.Api",
                    "ImportExport.Api",
                    "Bootstrapper")
                .GetResult();

            Assert.True(
                result.IsSuccessful,
                $"Domain projesi '{assembly.GetName().Name}' Api katmanına bağımlı olamaz! İhlaller: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }

    [Fact]
    public void ApplicationLayers_ShouldNotHaveDependencyOn_InfrastructureLayers()
    {
        foreach (var assembly in ApplicationAssemblies)
        {
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(
                    "Security.Infrastructure",
                    "Files.Infrastructure",
                    "Notifications.Infrastructure",
                    "Translations.Infrastructure",
                    "Auditing.Infrastructure",
                    "Messaging.Infrastructure",
                    "Scheduling.Infrastructure",
                    "Webhooks.Infrastructure",
                    "ImportExport.Infrastructure")
                .GetResult();

            Assert.True(
                result.IsSuccessful,
                $"Application projesi '{assembly.GetName().Name}' Infrastructure katmanına bağımlı olamaz! İhlaller: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }

    [Fact]
    public void ApplicationLayers_ShouldNotHaveDependencyOn_ApiLayers()
    {
        foreach (var assembly in ApplicationAssemblies)
        {
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(
                    "Security.Api",
                    "Files.Api",
                    "Notifications.Api",
                    "Translations.Api",
                    "Auditing.Api",
                    "Messaging.Api",
                    "Scheduling.Api",
                    "Webhooks.Api",
                    "ImportExport.Api",
                    "Bootstrapper")
                .GetResult();

            Assert.True(
                result.IsSuccessful,
                $"Application projesi '{assembly.GetName().Name}' Api katmanına bağımlı olamaz! İhlaller: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }
}
