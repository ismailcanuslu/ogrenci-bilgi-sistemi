using BuildingBlocks.Domain.Authorization;


namespace BuildingBlocks.Application.Abstractions.Security;

/// <summary>
/// Yetki kontrolünün yapıldığı somut kaynak kapsamı (ör. belirli bir bölüm).
/// University kapsamı için <see cref="Id"/> null olmalıdır.
/// </summary>
public sealed record ResourceScope(ScopeType Type, Guid? Id)
{
    public static ResourceScope University { get; } = new(ScopeType.University, null);

    public static ResourceScope Faculty(Guid facultyId) => new(ScopeType.Faculty, facultyId);

    public static ResourceScope Department(Guid departmentId) => new(ScopeType.Department, departmentId);

    public static ResourceScope Course(Guid courseOfferingId) => new(ScopeType.Course, courseOfferingId);

    public static ResourceScope Self(Guid userId) => new(ScopeType.Self, userId);
}
