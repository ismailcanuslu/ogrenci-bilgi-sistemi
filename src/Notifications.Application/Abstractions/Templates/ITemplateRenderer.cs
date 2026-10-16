namespace Notifications.Application.Abstractions.Templates;

public interface ITemplateRenderer
{
    Task<string> RenderAsync(
        string templateCode,
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken = default);
}
