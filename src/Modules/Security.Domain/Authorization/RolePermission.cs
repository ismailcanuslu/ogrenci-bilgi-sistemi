namespace Security.Domain.Authorization;

public sealed class RolePermission
{
    public const int PermissionMaxLength = 150;

    public Guid RoleId { get; private set; }

    public string Permission { get; private set; } = null!;

    private RolePermission()
    {
    }

    internal RolePermission(Guid roleId, string permission)
    {
        RoleId = roleId;
        Permission = permission;
    }
}
