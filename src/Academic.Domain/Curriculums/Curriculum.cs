using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Curriculums;

public sealed class Curriculum : AggregateRoot<Guid>
{
    public const int CodeMaxLength = 30;
    public const int NameMaxLength = 200;

    private readonly List<CurriculumVersion> _versions = [];

    public Guid ProgramId { get; private set; }

    public Guid AcademicYearId { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<CurriculumVersion> Versions => _versions.AsReadOnly();

    private Curriculum()
    {
    }

    private Curriculum(
        Guid id,
        Guid programId,
        Guid academicYearId,
        string code,
        string name,
        DateTime createdAtUtc)
        : base(id)
    {
        ProgramId = programId;
        AcademicYearId = academicYearId;
        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public static Curriculum Create(
        Guid programId,
        Guid academicYearId,
        string code,
        string name,
        DateTime createdAtUtc)
    {
        if (programId == Guid.Empty)
        {
            throw new DomainException("Curriculum.ProgramRequired");
        }

        if (academicYearId == Guid.Empty)
        {
            throw new DomainException("Curriculum.AcademicYearRequired");
        }

        var curriculum = new Curriculum(Guid.NewGuid(), programId, academicYearId, code, name, createdAtUtc);

        // Varsayılan ilk taslak sürümü (Version 1) otomatik ekle
        curriculum._versions.Add(new CurriculumVersion(Guid.NewGuid(), curriculum.Id, 1, "Başlangıç Sürümü", createdAtUtc));

        return curriculum;
    }

    public void Update(string code, string name, DateTime updatedAtUtc)
    {
        EnsureActive();

        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        UpdatedAtUtc = updatedAtUtc;
    }

    public CurriculumVersion CreateNewVersion(string? notes, DateTime createdAtUtc)
    {
        EnsureActive();

        if (_versions.Any(v => v.Status == CurriculumVersionStatus.Draft))
        {
            throw new DomainException("Curriculum.ExistingDraftVersionMustBeResolved");
        }

        var nextVersionNumber = _versions.Count == 0 ? 1 : _versions.Max(v => v.VersionNumber) + 1;
        var newVersion = new CurriculumVersion(Guid.NewGuid(), Id, nextVersionNumber, notes, createdAtUtc);

        // Bir önceki published sürümün derslerini kopyalayarak yeni taslak sürümü hazırla
        var latestPublished = _versions
            .Where(v => v.Status == CurriculumVersionStatus.Published)
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefault();

        if (latestPublished is not null)
        {
            foreach (var course in latestPublished.Courses)
            {
                newVersion.AddCourse(
                    course.CourseCode,
                    course.CourseName,
                    course.Ects,
                    course.Credit,
                    course.Semester,
                    course.IsElective,
                    course.Category,
                    course.Prerequisites);
            }
        }

        _versions.Add(newVersion);
        return newVersion;
    }

    public void PublishVersion(Guid versionId, DateTime publishedAtUtc)
    {
        EnsureActive();

        var version = _versions.FirstOrDefault(v => v.Id == versionId);
        if (version is null)
        {
            throw new DomainException("Curriculum.VersionNotFound");
        }

        // Önceki yayınlanmış sürümleri arşivle
        foreach (var previous in _versions.Where(v => v.Status == CurriculumVersionStatus.Published && v.Id != versionId))
        {
            previous.Archive();
        }

        version.Publish(publishedAtUtc);
    }

    public void Deactivate(DateTime updatedAtUtc)
    {
        if (!IsActive)
        {
            throw new DomainException("Curriculum.AlreadyInactive");
        }

        IsActive = false;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Activate(DateTime updatedAtUtc)
    {
        if (IsActive)
        {
            throw new DomainException("Curriculum.AlreadyActive");
        }

        IsActive = true;
        UpdatedAtUtc = updatedAtUtc;
    }

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new DomainException("Curriculum.Inactive");
        }
    }

    public static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Curriculum.CodeRequired");
        }

        var trimmed = code.Trim().ToUpperInvariant();
        if (trimmed.Length > CodeMaxLength)
        {
            throw new DomainException("Curriculum.CodeTooLong");
        }

        return trimmed;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Curriculum.NameRequired");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > NameMaxLength)
        {
            throw new DomainException("Curriculum.NameTooLong");
        }

        return trimmed;
    }
}
