using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public sealed class StudentMilitaryStatus : Entity<Guid>
{
    public const int StatusCodeMaxLength = 50;
    public const int DocumentNoMaxLength = 100;
    public const int NotesMaxLength = 500;

    public Guid StudentId { get; private set; }

    public string StatusCode { get; private set; } = null!; // Tecilli, Muaf, Terhisli, İşlemde vb.

    public DateOnly? PostponementEndDate { get; private set; } // Tecil Bitiş Tarihi

    public string? AsalDocumentNo { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public string? Notes { get; private set; }

    private StudentMilitaryStatus()
    {
    }

    public StudentMilitaryStatus(
        Guid id,
        Guid studentId,
        string statusCode,
        DateOnly? postponementEndDate,
        string? asalDocumentNo,
        string? notes,
        DateTime updatedAtUtc)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentMilitaryStatus.StudentRequired");
        }

        if (string.IsNullOrWhiteSpace(statusCode))
        {
            throw new DomainException("StudentMilitaryStatus.StatusCodeRequired");
        }

        StudentId = studentId;
        StatusCode = statusCode.Trim();
        PostponementEndDate = postponementEndDate;
        AsalDocumentNo = asalDocumentNo?.Trim();
        Notes = notes?.Trim();
        UpdatedAtUtc = updatedAtUtc;
    }
}
