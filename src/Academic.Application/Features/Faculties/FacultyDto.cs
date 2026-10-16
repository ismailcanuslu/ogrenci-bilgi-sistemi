using Academic.Domain.Faculties;

namespace Academic.Application.Features.Faculties;

public sealed record FacultyDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc)
{
    public static FacultyDto From(Faculty faculty)
        => new(faculty.Id, faculty.Code, faculty.Name, faculty.IsActive, faculty.CreatedAtUtc, faculty.UpdatedAtUtc);
}
