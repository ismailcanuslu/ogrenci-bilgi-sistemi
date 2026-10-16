using Academic.Domain.Programs;

namespace Academic.Application.Features.Programs;

public sealed record ProgramDto(
    Guid Id,
    Guid DepartmentId,
    string Code,
    string Name,
    DegreeLevel DegreeLevel,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc)
{
    public static ProgramDto From(Program program)
        => new(
            program.Id,
            program.DepartmentId,
            program.Code,
            program.Name,
            program.DegreeLevel,
            program.IsActive,
            program.CreatedAtUtc,
            program.UpdatedAtUtc);
}
