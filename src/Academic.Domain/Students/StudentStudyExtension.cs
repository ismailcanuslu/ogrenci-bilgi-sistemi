using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public sealed class StudentStudyExtension : Entity<Guid>
{
    public const int NotesMaxLength = 500;

    public Guid StudentId { get; private set; }

    public int StandardStudyTerms { get; private set; } // Örn: 8 dönem (4 yıl)

    public int CompletedTerms { get; private set; } // Görseldeki gibi örn: 9

    public int MaximumAllowedTerms { get; private set; } // Örn: 14 dönem (7 yıl)

    public int RemainingTerms => Math.Max(0, MaximumAllowedTerms - CompletedTerms);

    public bool IsInExtendedPeriod => CompletedTerms > StandardStudyTerms;

    public bool HasAdditionalExamRights { get; private set; }

    public int AdditionalExamRightsCount { get; private set; }

    public string? Notes { get; private set; }

    private StudentStudyExtension()
    {
    }

    public StudentStudyExtension(
        Guid id,
        Guid studentId,
        int standardStudyTerms,
        int completedTerms,
        int maximumAllowedTerms,
        bool hasAdditionalExamRights,
        int additionalExamRightsCount,
        string? notes)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentStudyExtension.StudentRequired");
        }

        StudentId = studentId;
        StandardStudyTerms = standardStudyTerms;
        CompletedTerms = completedTerms;
        MaximumAllowedTerms = maximumAllowedTerms;
        HasAdditionalExamRights = hasAdditionalExamRights;
        AdditionalExamRightsCount = additionalExamRightsCount;
        Notes = notes?.Trim();
    }
}
