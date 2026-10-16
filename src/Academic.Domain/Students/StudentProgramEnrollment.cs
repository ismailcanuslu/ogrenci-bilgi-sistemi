using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public sealed class StudentProgramEnrollment : Entity<Guid>
{
    public Guid StudentId { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid ProgramId { get; private set; }

    public Guid CurriculumVersionId { get; private set; }

    public EnrollmentType EnrollmentType { get; private set; }

    public StudentStatus Status { get; private set; }

    public int EnrollmentYear { get; private set; }

    public DateTime EnrollmentDateUtc { get; private set; }

    public DateTime? CompletionDateUtc { get; private set; }

    public bool IsPrimary { get; private set; }

    public string? Notes { get; private set; }

    private StudentProgramEnrollment()
    {
    }

    internal StudentProgramEnrollment(
        Guid id,
        Guid studentId,
        Guid departmentId,
        Guid programId,
        Guid curriculumVersionId,
        EnrollmentType enrollmentType,
        int enrollmentYear,
        DateTime enrollmentDateUtc,
        bool isPrimary,
        string? notes = null)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentProgramEnrollment.StudentRequired");
        }

        if (departmentId == Guid.Empty)
        {
            throw new DomainException("StudentProgramEnrollment.DepartmentRequired");
        }

        if (programId == Guid.Empty)
        {
            throw new DomainException("StudentProgramEnrollment.ProgramRequired");
        }

        if (curriculumVersionId == Guid.Empty)
        {
            throw new DomainException("StudentProgramEnrollment.CurriculumVersionRequired");
        }

        StudentId = studentId;
        DepartmentId = departmentId;
        ProgramId = programId;
        CurriculumVersionId = curriculumVersionId;
        EnrollmentType = enrollmentType;
        Status = StudentStatus.Active;
        EnrollmentYear = enrollmentYear;
        EnrollmentDateUtc = enrollmentDateUtc;
        IsPrimary = isPrimary;
        Notes = notes?.Trim();
    }

    internal void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    internal void ChangeStatus(StudentStatus newStatus, DateTime? completionDateUtc = null)
    {
        if (Status == StudentStatus.Dismissed && newStatus != StudentStatus.Dismissed)
        {
            throw new DomainException("StudentProgramEnrollment.DismissedStatusCannotBeReversed");
        }

        Status = newStatus;
        if (newStatus == StudentStatus.Graduated || newStatus == StudentStatus.Dismissed)
        {
            CompletionDateUtc = completionDateUtc ?? DateTime.UtcNow;
        }
    }

    internal void UpdateCurriculumVersion(Guid newCurriculumVersionId)
    {
        if (newCurriculumVersionId == Guid.Empty)
        {
            throw new DomainException("StudentProgramEnrollment.CurriculumVersionRequired");
        }

        CurriculumVersionId = newCurriculumVersionId;
    }
}
