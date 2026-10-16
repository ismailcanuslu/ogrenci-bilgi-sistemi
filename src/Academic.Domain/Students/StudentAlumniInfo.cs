using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public sealed class StudentAlumniInfo : Entity<Guid>
{
    public const int CompanyMaxLength = 200;
    public const int PositionMaxLength = 100;
    public const int UrlMaxLength = 500;
    public const int EmailMaxLength = 256;
    public const int PhoneMaxLength = 50;

    public Guid StudentId { get; private set; }

    public int? GraduationYear { get; private set; }

    public string? CurrentCompany { get; private set; }

    public string? CurrentPosition { get; private set; }

    public string? LinkedInUrl { get; private set; }

    public string? PersonalEmail { get; private set; }

    public string? PhoneNumber { get; private set; }

    public bool IsVisibleToOtherAlumni { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    private StudentAlumniInfo()
    {
    }

    public StudentAlumniInfo(
        Guid id,
        Guid studentId,
        int? graduationYear,
        string? currentCompany,
        string? currentPosition,
        string? linkedInUrl,
        string? personalEmail,
        string? phoneNumber,
        bool isVisibleToOtherAlumni,
        DateTime updatedAtUtc)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentAlumniInfo.StudentRequired");
        }

        StudentId = studentId;
        Update(graduationYear, currentCompany, currentPosition, linkedInUrl, personalEmail, phoneNumber, isVisibleToOtherAlumni, updatedAtUtc);
    }

    public void Update(
        int? graduationYear,
        string? currentCompany,
        string? currentPosition,
        string? linkedInUrl,
        string? personalEmail,
        string? phoneNumber,
        bool isVisibleToOtherAlumni,
        DateTime updatedAtUtc)
    {
        GraduationYear = graduationYear;
        CurrentCompany = currentCompany?.Trim();
        CurrentPosition = currentPosition?.Trim();
        LinkedInUrl = linkedInUrl?.Trim();
        PersonalEmail = personalEmail?.Trim();
        PhoneNumber = phoneNumber?.Trim();
        IsVisibleToOtherAlumni = isVisibleToOtherAlumni;
        UpdatedAtUtc = updatedAtUtc;
    }
}
