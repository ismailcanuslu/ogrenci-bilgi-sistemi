using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public sealed class Student : AggregateRoot<Guid>
{
    public const int StudentNumberMaxLength = 30;
    public const int NameMaxLength = 100;
    public const int EmailMaxLength = 256;

    private readonly List<StudentProgramEnrollment> _programEnrollments = [];

    public string StudentNumber { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public Guid? UserId { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid ProgramId { get; private set; }

    public Guid CurriculumVersionId { get; private set; }

    public int EnrollmentYear { get; private set; }

    public StudentStatus Status { get; private set; }

    public DateTime EnrollmentDateUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<StudentProgramEnrollment> ProgramEnrollments => _programEnrollments.AsReadOnly();

    public StudentProgramEnrollment? PrimaryEnrollment => _programEnrollments.FirstOrDefault(p => p.IsPrimary);

    private Student()
    {
    }

    private Student(
        Guid id,
        string studentNumber,
        string firstName,
        string lastName,
        string email,
        Guid departmentId,
        Guid programId,
        Guid curriculumVersionId,
        int enrollmentYear,
        DateTime enrollmentDateUtc,
        Guid? userId,
        DateTime createdAtUtc)
        : base(id)
    {
        StudentNumber = NormalizeStudentNumber(studentNumber);
        FirstName = NormalizeName(firstName, "FirstName");
        LastName = NormalizeName(lastName, "LastName");
        Email = NormalizeEmail(email);
        DepartmentId = departmentId;
        ProgramId = programId;
        CurriculumVersionId = curriculumVersionId;
        EnrollmentYear = ValidateEnrollmentYear(enrollmentYear);
        EnrollmentDateUtc = enrollmentDateUtc;
        UserId = userId;
        Status = StudentStatus.Active;
        CreatedAtUtc = createdAtUtc;

        // İlk kayıt otomatik olarak birincil ana dal programı (Major) olarak eklenir
        _programEnrollments.Add(new StudentProgramEnrollment(
            Guid.NewGuid(),
            id,
            departmentId,
            programId,
            curriculumVersionId,
            EnrollmentType.Major,
            enrollmentYear,
            enrollmentDateUtc,
            isPrimary: true,
            notes: "Asil Ana Dal Kaydı"));
    }

    public static Student Create(
        string studentNumber,
        string firstName,
        string lastName,
        string email,
        Guid departmentId,
        Guid programId,
        Guid curriculumVersionId,
        int enrollmentYear,
        DateTime enrollmentDateUtc,
        Guid? userId,
        DateTime createdAtUtc)
    {
        if (departmentId == Guid.Empty)
        {
            throw new DomainException("Student.DepartmentRequired");
        }

        if (programId == Guid.Empty)
        {
            throw new DomainException("Student.ProgramRequired");
        }

        if (curriculumVersionId == Guid.Empty)
        {
            throw new DomainException("Student.CurriculumVersionRequired");
        }

        return new Student(
            Guid.NewGuid(),
            studentNumber,
            firstName,
            lastName,
            email,
            departmentId,
            programId,
            curriculumVersionId,
            enrollmentYear,
            enrollmentDateUtc,
            userId,
            createdAtUtc);
    }

    public void UpdateProfile(string firstName, string lastName, string email, DateTime updatedAtUtc)
    {
        EnsureNotDismissed();

        FirstName = NormalizeName(firstName, "FirstName");
        LastName = NormalizeName(lastName, "LastName");
        Email = NormalizeEmail(email);
        UpdatedAtUtc = updatedAtUtc;
    }

    public void ChangeProgramAndCurriculum(
        Guid departmentId,
        Guid programId,
        Guid curriculumVersionId,
        DateTime updatedAtUtc)
    {
        EnsureNotDismissed();

        if (departmentId == Guid.Empty)
        {
            throw new DomainException("Student.DepartmentRequired");
        }

        if (programId == Guid.Empty)
        {
            throw new DomainException("Student.ProgramRequired");
        }

        if (curriculumVersionId == Guid.Empty)
        {
            throw new DomainException("Student.CurriculumVersionRequired");
        }

        DepartmentId = departmentId;
        ProgramId = programId;
        CurriculumVersionId = curriculumVersionId;
        UpdatedAtUtc = updatedAtUtc;
    }

    public StudentProgramEnrollment EnrollInProgram(
        Guid departmentId,
        Guid programId,
        Guid curriculumVersionId,
        EnrollmentType enrollmentType,
        int enrollmentYear,
        DateTime enrollmentDateUtc,
        bool isPrimary = false,
        string? notes = null)
    {
        EnsureNotDismissed();

        if (_programEnrollments.Any(p => p.ProgramId == programId && p.Status == StudentStatus.Active))
        {
            throw new DomainException("Student.AlreadyEnrolledInProgram");
        }

        if (isPrimary)
        {
            foreach (var existing in _programEnrollments.Where(p => p.IsPrimary))
            {
                existing.SetPrimary(false);
            }
        }

        var enrollment = new StudentProgramEnrollment(
            Guid.NewGuid(),
            Id,
            departmentId,
            programId,
            curriculumVersionId,
            enrollmentType,
            ValidateEnrollmentYear(enrollmentYear),
            enrollmentDateUtc,
            isPrimary || _programEnrollments.Count == 0,
            notes);

        _programEnrollments.Add(enrollment);

        if (enrollment.IsPrimary)
        {
            DepartmentId = departmentId;
            ProgramId = programId;
            CurriculumVersionId = curriculumVersionId;
        }

        UpdatedAtUtc = enrollmentDateUtc;
        return enrollment;
    }

    public void SetPrimaryProgram(Guid programEnrollmentId, DateTime updatedAtUtc)
    {
        EnsureNotDismissed();

        var target = _programEnrollments.FirstOrDefault(p => p.Id == programEnrollmentId);
        if (target is null)
        {
            throw new DomainException("Student.ProgramEnrollmentNotFound");
        }

        if (target.Status != StudentStatus.Active)
        {
            throw new DomainException("Student.InactiveProgramCannotBePrimary");
        }

        foreach (var existing in _programEnrollments)
        {
            existing.SetPrimary(existing.Id == programEnrollmentId);
        }

        DepartmentId = target.DepartmentId;
        ProgramId = target.ProgramId;
        CurriculumVersionId = target.CurriculumVersionId;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void UpdateProgramStatus(
        Guid programEnrollmentId,
        StudentStatus newStatus,
        DateTime updatedAtUtc)
    {
        var target = _programEnrollments.FirstOrDefault(p => p.Id == programEnrollmentId);
        if (target is null)
        {
            throw new DomainException("Student.ProgramEnrollmentNotFound");
        }

        target.ChangeStatus(newStatus, updatedAtUtc);

        if (target.IsPrimary && newStatus != StudentStatus.Active)
        {
            var nextActive = _programEnrollments.FirstOrDefault(p => p.Id != programEnrollmentId && p.Status == StudentStatus.Active);
            if (nextActive is not null)
            {
                SetPrimaryProgram(nextActive.Id, updatedAtUtc);
            }
        }

        UpdatedAtUtc = updatedAtUtc;
    }

    public void ChangeStatus(StudentStatus newStatus, DateTime updatedAtUtc)
    {
        if (Status == newStatus)
        {
            return;
        }

        if (Status == StudentStatus.Dismissed && newStatus != StudentStatus.Dismissed)
        {
            throw new DomainException("Student.DismissedStatusCannotBeReversed");
        }

        Status = newStatus;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void AssignUser(Guid userId, DateTime updatedAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("Student.UserRequired");
        }

        UserId = userId;
        UpdatedAtUtc = updatedAtUtc;
    }

    private void EnsureNotDismissed()
    {
        if (Status == StudentStatus.Dismissed)
        {
            throw new DomainException("Student.Dismissed");
        }
    }

    public static string NormalizeStudentNumber(string studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            throw new DomainException("Student.StudentNumberRequired");
        }

        var trimmed = studentNumber.Trim().ToUpperInvariant();
        if (trimmed.Length > StudentNumberMaxLength)
        {
            throw new DomainException("Student.StudentNumberTooLong");
        }

        return trimmed;
    }

    private static string NormalizeName(string name, string field)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException($"Student.{field}Required");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > NameMaxLength)
        {
            throw new DomainException($"Student.{field}TooLong");
        }

        return trimmed;
    }

    public static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Student.EmailRequired");
        }

        var trimmed = email.Trim().ToLowerInvariant();
        if (trimmed.Length > EmailMaxLength)
        {
            throw new DomainException("Student.EmailTooLong");
        }

        if (!trimmed.Contains('@') || !trimmed.Contains('.'))
        {
            throw new DomainException("Student.InvalidEmail");
        }

        return trimmed;
    }

    private static int ValidateEnrollmentYear(int year)
    {
        if (year < 1950 || year > 2100)
        {
            throw new DomainException("Student.InvalidEnrollmentYear");
        }

        return year;
    }
}
