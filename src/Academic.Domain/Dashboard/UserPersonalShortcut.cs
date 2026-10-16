using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Dashboard;

public sealed class UserPersonalShortcut : Entity<Guid>
{
    public const int TitleMaxLength = 100;
    public const int UrlMaxLength = 500;
    public const int IconMaxLength = 50;
    public const int ColorMaxLength = 30;

    public Guid UserId { get; private set; }

    public string Title { get; private set; } = null!;

    public string TargetUrl { get; private set; } = null!;

    public string? Icon { get; private set; }

    public string? Color { get; private set; }

    public int Order { get; private set; }

    private UserPersonalShortcut()
    {
    }

    public UserPersonalShortcut(
        Guid id,
        Guid userId,
        string title,
        string targetUrl,
        string? icon,
        string? color,
        int order)
        : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("UserPersonalShortcut.UserRequired");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("UserPersonalShortcut.TitleRequired");
        }

        if (string.IsNullOrWhiteSpace(targetUrl))
        {
            throw new DomainException("UserPersonalShortcut.TargetUrlRequired");
        }

        UserId = userId;
        Title = title.Trim();
        TargetUrl = targetUrl.Trim();
        Icon = icon?.Trim();
        Color = color?.Trim();
        Order = Math.Max(0, order);
    }

    public void Update(string title, string targetUrl, string? icon, string? color, int order)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("UserPersonalShortcut.TitleRequired");
        }

        if (string.IsNullOrWhiteSpace(targetUrl))
        {
            throw new DomainException("UserPersonalShortcut.TargetUrlRequired");
        }

        Title = title.Trim();
        TargetUrl = targetUrl.Trim();
        Icon = icon?.Trim();
        Color = color?.Trim();
        Order = Math.Max(0, order);
    }
}
