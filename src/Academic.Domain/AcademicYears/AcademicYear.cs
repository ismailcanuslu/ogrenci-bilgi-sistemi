using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.AcademicYears;

public sealed class AcademicYear : AggregateRoot<Guid>
{
    public const int CodeMaxLength = 20;
    public const int NameMaxLength = 100;

    private readonly List<AcademicTerm> _terms = [];

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<AcademicTerm> Terms => _terms.AsReadOnly();

    private AcademicYear()
    {
    }

    private AcademicYear(
        Guid id,
        string code,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        DateTime createdAtUtc)
        : base(id)
    {
        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        ValidateDates(startDate, endDate);
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public static AcademicYear Create(
        string code,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        DateTime createdAtUtc)
        => new(Guid.NewGuid(), code, name, startDate, endDate, createdAtUtc);

    public void Update(string code, string name, DateOnly startDate, DateOnly endDate, DateTime updatedAtUtc)
    {
        EnsureActive();

        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        ValidateDates(startDate, endDate);
        StartDate = startDate;
        EndDate = endDate;
        UpdatedAtUtc = updatedAtUtc;
    }

    public AcademicTerm AddTerm(
        TermType termType,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        bool isRegistrationOpen)
    {
        EnsureActive();

        if (!Enum.IsDefined(termType))
        {
            throw new DomainException("AcademicTerm.InvalidTermType");
        }

        if (_terms.Any(t => t.TermType == termType && t.IsActive))
        {
            throw new DomainException("AcademicTerm.TermTypeAlreadyExistsInYear");
        }

        var term = new AcademicTerm(
            Guid.NewGuid(),
            Id,
            termType,
            name,
            startDate,
            endDate,
            isRegistrationOpen);

        _terms.Add(term);
        return term;
    }

    public void Deactivate(DateTime updatedAtUtc)
    {
        if (!IsActive)
        {
            throw new DomainException("AcademicYear.AlreadyInactive");
        }

        IsActive = false;
        UpdatedAtUtc = updatedAtUtc;

        foreach (var term in _terms)
        {
            term.Deactivate();
        }
    }

    public void Activate(DateTime updatedAtUtc)
    {
        if (IsActive)
        {
            throw new DomainException("AcademicYear.AlreadyActive");
        }

        IsActive = true;
        UpdatedAtUtc = updatedAtUtc;
    }

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new DomainException("AcademicYear.Inactive");
        }
    }

    public static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("AcademicYear.CodeRequired");
        }

        var trimmed = code.Trim().ToUpperInvariant();
        if (trimmed.Length > CodeMaxLength)
        {
            throw new DomainException("AcademicYear.CodeTooLong");
        }

        return trimmed;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("AcademicYear.NameRequired");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > NameMaxLength)
        {
            throw new DomainException("AcademicYear.NameTooLong");
        }

        return trimmed;
    }

    private static void ValidateDates(DateOnly startDate, DateOnly endDate)
    {
        if (startDate >= endDate)
        {
            throw new DomainException("AcademicYear.InvalidDateRange");
        }
    }
}
