using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.AcademicYears;

public sealed class AcademicTerm : Entity<Guid>
{
    public const int NameMaxLength = 100;

    public Guid AcademicYearId { get; private set; }

    public TermType TermType { get; private set; }

    public string Name { get; private set; } = null!;

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public bool IsRegistrationOpen { get; private set; }

    public bool IsActive { get; private set; }

    private AcademicTerm()
    {
    }

    internal AcademicTerm(
        Guid id,
        Guid academicYearId,
        TermType termType,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        bool isRegistrationOpen)
        : base(id)
    {
        AcademicYearId = academicYearId;
        TermType = termType;
        Name = NormalizeName(name);
        ValidateDates(startDate, endDate);
        StartDate = startDate;
        EndDate = endDate;
        IsRegistrationOpen = isRegistrationOpen;
        IsActive = true;
    }

    public void Update(string name, DateOnly startDate, DateOnly endDate)
    {
        Name = NormalizeName(name);
        ValidateDates(startDate, endDate);
        StartDate = startDate;
        EndDate = endDate;
    }

    public void SetRegistrationStatus(bool isOpen)
    {
        IsRegistrationOpen = isOpen;
    }

    public void Deactivate()
    {
        IsActive = false;
        IsRegistrationOpen = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("AcademicTerm.NameRequired");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > NameMaxLength)
        {
            throw new DomainException("AcademicTerm.NameTooLong");
        }

        return trimmed;
    }

    private static void ValidateDates(DateOnly startDate, DateOnly endDate)
    {
        if (startDate >= endDate)
        {
            throw new DomainException("AcademicTerm.InvalidDateRange");
        }
    }
}
