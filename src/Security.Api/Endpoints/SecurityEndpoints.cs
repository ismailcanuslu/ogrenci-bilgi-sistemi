using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Security.Application.Features;

namespace Security.Api.Endpoints;

public static class SecurityEndpoints
{
    public static IEndpointRouteBuilder MapSecurityEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("auth", 1, 0)
            .WithTags("Security");

        group.MapPost("/register", async (
            RegisterWithPassword.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("RegisterWithPassword")
        .WithSummary("Parola ile yeni kullanıcı kaydı");

        group.MapPost("/login", async (
            LoginWithPassword.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("LoginWithPassword")
        .WithSummary("Parola ile oturum açma");

        group.MapPost("/refresh", async (
            RefreshToken.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("RefreshToken")
        .WithSummary("Yenileme belirteci ile erişim belirtecini güncelleme");

        group.MapPost("/logout", async (
            RevokeSession.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("Logout")
        .WithSummary("Oturumu sonlandırma");

        group.MapPost("/passkey/register/begin", async (
            BeginPasskeyRegistration.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("BeginPasskeyRegistration")
        .WithSummary("Passkey kaydını başlatma");

        group.MapPost("/passkey/register/complete", async (
            CompletePasskeyRegistration.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("CompletePasskeyRegistration")
        .WithSummary("Passkey kaydını tamamlama");

        group.MapPost("/passkey/login/begin", async (
            BeginPasskeyLogin.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("BeginPasskeyLogin")
        .WithSummary("Passkey ile oturum açmayı başlatma");

        group.MapPost("/passkey/login/complete", async (
            LoginWithPasskey.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("CompletePasskeyLogin")
        .WithSummary("Passkey ile oturum açmayı tamamlama");

        app.MapAuthorizationEndpoints();

        return app;
    }
}
