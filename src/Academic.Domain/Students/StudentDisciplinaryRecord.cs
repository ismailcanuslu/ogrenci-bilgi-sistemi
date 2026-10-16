using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public enum DisciplinaryPenaltyType
{
    Warning = 1,       // Uyarı
    Reprimand = 2,     // Kınama
    Suspension = 3,    // Uzaklaştırma (1 hafta - 1 ay - 1 yarıyıl)
    Expulsion = 4      // Üniversiteden Çıkarma
}

public sealed class StudentDisciplinaryRecord : Entity<Guid>
{
    public const int ReasonMaxLength = 1000;
    public const int DecisionNumberMaxLength = 50;

    public Guid StudentId { get; private set; }

    public DisciplinaryPenaltyType PenaltyType { get; private set; }

    public string Reason { get; private set; } = null!;

    public string? DecisionNumber { get; private set; }

    public DateOnly DecisionDate { get; private set; }

    public int? DurationDays { get; private set; }

    public bool IsActive { get; private set; }

    private StudentDisciplinaryRecord()
    {
    }

    public StudentDisciplinaryRecord(
        Guid id,
        Guid studentId,
        DisciplinaryPenaltyType penaltyType,
        string reason,
        string? decisionNumber,
        DateOnly decisionDate,
        int? durationDays,
        bool isActive = true)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentDisciplinaryRecord.StudentRequired");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("StudentDisciplinaryRecord.ReasonRequired");
        }

        StudentId = studentId;
        PenaltyType = penaltyType;
        Reason = reason.Trim();
        DecisionNumber = decisionNumber?.Trim();
        DecisionDate = decisionDate;
        DurationDays = durationDays;
        IsActive = isActive;
    }
}
