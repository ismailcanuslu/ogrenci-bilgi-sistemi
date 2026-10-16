using Academic.Domain.Departments;

namespace Academic.Application.Features.Departments;

public sealed record DepartmentDto(
    Guid Id,
    Guid FacultyId,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc)
{
    public static DepartmentDto From(Department department)
        => new(
            department.Id, department.FacultyId, department.Code, department.Name,
            department.IsActive, department.CreatedAtUtc, department.UpdatedAtUtc);
}
