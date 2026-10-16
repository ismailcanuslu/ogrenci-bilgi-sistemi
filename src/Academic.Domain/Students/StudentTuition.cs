using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public sealed class StudentTuition : Entity<Guid>
{
    public const int TermNameMaxLength = 50;
    public const int NotesMaxLength = 500;

    public Guid StudentId { get; private set; }

    public string TermName { get; private set; } = null!;

    public decimal TotalAmount { get; private set; }

    public decimal PaidAmount { get; private set; }

    public decimal RemainingDebt => Math.Max(0, TotalAmount - PaidAmount);

    public bool IsPaid => RemainingDebt <= 0;

    public bool IsExempt { get; private set; }

    public DateTime DueDateUtc { get; private set; }

    public string? Notes { get; private set; }

    private StudentTuition()
    {
    }

    public StudentTuition(
        Guid id,
        Guid studentId,
        string termName,
        decimal totalAmount,
        DateTime dueDateUtc,
        bool isExempt = false,
        string? notes = null)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentTuition.StudentRequired");
        }

        if (string.IsNullOrWhiteSpace(termName))
        {
            throw new DomainException("StudentTuition.TermNameRequired");
        }

        StudentId = studentId;
        TermName = termName.Trim();
        TotalAmount = totalAmount < 0 ? 0 : totalAmount;
        PaidAmount = 0;
        DueDateUtc = dueDateUtc;
        IsExempt = isExempt;
        Notes = notes?.Trim();

        if (isExempt)
        {
            PaidAmount = TotalAmount;
        }
    }

    public void RecordPayment(decimal amount)
    {
        if (amount <= 0)
        {
            throw new DomainException("StudentTuition.InvalidPaymentAmount");
        }

        PaidAmount += amount;
    }
}
