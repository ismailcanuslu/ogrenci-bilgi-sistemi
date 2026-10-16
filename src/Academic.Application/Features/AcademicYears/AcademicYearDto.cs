using Academic.Domain.AcademicYears;

namespace Academic.Application.Features.AcademicYears;

public sealed record AcademicTermDto(
    Guid Id,
    Guid AcademicYearId,
    TermType TermType,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsRegistrationOpen,
    bool IsActive)
{
    public static AcademicTermDto From(AcademicTerm term)
        => new(
            term.Id,
            term.AcademicYearId,
            term.TermType,
            term.Name,
            term.StartDate,
            term.EndDate,
            term.IsRegistrationOpen,
            term.IsActive);
}

public sealed record AcademicYearDto(
    Guid Id,
    string Code,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsActive,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<AcademicTermDto> Terms)
{
    public static AcademicYearDto From(AcademicYear year)
        => new(
            year.Id,
            year.Code,
            year.Name,
            year.StartDate,
            year.EndDate,
            year.IsActive,
            year.CreatedAtUtc,
            year.Terms.Select(AcademicTermDto.From).ToArray());
}
