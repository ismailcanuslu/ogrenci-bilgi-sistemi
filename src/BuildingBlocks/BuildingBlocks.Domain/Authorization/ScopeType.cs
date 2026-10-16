namespace BuildingBlocks.Domain.Authorization;

/// <summary>
/// Bir yetkinin geçerli olduğu kaynak kapsamı seviyesi.
/// </summary>
public enum ScopeType
{
    University = 0,
    Faculty = 1,
    Department = 2,
    Course = 3,
    Self = 4
}
